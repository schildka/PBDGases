using System;
using System.Collections.Generic;
using Common.LinearAlgebra;
using Common.Shapes;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <summary>
    /// Bounds which calculates amount of particles that fits inside of the bound.
    /// </summary>
    [Serializable]
    public class ParticleBounds
    {
        public int NumParticles => positions.Count;

        public List<Vector3D> positions;

        public Box3D bounds;

        public Sphere3D sphereBounds;

        public double spacing;

        private double Diameter => spacing * 2.0;

        private double Volume => (4.0 / 3.0) * Math.PI * Math.Pow(sphereBounds.Radius, 3);


        public ParticleBounds()
        {
        }

        public ParticleBounds(double spacing, Box3D bounds)
        {
            this.spacing = spacing;
            this.bounds = bounds;
            CreateParticles();
        }

        public ParticleBounds(double spacing, Sphere3D bounds)
        {
            this.spacing = spacing;
            sphereBounds = bounds;
            CreateParticlesInSphere();
        }

        /// <summary>
        /// Calculates how many particles fit into a spherical bound.
        /// </summary>
        public void CreateParticlesInSphere()
        {
            var volume = (4.0 / 3.0) * Math.PI * Math.Pow(spacing, 3);

            positions = new List<Vector3D>();

            while (sphereBounds.Radius >= 0)
            {
                var anz = (int) (Volume / volume);

                var a = (4.0 * Math.PI * Math.Pow(sphereBounds.Radius, 2)) / anz;
                var d = Math.Sqrt(a);
                var m = (int) (Math.PI / d);

                var dO = Math.PI / m;
                var dP = a / dO;

                for (var i = 0; i < m; i++)
                {
                    var v = Math.PI * (i + 0.5) / m;
                    var mP = (int) ((2.0 * Math.PI * Math.Sin(v)) / dP);

                    for (var j = 0; j < mP; j++)
                    {
                        var p = (2.0 * Math.PI * j) / mP;

                        var pos = new Vector3D
                        {
                            x = sphereBounds.Radius * Math.Sin(v) * Math.Cos(p) + sphereBounds.Position.x,
                            y = sphereBounds.Radius * Math.Sin(v) * Math.Sin(p) + sphereBounds.Position.y,
                            z = sphereBounds.Radius * Math.Cos(v) + sphereBounds.Position.z
                        };

                        positions.Add(pos);
                    }
                }

                sphereBounds.Radius = sphereBounds.Radius - spacing * 2.5;
            }

            positions.Add(new Vector3D(sphereBounds.Position.x, sphereBounds.Position.y, sphereBounds.Position.z));
        }

        /// <summary>
        /// Calculates amount of particles that fit into a box bound.
        /// </summary>
        public void CreateParticles()
        {
            var numX = (int) (bounds.Width / Diameter);
            var numY = (int) (bounds.Height / Diameter);
            var numZ = (int) (bounds.Depth / Diameter);

            positions = new List<Vector3D>(numX * numY * numZ);

            for (var z = 0; z < numZ; z++)
            {
                for (var y = 0; y < numY; y++)
                {
                    for (var x = 0; x < numX; x++)
                    {
                        var pos = new Vector3D
                        {
                            x = Diameter * x + bounds.Min.x + spacing,
                            y = Diameter * y + bounds.Min.y + spacing,
                            z = Diameter * z + bounds.Min.z + spacing
                        };

                        positions.Add(pos);
                    }
                }
            }
        }
    }
}