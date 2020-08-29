using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common.LinearAlgebra;
using UnityEngine;

namespace Common.Shapes
{
    /// <summary>
    /// This class represents a geometrical box shape.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Box3D
    {
        public Vector3D Center => (Min + Max) / 2.0;

        public Vector3D Size => new Vector3D(Width, Height, Depth);

        public double Width => Max.x - Min.x;

        public double Height => Max.y - Min.y;

        public double Depth => Max.z - Min.z;

        public double Area => (Max.x - Min.x) * (Max.y - Min.y) * (Max.z - Min.z);

        public double SurfaceArea
        {
            get
            {
                var d = Max - Min;
                return 2.0 * (d.x * d.y + d.x * d.z + d.y * d.z);
            }
        }

        public Vector3D Min;

        public Vector3D Max;

        public Vector3 Position;

        public Box3D(double min, double max)
        {
            Min = new Vector3D(min);
            Max = new Vector3D(max);
            Position = new Vector3(0, 0, 0);
        }

        public Box3D(Vector3 position, double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
        {
            Position = position;
            Min = new Vector3D(minX, minY, minZ);
            Max = new Vector3D(maxX, maxY, maxZ);
        }

        public Box3D(Vector3 position, Vector3D min, Vector3D max)
        {
            Position = position;
            Min = new Vector3D(min.x + position.x, min.y + position.y, min.z + position.z);
            Max = new Vector3D(max.x + position.x, max.y + position.y, max.z + position.z);
        }

        public Box3D(Vector3D min, Vector3D max)
        {
            Min = min;
            Max = max;
            Position = new Vector3(0, 0, 0);
        }

        public Box3D(Box3D box)
        {
            Min = box.Min;
            Max = box.Max;
            Position = new Vector3(0, 0, 0);
        }

        public Box3D SetBounds(Vector3 position, Vector3 scale)
        {
            Position = position;
            Max = new Vector3D(position.x + (scale.x / 2.0), position.y + (scale.y / 2.0),
                position.z + (scale.z / 2.0));
            Min = new Vector3D(position.x - (scale.x / 2.0), position.y - (scale.y / 2.0),
                position.z - (scale.z / 2.0));

            return this;
        }

        public void GetCorners(IList<Vector3D> corners)
        {
            corners[0] = new Vector3D(Min.x, Min.y, Min.z);
            corners[1] = new Vector3D(Min.x, Min.y, Max.z);
            corners[2] = new Vector3D(Max.x, Min.y, Max.z);
            corners[3] = new Vector3D(Max.x, Min.y, Min.z);

            corners[4] = new Vector3D(Min.x, Max.y, Min.z);
            corners[5] = new Vector3D(Min.x, Max.y, Max.z);
            corners[6] = new Vector3D(Max.x, Max.y, Max.z);
            corners[7] = new Vector3D(Max.x, Max.y, Min.z);
        }

        /// <summary>
        /// Returns the bounding box containing this box and the given point.
        /// </summary>
        public Box3D Enlarge(Vector3D p)
        {
            return new Box3D(Position, Math.Min(Min.x, p.x), Math.Max(Max.x, p.x),
                Math.Min(Min.y, p.y), Math.Max(Max.y, p.y),
                Math.Min(Min.z, p.z), Math.Max(Max.z, p.z));
        }

        /// <summary>
        /// Returns the bounding box containing this box and the given box.
        /// </summary>
        public Box3D Enlarge(Box3D r)
        {
            return new Box3D(Position, Math.Min(Min.x, r.Min.x), Math.Max(Max.x, r.Max.x),
                Math.Min(Min.y, r.Min.y), Math.Max(Max.y, r.Max.y),
                Math.Min(Min.z, r.Min.z), Math.Max(Max.z, r.Max.z));
        }

        /// <summary>
        /// Returns true if this bounding box contains the given bounding box.
        /// </summary>
        public bool Intersects(Box3D a)
        {
            if (Max.x < a.Min.x || Min.x > a.Max.x) return false;
            if (Max.y < a.Min.y || Min.y > a.Max.y) return false;
            return !(Max.z < a.Min.z) && !(Min.z > a.Max.z);
        }

        /// <summary>
        /// Returns true if this bounding box contains the given point.
        /// </summary>
        public bool Contains(Vector3D p)
        {
            if (p.x > Max.x || p.x < Min.x) return false;
            if (p.y > Max.y || p.y < Min.y) return false;
            return !(p.z > Max.z) && !(p.z < Min.z);
        }

        /// <summary>
        /// Returns the closest point to a on the box.
        /// </summary>
        public Vector3D Closest(Vector3D p)
        {
            Vector3D c;

            if (p.x < Min.x)
                c.x = Min.x;
            else if (p.x > Max.x)
                c.x = Max.x;
            else if (p.x - Min.x < Width * 0.5)
                c.x = Min.x;
            else
                c.x = Max.x;

            if (p.y < Min.y)
                c.y = Min.y;
            else if (p.y > Max.y)
                c.y = Max.y;
            else if (p.y - Min.y < Height * 0.5)
                c.y = Min.y;
            else
                c.y = Max.y;

            if (p.z < Min.z)
                c.z = Min.z;
            else if (p.z > Max.z)
                c.z = Max.z;
            else if (p.z - Min.z < Depth * 0.5)
                c.z = Min.z;
            else
                c.z = Max.z;

            return c;
        }

        public bool Intersects(Vector3D p1, Vector3D p2)
        {
            var d = (p2 - p1) * 0.5;
            var e = (Max - Min) * 0.5;
            var c = p1 + d - (Min + Max) * 0.5;
            var ad = d.Absolute;

            if (Math.Abs(c.x) > e.x + ad.x) return false;
            if (Math.Abs(c.y) > e.y + ad.y) return false;
            if (Math.Abs(c.z) > e.z + ad.z) return false;

            const double eps = 1e-12;

            if (Math.Abs(d.y * c.z - d.z * c.y) > e.y * ad.z + e.z * ad.y + eps) return false;
            if (Math.Abs(d.z * c.x - d.x * c.z) > e.z * ad.x + e.x * ad.z + eps) return false;
            return !(Math.Abs(d.x * c.y - d.y * c.x) > e.x * ad.y + e.y * ad.x + eps);
        }
    }
}