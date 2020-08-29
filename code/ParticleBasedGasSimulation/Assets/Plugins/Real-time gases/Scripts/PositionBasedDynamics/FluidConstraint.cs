using System.Collections.Generic;
using System.Linq;
using Common.LinearAlgebra;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <summary>
    /// Constraint for gases. Takes all particles and their neighbors and solid collisions.  
    /// </summary>
    public class FluidConstraint
    {
        private FluidBody Fluid { get; set; }

        public readonly double[] Psi = new double[5]
        {
            0.717662382055, 0.617662382055, 0.517662382055, 0.417662382055, 0.317662382055
        };

        private int Iterations { get; }

        internal FluidConstraint(FluidBody body, int numberOfIterations)
        {
            Iterations = numberOfIterations;
            Fluid = body;

            if (!(Fluid.ParticleDiameter > 0.5)) return;
            for (var i = 0; i < Psi.Length; i++)
            {
                Psi[i] = Psi[i] + Fluid.ParticleDiameter * 2;
            }
        }

        /// <summary>
        /// Calculates the new predicted positions for the particles, using the density constraint.
        /// </summary>
        internal void ConstrainPositions(double di)
        {
            if (Fluid == null) return;

            Fluid.Hash.NeighborhoodSearch(Fluid.Fluids.Select(x => x.Predicted).ToArray());

            var neighbors = Fluid.Hash.Neighbors;
            var fluids = Fluid.Fluids;
            var hash = Fluid.Hash;

            var iter = 0;
            while (iter < Iterations)
            {
                for (var i = 0; i < Fluid.NumParticles; i++)
                {
                    var points = fluids[i].Sphere.point;
                    var normals = fluids[i].Sphere.normal;

                    var pi = fluids[i].Predicted;

                    ComputePbfDensity(Fluid, fluids, pi, i, hash.NumNeighbors[i], neighbors, points, normals);
                    ComputePbfLagrangeMultiplier(Fluid, fluids, pi, i, hash.NumNeighbors[i], neighbors, points,
                        normals);
                }

                for (var i = 0; i < Fluid.NumParticles; i++)
                {
                    var points = fluids[i].Sphere.point;
                    var normals = fluids[i].Sphere.normal;

                    var pi = fluids[i].Predicted;
                    fluids[i].Predicted +=
                        SolveDensityConstraint(Fluid, fluids, pi, i, hash.NumNeighbors[i], neighbors, points, normals);
                }

                iter++;
            }

            hash.IncrementTimeStamp();
        }

        /// <summary>
        /// Calculates current densities for every particle.
        /// </summary>
        private void ComputePbfDensity(FluidBody fluid, IReadOnlyList<Fluid> fluids, Vector3D pi, int i,
            int numNeighbors, int[,] neighbors,
            Dictionary<Vector3D, List<Vector3D>> points, ICollection<Vector3D> normals)
        {
            fluids[i].Densities = fluids[i].ParticleMass * fluid.Kernel.WZero;

            for (var j = 0; j < numNeighbors; j++)
            {
                var neighborIndex = neighbors[i, j];

                var pn = fluids[neighborIndex].Predicted;
                fluids[i].Densities += fluids[i].ParticleMass *
                                       fluid.Kernel.W(pi.x - pn.x, pi.y - pn.y,
                                           pi.z - pn.z);
            }

            foreach (var entry in points)
            {
                foreach (var ve in entry.Value)
                {
                    fluids[i].Densities +=
                        Psi[normals.Count - 1] * fluid.Kernel.W(pi.x - ve.x, pi.y - ve.y, pi.z - ve.z);
                }
            }
        }

        /// <summary>
        /// Computes gradients using Lagrange method.
        /// </summary>
        private void ComputePbfLagrangeMultiplier(FluidBody fluid, IReadOnlyList<Fluid> fluids, Vector3D pi, int i,
            int numNeighbors,
            int[,] neighbors, Dictionary<Vector3D, List<Vector3D>> points, ICollection<Vector3D> normals)
        {
            const double eps = 1.0e-6;
            var invDensity = 1.0 / fluids[i].Density;
            var massMulInvDensity = fluids[i].ParticleMass * invDensity;

            var c = fluids[i].Densities * invDensity - 1.0;
            if (c < 0.0) c = 0.0;

            if (!c.Equals(0.0))
            {
                var sumGradC2 = 0.0;
                var gradCi = Vector3D.Zero;

                for (var j = 0; j < numNeighbors; j++)
                {
                    var neighborIndex = neighbors[i, j];

                    var pn = fluids[neighborIndex].Predicted;
                    var gradW = fluid.Kernel.GradW(pi.x - pn.x, pi.y - pn.y, pi.z - pn.z);

                    Vector3D gradCj;
                    gradCj.x = -massMulInvDensity * gradW.x;
                    gradCj.y = -massMulInvDensity * gradW.y;
                    gradCj.z = -massMulInvDensity * gradW.z;

                    sumGradC2 += gradCj.x * gradCj.x + gradCj.y * gradCj.y + gradCj.z * gradCj.z;

                    gradCi.x -= gradCj.x;
                    gradCi.y -= gradCj.y;
                    gradCi.z -= gradCj.z;
                }

                foreach (var entry in points)
                {
                    foreach (var ve in entry.Value)
                    {
                        var gradW = fluid.Kernel.GradW(pi.x - ve.x, pi.y - ve.y, pi.z - ve.z);

                        var psi = -Psi[normals.Count - 1] * invDensity;

                        Vector3D gradCj;
                        gradCj.x = psi * gradW.x;
                        gradCj.y = psi * gradW.y;
                        gradCj.z = psi * gradW.z;

                        sumGradC2 += gradCj.x * gradCj.x + gradCj.y * gradCj.y + gradCj.z * gradCj.z;

                        gradCi.x -= gradCj.x;
                        gradCi.y -= gradCj.y;
                        gradCi.z -= gradCj.z;
                    }
                }

                sumGradC2 += gradCi.SqrMagnitude;

                fluids[i].Lambda = -c / (sumGradC2 + eps);
            }
            else
            {
                fluids[i].Lambda = 0.0;
            }
        }

        /// <summary>
        /// Calculates density constraint.
        /// </summary>
        /// <returns>Vector position correction</returns>
        private Vector3D SolveDensityConstraint(FluidBody fluid, IReadOnlyList<Fluid> fluids, Vector3D pi, int i,
            int numNeighbors, int[,] neighbors,
            Dictionary<Vector3D, List<Vector3D>> points, ICollection<Vector3D> normals)
        {
            var corr = Vector3D.Zero;
            var invDensity = 1.0 / fluids[i].Density;
            var massMulInvDensity = fluids[i].ParticleMass * invDensity;

            for (var j = 0; j < numNeighbors; j++)
            {
                var neighborIndex = neighbors[i, j];

                var pn = fluids[neighborIndex].Predicted;

                var gradW = fluid.Kernel.GradW(pi.x - pn.x, pi.y - pn.y, pi.z - pn.z);

                var lambda = (fluids[i].Lambda + fluids[neighborIndex].Lambda) * -massMulInvDensity;
                corr.x -= lambda * gradW.x;
                corr.y -= lambda * gradW.y;
                corr.z -= lambda * gradW.z;
            }

            foreach (var entry in points)
            {
                foreach (var ve in entry.Value)
                {
                    var gradW = fluid.Kernel.GradW(pi.x - ve.x, pi.y - ve.y, pi.z - ve.z);

                    var lambda = fluids[i].Lambda * -Psi[normals.Count - 1] * invDensity;
                    corr.x -= lambda * gradW.x;
                    corr.y -= lambda * gradW.y;
                    corr.z -= lambda * gradW.z;
                }
            }

            return corr;
        }
    }
}