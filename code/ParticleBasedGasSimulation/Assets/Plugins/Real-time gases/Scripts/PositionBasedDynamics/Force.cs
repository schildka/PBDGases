using System;
using System.Collections.Generic;
using Common.LinearAlgebra;
using Common.Shapes;
using UnityEngine;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <summary>
    /// This class applies the gravity, drag and wind forces to all particles.
    /// </summary>
    [Serializable]
    public class Force
    {
        [SerializeField] public Vector3D gravity;

        [SerializeField] public Vector3D drag;

        [SerializeField] public List<Vector3D> windForces;

        [SerializeField] public List<Box3D> windBoxes;

        private int _forceCount = 0;

        public void ApplyForce(double dt, FluidBody body)
        {
            var len = body.NumParticles;
            var fluids = body.Fluids;
            for (var i = 0; i < len; i++)
            {
                fluids[i].Velocity += dt * gravity;


                foreach (var box in windBoxes)
                {
                    if (box.Contains(fluids[i].Position))
                        fluids[i].Velocity += dt * windForces[_forceCount];
                    _forceCount++;
                }

                _forceCount = 0;
            }
        }
    }
}