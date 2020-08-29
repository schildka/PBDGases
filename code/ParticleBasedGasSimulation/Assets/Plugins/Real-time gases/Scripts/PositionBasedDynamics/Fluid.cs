using Common.LinearAlgebra;
using UnityEngine;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <summary>
    /// This class holds all the information's for one gas particle.
    /// </summary>
    public class Fluid
    {
        public Vector3D Position { get; set; }

        public Vector3D Predicted { get; set; }

        public Vector3D Velocity { get; set; }

        public Vector3D Vorticity { get; set; }

        public double ParticleMass { get; private set; }

        public double Density { get; private set; }

        public double Damping { get; private set; }

        public double Viscosity { get; private set; }

        public float Toxicity { get; private set; }

        public double Lambda { get; set; }

        public double Densities { get; set; }

        public readonly GameObject FluidSphere;

        public readonly Sphere Sphere;

        public Fluid(Vector3D position, Vector3D predicted, Vector3D velocity, Vector3D vorticity, double particleMass,
            double density, double damping, double viscosity, float toxicity, GameObject fluidSphere)
        {
            Position = position;
            Predicted = predicted;
            Velocity = velocity;
            Vorticity = vorticity;
            ParticleMass = particleMass;
            Density = density;
            Damping = damping;
            Viscosity = viscosity;
            Toxicity = toxicity;
            FluidSphere = fluidSphere;
            Sphere = fluidSphere.GetComponent<Sphere>();
        }
    }
}