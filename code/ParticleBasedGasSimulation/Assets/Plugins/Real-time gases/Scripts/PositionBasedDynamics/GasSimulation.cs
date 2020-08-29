using System;
using System.Collections.Generic;
using Common.LinearAlgebra;
using Common.Shapes;
using Plugins.Scripts.PositionBasedDynamics.Area;
using UnityEngine;
using Random = System.Random;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <inheritdoc />
    /// <summary>
    /// Main class of simulation. Initializes the attributes from the GUI and starts and updates the simulation.   
    /// </summary>
    public class GasSimulation : MonoBehaviour
    {
        public double timeStep;

        public int numberOfIterations;

        public bool simulationMode;

        public double simulationDistance;

        public double radius;

        public double spawnRate;

        public List<FluidCreation> fluids;
        public List<FluidCreation> runtimeFluids;
        public Force force;

        private FluidBody Body { get; set; }
        private Solver Solver { get; set; }

        private AreaRaster _areaRaster;

        private readonly Random _random = new Random();

        private Box3D _fluidBounds, _outerBounds, _innerBounds;

        private Vector3D _startLocation;
        private readonly Vector3D _velocity = new Vector3D(0, 0, 0);
        private readonly Vector3D _vorticity = new Vector3D(0, 0, 0);

        private double _elapsed = 0.0;


        private void Awake()
        {
            var position = transform.position;
            _startLocation = new Vector3D(position.x, position.y, position.z);
        }

        private void Start()
        {
            _areaRaster = AreaRaster.RasterArea;

            Body = new FluidBody(radius, GasPool.SharedInstance.amountToPool, numberOfIterations);

            CreateFluid();

            Solver = new Solver(Body, force);
        }

        private void Update()
        {
            if (simulationMode && (!Input.GetKey("e") || !simulationMode)) return;

            Solver.StepPhysics(timeStep);

            UpdateSpheres();

            _elapsed += Time.deltaTime;

            if (!(_elapsed >= spawnRate)) return;
            _elapsed = 0.0f;
            CreateRuntimeFluid();
        }

        /// <summary>
        /// Creates all particles which are created at the start.
        /// </summary>
        private void CreateFluid()
        {
            var d = Body.ParticleDiameter;
            var scale = new Vector3((float) d, (float) d, (float) d);

            foreach (var fluid in fluids)
            {
                for (var i = 0; i < fluid.bounds.NumParticles; i++)
                {
                    var pos = fluid.bounds.positions[i];

                    if (fluid.mass.Equals(0)) fluid.mass = 0.8 * d * d * d * fluid.density;

                    var gasSphere = GasPool.SharedInstance.GetPooledObject();
                    if (ReferenceEquals(gasSphere, null)) continue;
                    gasSphere.transform.position = new Vector3((float) pos.x, (float) pos.y, (float) pos.z);
                    gasSphere.transform.localScale = scale;
                    gasSphere.SetActive(true);

                    Body.Fluids.Add(new Fluid(pos, pos, _velocity, _vorticity, fluid.mass, fluid.density,
                        fluid.damping, fluid.viscosity, fluid.toxicity, gasSphere));
                    Body.Hash.NumParticles++;
                }
            }

            CreateRuntimeFluid();
        }

        /// <summary>
        /// Runtime creation of particles.
        /// </summary>
        private void CreateRuntimeFluid()
        {
            var diam = (float) Body.ParticleDiameter;

            foreach (var fluid in runtimeFluids)
            {
                var gasSphere = GasPool.SharedInstance.GetPooledObject();
                if (ReferenceEquals(gasSphere, null)) continue;
                if (fluid.mass.Equals(0)) fluid.mass = 0.8 * diam * diam * diam * fluid.density;

                var position = fluid.gameObject.transform.position;
                fluid.position = new Vector3D(position.x, position.y, position.z);

                var random = _random.NextDouble();

                gasSphere.transform.position = new Vector3((float) (fluid.position.x + random),
                    (float) (fluid.position.y + random),
                    (float) (fluid.position.z + random));
                gasSphere.transform.localScale = new Vector3(diam, diam, diam);
                gasSphere.SetActive(true);

                Body.Fluids.Add(new Fluid(fluid.position + random, fluid.position + random, _velocity, _vorticity,
                    fluid.mass, fluid.density, fluid.damping, fluid.viscosity, fluid.toxicity, gasSphere));
                Body.Hash.NumParticles++;
            }
        }

        /// <summary>
        /// Updates the particles GameObject position, taking the new calculated position.
        /// </summary>
        private void UpdateSpheres()
        {
            var tmpVolumeLocation = Vector3D.Zero;
            var gasAreas = _areaRaster.areaGas;
            var fluidObjects = Body.Fluids;
            var hash = Body.Hash;

            for (var i = 0; i < fluidObjects.Count; i++)
            {
                var pos = fluidObjects[i].Position;

                tmpVolumeLocation += pos;

                var position = Math.Sqrt(Math.Pow((pos.x - _startLocation.x), 2) +
                                         Math.Pow((pos.y - _startLocation.y), 2) +
                                         Math.Pow((pos.z - _startLocation.z), 2));

                if (position >= simulationDistance)
                {
                    fluidObjects[i].FluidSphere.SetActive(false);
                    fluidObjects.RemoveAt(i);
                    hash.NumParticles--;
                }
                else
                {
                    fluidObjects[i].FluidSphere.transform.position =
                        new Vector3((float) pos.x, (float) pos.y, (float) pos.z);

                    var newArea = true;
                    foreach (var area in gasAreas)
                    {
                        var areaRadius = area.GasArea.transform.lossyScale.x / 2;

                        position = Math.Sqrt(Math.Pow((pos.x - area.VolumeLocation.x), 2) +
                                             Math.Pow((pos.y - area.VolumeLocation.y), 2) +
                                             Math.Pow((pos.z - area.VolumeLocation.z), 2));

                        if (!(position <= areaRadius)) continue;
                        area.Density += (float) (fluidObjects[i].Toxicity * radius) / areaRadius;
                        newArea = false;
                    }

                    if (newArea && _areaRaster.staticAreaMode && !_areaRaster.placedAreaMode) _areaRaster.NewGas(pos);
                }
            }

            _areaRaster.UpdateGasArea((tmpVolumeLocation / Body.Fluids.Count),
                Body.Fluids.Count);
        }
    }
}