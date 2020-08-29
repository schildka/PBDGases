using System.Collections.Generic;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <summary>
    /// This class holds all simulating particles.
    /// </summary>
    public class FluidBody
    {
        public int NumParticles => Fluids.Count;

        private double ParticleRadius { get; set; }

        public double ParticleDiameter => ParticleRadius * 2.0;

        public List<Fluid> Fluids { get; private set; }

        public FluidConstraint FluidConstraint { get; private set; }

        internal CubicKernel Kernel { get; private set; }

        internal ParticleHash Hash { get; private set; }

        public FluidBody(double radius, int maxParticles, int numberOfIterations)
        {
            Fluids = new List<Fluid>();

            ParticleRadius = radius;

            var cellSize = ParticleRadius * 4.0;
            Kernel = new CubicKernel(cellSize);
            Hash = new ParticleHash(cellSize, maxParticles);

            FluidConstraint = new FluidConstraint(this, numberOfIterations);
        }


        internal void ConstrainPositions(double di)
        {
            FluidConstraint.ConstrainPositions(di);
        }


        /// <summary>
        /// Computes the viscosity for all particles.
        /// </summary>
        internal void ComputeViscosity()
        {
            var neighbors = Hash.Neighbors;

            for (var i = 0; i < NumParticles; i++)
            {
                var viscosityMulMass = Fluids[i].Viscosity * Fluids[i].ParticleMass;

                var pi = Fluids[i].Predicted;

                for (var j = 0; j < Hash.NumNeighbors[i]; j++)
                {
                    var neighborIndex = neighbors[i, j];
                    if (neighborIndex >= NumParticles) continue;
                    var invDensity = 1.0 / Fluids[neighborIndex].Densities;
                    var pn = Fluids[neighborIndex].Predicted;

                    var k = Kernel.W(pi.x - pn.x, pi.y - pn.y, pi.z - pn.z) * viscosityMulMass * invDensity;
                    Fluids[i].Velocity -= k * (Fluids[i].Velocity - Fluids[neighborIndex].Velocity);
                }
            }
        }
    }
}