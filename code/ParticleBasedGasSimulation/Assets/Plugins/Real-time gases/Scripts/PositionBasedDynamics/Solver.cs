using Common.LinearAlgebra;
using UnityEngine;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <summary>
    /// Physics loop for particles.
    /// </summary>
    public class Solver
    {
        private FluidBody Body { get; set; }

        private Force Force { get; set; }

        private const double Correction = 0.5;

        private readonly Vector3D _dragForce;

        public Solver(FluidBody body, Force force)
        {
            Body = body;

            Force = force;
            _dragForce = force.drag;
        }

        public void StepPhysics(double dt)
        {
            if (dt.Equals(0.0)) return;

            ApplyExternalForces(dt);

            EstimatePositions(dt);

            FindBounds();

            UpdateConstraint();

            UpdateVelocities(dt);

            Body.ComputeViscosity();

            UpdatePositions();
        }

        /// <summary>
        /// Applies damping, drag and wind forces.
        /// </summary>
        private void ApplyExternalForces(double dt)
        {
            var fluids = Body.Fluids;
            for (var i = 0; i < Body.NumParticles; i++)
            {
                fluids[i].Velocity -= (fluids[i].Velocity * fluids[i].Damping) * dt;
            }

            Force.ApplyForce(dt, Body);
        }

        /// <summary>
        /// Calculates predicted positions of particles based on their velocity.
        /// </summary>
        private void EstimatePositions(double dt)
        {
            var fluids = Body.Fluids;
            for (var i = 0; i < Body.NumParticles; i++)
            {
                fluids[i].Predicted = fluids[i].Position + dt * fluids[i].Velocity;
            }
        }

        /// <summary>
        /// Searches for neighboring solids.
        /// </summary>
        private void FindBounds()
        {
            var fluids = Body.Fluids;
            for (var i = 0; i < Body.NumParticles; i++)
            {
                var pre = new Vector3((float) fluids[i].Predicted.x, (float) fluids[i].Predicted.y,
                    (float) fluids[i].Predicted.z);

                fluids[i].Sphere.UpdateBounds(pre);
            }
        }

        private void UpdateConstraint()
        {
            Body.ConstrainPositions(1);
        }

        /// <summary>
        /// Calculates and applies drag and vorticity force.
        /// </summary>
        private void UpdateVelocities(double dt)
        {
            var invDt = 1.0 / dt;
            var neighbors = Body.Hash.Neighbors;
            var fluids = Body.Fluids;
            var hash = Body.Hash;
            var kernel = Body.Kernel;

            for (var i = 0; i < Body.NumParticles; i++)
            {
                var d = fluids[i].Predicted - fluids[i].Position;
                fluids[i].Velocity = d * invDt;

                // Drag force
                var drag = -0.5 * (fluids[i].Velocity - _dragForce) *
                           (1 - (fluids[i].Densities / fluids[i].Density));


                var n = Vector3D.Zero;

                for (var j = 0; j < hash.NumNeighbors[i]; j++)
                {
                    var neighborIndex = neighbors[i, j];
                    var neighbor = fluids[neighborIndex];

                    n += (neighbor.ParticleMass / neighbor.Densities) * kernel.GradW(
                             fluids[i].Position.x - neighbor.Position.x,
                             fluids[i].Position.y - neighbor.Position.y,
                             fluids[i].Position.z - neighbor.Position.z);
                }

                var points = fluids[i].Sphere.point;

                foreach (var entry in points)
                {
                    foreach (var ve in entry.Value)
                    {
                        n += kernel.GradW(fluids[i].Position.x - ve.x, fluids[i].Position.y - ve.y,
                            fluids[i].Position.z - ve.z);
                    }
                }

                //Boussinesq pressure gradient approximation
                fluids[i].Vorticity = dt *
                                      (0.3 * (n.Cross(new Vector3D(0, 9.81, 0))) + Vector3D.Dot(
                                           fluids[i].Vorticity, kernel.GradW(fluids[i].Velocity.x,
                                               fluids[i].Velocity.y, fluids[i].Velocity.z)));


                var vort = Vector3D.Zero;

                for (var j = 0; j < hash.NumNeighbors[i]; j++)
                {
                    var neighborIndex = neighbors[i, j];
                    var neighbor = fluids[neighborIndex];

                    vort += fluids[j].Vorticity.Cross(fluids[i].Position - neighbor.Position) *
                            kernel.W(fluids[i].Position.x - neighbor.Position.x,
                                fluids[i].Position.y - neighbor.Position.y,
                                fluids[i].Position.z - neighbor.Position.z);
                }

                fluids[i].Velocity =
                    fluids[i].Velocity + (((vort + drag) / fluids[i].ParticleMass) * dt);
            }
        }

        /// <summary>
        /// Updates the particles positions using the calculated predicted position.
        /// </summary>
        private void UpdatePositions()
        {
            var fluids = Body.Fluids;
            for (var i = 0; i < Body.NumParticles; i++)
            {
                fluids[i].Position = fluids[i].Predicted;
            }
        }
    }
}