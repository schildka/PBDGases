using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Common.LinearAlgebra
{
    /// <summary>
    /// A 3d double precision vector.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Vector3D
    {
        public double x, y, z;

        /// <summary>
        /// The unit x vector.
        /// </summary>
        private static readonly Vector3D UnitX = new Vector3D(1, 0, 0);

        /// <summary>
        /// The unit y vector.
        /// </summary>
        private static readonly Vector3D UnitY = new Vector3D(0, 1, 0);

        /// <summary>
        /// The unit z vector.
        /// </summary>
        private static readonly Vector3D UnitZ = new Vector3D(0, 0, 1);

        /// <summary>
        /// A vector of zeros.
        /// </summary>
        public static readonly Vector3D Zero = new Vector3D(0, 0, 0);

        /// <summary>
        /// A vector of ones.
        /// </summary>
        private static readonly Vector3D One = new Vector3D(1, 1, 1);

        /// <summary>
        /// Convert to a 4 dimension vector.
        /// </summary>
        public Vector4D Xyz0 => new Vector4D(x, y, z, 0);

        /// <summary>
        /// Convert to a 4 dimension vector.
        /// </summary>
        public Vector4D Xyz1 => new Vector4D(x, y, z, 1);

        /// <summary>
        /// A vector all with the value v.
        /// </summary>
        public Vector3D(double v)
        {
            this.x = v;
            this.y = v;
            this.z = v;
        }

        /// <summary>
        /// A vector from the variables.
        /// </summary>
        public Vector3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default: throw new IndexOutOfRangeException("Vector3d index out of range: " + i);
                }
            }
            set
            {
                switch (i)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default: throw new IndexOutOfRangeException("Vector3d index out of range: " + i);
                }
            }
        }

        /// <summary>
        /// The length of the vector.
        /// </summary>
        public double Magnitude => Math.Sqrt(SqrMagnitude);

        /// <summary>
        /// The length of the vector squared.
        /// </summary>
        public double SqrMagnitude => (x * x + y * y + z * z);

        public Vector3 ToVec3()
        {
            return new Vector3((float) x, (float) y, (float) z);
        }

        /// <summary>
        /// The vector normalized.
        /// </summary>
        public Vector3D Normalized
        {
            get
            {
                var invLength = 1.0 / Math.Sqrt(x * x + y * y + z * z);
                return new Vector3D(x * invLength, y * invLength, z * invLength);
            }
        }

        /// <summary>
        /// The vectors absolute values.
        /// </summary>
        public Vector3D Absolute => new Vector3D(Math.Abs(x), Math.Abs(y), Math.Abs(z));

        /// <summary>
        /// Add two vectors.
        /// </summary>
        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        /// <summary>
        /// Add vector and scalar.
        /// </summary>
        public static Vector3D operator +(Vector3D v1, double s)
        {
            return new Vector3D(v1.x + s, v1.y + s, v1.z + s);
        }

        /// <summary>
        /// Subtract two vectors.
        /// </summary>
        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        /// <summary>
        /// Subtract vector and scalar.
        /// </summary>
        public static Vector3D operator -(Vector3D v1, double s)
        {
            return new Vector3D(v1.x - s, v1.y - s, v1.z - s);
        }

        /// <summary>
        /// Multiply two vectors.
        /// </summary>
        public static Vector3D operator *(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        /// <summary>
        /// Multiply a vector and a scalar.
        /// </summary>
        public static Vector3D operator *(Vector3D v, double s)
        {
            return new Vector3D(v.x * s, v.y * s, v.z * s);
        }

        /// <summary>
        /// Multiply a vector and a scalar.
        /// </summary>
        public static Vector3D operator *(double s, Vector3D v)
        {
            return new Vector3D(v.x * s, v.y * s, v.z * s);
        }

        /// <summary>
        /// Divide two vectors.
        /// </summary>
        public static Vector3D operator /(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        }

        /// <summary>
        /// Divide a vector and a scalar.
        /// </summary>
        public static Vector3D operator /(Vector3D v, double s)
        {
            return new Vector3D(v.x / s, v.y / s, v.z / s);
        }

        /// <summary>
        /// Are these vectors equal.
        /// </summary>
        public static bool operator ==(Vector3D v1, Vector3D v2)
        {
            return (v1.x == v2.x && v1.y == v2.y && v1.z == v2.z);
        }

        /// <summary>
        /// Are these vectors not equal.
        /// </summary>
        public static bool operator !=(Vector3D v1, Vector3D v2)
        {
            return (v1.x != v2.x || v1.y != v2.y || v1.z != v2.z);
        }

        /// <summary>
        /// Are these vectors equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector3D)) return false;

            var v = (Vector3D) obj;

            return this == v;
        }

        /// <summary>
        /// Are these vectors equal given the error.
        /// </summary>
        public bool EqualsWithError(Vector3D v, double eps)
        {
            if (Math.Abs(x - v.x) > eps) return false;
            if (Math.Abs(y - v.y) > eps) return false;
            return !(Math.Abs(z - v.z) > eps);
        }

        /// <summary>
        /// Are these vectors equal.
        /// </summary>
        public bool Equals(Vector3D v)
        {
            return this == v;
        }

        /// <summary>
        /// Vectors hash code. 
        /// </summary>
        public override int GetHashCode()
        {
            double hashcode = 23;
            hashcode = (hashcode * 37) + x;
            hashcode = (hashcode * 37) + y;
            hashcode = (hashcode * 37) + z;

            return (int) hashcode;
        }

        /// <summary>
        /// Vector as a string.
        /// </summary>
        public override string ToString()
        {
            return x + "," + y + "," + z;
        }

        /// <summary>
        /// Vector from a string.
        /// </summary>
        public static Vector3D FromString(string s)
        {
            var v = new Vector3D();

            try
            {
                var separators = new string[] {","};
                var result = s.Split(separators, StringSplitOptions.None);

                v.x = double.Parse(result[0]);
                v.y = double.Parse(result[1]);
                v.z = double.Parse(result[2]);
            }
            catch
            {
                // ignored
            }

            return v;
        }

        /// <summary>
        /// The dot product of two vectors.
        /// </summary>
        public static double Dot(Vector3D v0, Vector3D v1)
        {
            return (v0.x * v1.x + v0.y * v1.y + v0.z * v1.z);
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        public void Normalize()
        {
            var invLength = 1.0 / Math.Sqrt(x * x + y * y + z * z);
            x *= invLength;
            y *= invLength;
            z *= invLength;
        }

        /// <summary>
        /// Cross two vectors.
        /// </summary>
        public Vector3D Cross(Vector3D v)
        {
            return new Vector3D(y * v.z - z * v.y, z * v.x - x * v.z, x * v.y - y * v.x);
        }

        /// <summary>
        /// Cross two vectors.
        /// </summary>
        public static Vector3D Cross(Vector3D v0, Vector3D v1)
        {
            return new Vector3D(v0.y * v1.z - v0.z * v1.y, v0.z * v1.x - v0.x * v1.z, v0.x * v1.y - v0.y * v1.x);
        }

        /// <summary>
        /// Distance between two vectors.
        /// </summary>
        public static double Distance(Vector3D v0, Vector3D v1)
        {
            return Math.Sqrt(SqrDistance(v0, v1));
        }

        /// <summary>
        /// Square distance between two vectors.
        /// </summary>
        private static double SqrDistance(Vector3D v0, Vector3D v1)
        {
            var x = v0.x - v1.x;
            var y = v0.y - v1.y;
            var z = v0.z - v1.z;
            return x * x + y * y + z * z;
        }

        /// <summary>
        /// The minimum value between s and each component in vector.
        /// </summary>
        public void Min(double s)
        {
            x = Math.Min(x, s);
            y = Math.Min(y, s);
            z = Math.Min(z, s);
        }

        /// <summary>
        /// The minimum value between each component in vectors.
        /// </summary>
        public void Min(Vector3D v)
        {
            x = Math.Min(x, v.x);
            y = Math.Min(y, v.y);
            z = Math.Min(z, v.z);
        }

        /// <summary>
        /// The maximum value between s and each component in vector.
        /// </summary>
        public void Max(double s)
        {
            x = Math.Max(x, s);
            y = Math.Max(y, s);
            z = Math.Max(z, s);
        }

        /// <summary>
        /// The maximum value between each component in vectors.
        /// </summary>
        public void Max(Vector3D v)
        {
            x = Math.Max(x, v.x);
            y = Math.Max(y, v.y);
            z = Math.Max(z, v.z);
        }

        /// <summary>
        /// The absolute vector.
        /// </summary>
        public void Abs()
        {
            x = Math.Abs(x);
            y = Math.Abs(y);
            z = Math.Abs(z);
        }

        /// <summary>
        /// Clamp the each component to specified min and max.
        /// </summary>
        public void Clamp(double min, double max)
        {
            x = Math.Max(Math.Min(x, max), min);
            y = Math.Max(Math.Min(y, max), min);
            z = Math.Max(Math.Min(z, max), min);
        }

        /// <summary>
        /// Clamp the each component to specified min and max.
        /// </summary>
        public void Clamp(Vector3D min, Vector3D max)
        {
            x = Math.Max(Math.Min(x, max.x), min.x);
            y = Math.Max(Math.Min(y, max.y), min.y);
            z = Math.Max(Math.Min(z, max.z), min.z);
        }
    }
}