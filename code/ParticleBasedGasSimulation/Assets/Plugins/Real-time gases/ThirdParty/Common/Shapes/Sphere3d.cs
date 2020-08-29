using System;
using System.Runtime.InteropServices;
using Common.LinearAlgebra;
using UnityEngine;

namespace Common.Shapes
{
    /// <summary>
    /// This class represents a geometrical sphere shape.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Sphere3D
    {
        public double SurfaceArea => 4.0 * Math.PI * Radius;

        public Vector3 Position;

        public double Radius;

        public Sphere3D(Vector3 position, double radius)
        {
            Position = position;
            Radius = radius;
        }

        public Sphere3D SetBounds(Vector3 position, double radius)
        {
            Position = position;
            Radius = radius;

            return this;
        }

        /// <summary>
        /// Checks if a point is inside the sphere.
        /// </summary>
        /// <returns>bool according to whether the sphere contains the point</returns>
        public bool Contains(Vector3D p)
        {
            return Math.Pow(p.x - Position.x, 2) + Math.Pow(p.y - Position.y, 2) + Math.Pow(p.z - Position.z, 2) <=
                   Radius * Radius;
        }
    }
}