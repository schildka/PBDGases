using System;
using Common.LinearAlgebra;
using UnityEngine;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <summary>
    /// This class holds the particle properties, which are editable in the graphical-user-interface.
    /// </summary>
    [Serializable]
    public class FluidCreation
    {
        public GameObject gameObject;
        public Vector3D position;
        public ParticleBounds bounds;
        public double density;
        public double mass;
        public double damping;
        public double viscosity;
        public float toxicity;

        public FluidCreation(GameObject gameObject, Vector3D position, double density, double mass, double damping,
            double viscosity, float toxicity)
        {
            this.gameObject = gameObject;
            this.position = position;
            this.density = density;
            this.mass = mass;
            this.damping = damping;
            this.viscosity = viscosity;
            this.toxicity = toxicity;
        }

        public FluidCreation(ParticleBounds bounds, double density, double mass, double damping, double viscosity,
            float toxicity)
        {
            this.bounds = bounds;
            this.density = density;
            this.mass = mass;
            this.damping = damping;
            this.viscosity = viscosity;
            this.toxicity = toxicity;
        }
    }
}