using System;
using System.Runtime.InteropServices;

namespace Common.LinearAlgebra
{
    /// <summary>
    /// A 4d double precision vector.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4D
    {
        public double x, y, z, w;

        /// <summary>
        /// The unit x vector.
        /// </summary>
        private static readonly Vector4D UnitX = new Vector4D(1, 0, 0, 0);

        /// <summary>
        /// The unit y vector.
        /// </summary>
        private static readonly Vector4D UnitY = new Vector4D(0, 1, 0, 0);

        /// <summary>
        /// The unit z vector.
        /// </summary>
        private static readonly Vector4D UnitZ = new Vector4D(0, 0, 1, 0);

        /// <summary>
        /// The unit w vector.
        /// </summary>
        private static readonly Vector4D UnitW = new Vector4D(0, 0, 0, 1);

        /// <summary>
        /// A vector of zeros.
        /// </summary>
        private static readonly Vector4D Zero = new Vector4D(0, 0, 0, 0);

        /// <summary>
        /// A vector of ones.
        /// </summary>
        private static readonly Vector4D One = new Vector4D(1, 1, 1, 1);

        /// <summary>
        /// Convert to a 3 dimension vector.
        /// </summary>
        public Vector3D Xyz => new Vector3D(x, y, z);

        /// <summary>
        /// A copy of the vector with w as 0.
        /// </summary>
        public Vector4D Xyz0 => new Vector4D(x, y, z, 0);

        /// <summary>
        /// A vector all with the value v.
        /// </summary>
        public Vector4D(double v)
        {
            this.x = v;
            this.y = v;
            this.z = v;
            this.w = v;
        }

        /// <summary>
        /// A vector from the variables.
        /// </summary>
        public Vector4D(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// A vector from a 3d vector and the w variable.
        /// </summary>
        public Vector4D(Vector3D v, double w)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            this.w = w;
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
                    case 3: return w;
                    default: throw new IndexOutOfRangeException("Vector4d index out of range: " + i);
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
                    case 3:
                        w = value;
                        break;
                    default: throw new IndexOutOfRangeException("Vector4d index out of range: " + i);
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
        private double SqrMagnitude => (x * x + y * y + z * z + w * w);

        /// <summary>
        /// The vector normalized.
        /// </summary>
        public Vector4D Normalized
        {
            get
            {
                var invLength = 1.0 / Math.Sqrt(x * x + y * y + z * z + w * w);
                return new Vector4D(x * invLength, y * invLength, z * invLength, w * invLength);
            }
        }

        /// <summary>
        /// The vectors absolute values.
        /// </summary>
        public Vector4D Absolute => new Vector4D(Math.Abs(x), Math.Abs(y), Math.Abs(z), Math.Abs(w));

        /// <summary>
        /// Add two vectors.
        /// </summary>
        public static Vector4D operator +(Vector4D v1, Vector4D v2)
        {
            return new Vector4D(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w);
        }

        /// <summary>
        /// Add vector and scalar.
        /// </summary>
        public static Vector4D operator +(Vector4D v1, double s)
        {
            return new Vector4D(v1.x + s, v1.y + s, v1.z + s, v1.w + s);
        }

        /// <summary>
        /// Subtract two vectors.
        /// </summary>
        public static Vector4D operator -(Vector4D v1, Vector4D v2)
        {
            return new Vector4D(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w);
        }

        /// <summary>
        /// Subtract vector and scalar.
        /// </summary>
        public static Vector4D operator -(Vector4D v1, double s)
        {
            return new Vector4D(v1.x - s, v1.y - s, v1.z - s, v1.w - s);
        }

        /// <summary>
        /// Multiply two vectors.
        /// </summary>
        public static Vector4D operator *(Vector4D v1, Vector4D v2)
        {
            return new Vector4D(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z, v1.w * v2.w);
        }

        /// <summary>
        /// Multiply a vector and a scalar.
        /// </summary>
        public static Vector4D operator *(Vector4D v, double s)
        {
            return new Vector4D(v.x * s, v.y * s, v.z * s, v.w * s);
        }

        /// <summary>
        /// Multiply a vector and a scalar.
        /// </summary>
        public static Vector4D operator *(double s, Vector4D v)
        {
            return new Vector4D(v.x * s, v.y * s, v.z * s, v.w * s);
        }

        /// <summary>
        /// Divide two vectors.
        /// </summary>
        public static Vector4D operator /(Vector4D v1, Vector4D v2)
        {
            return new Vector4D(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z, v1.w / v2.w);
        }

        /// <summary>
        /// Divide a vector and a scalar.
        /// </summary>
        public static Vector4D operator /(Vector4D v, double s)
        {
            return new Vector4D(v.x / s, v.y / s, v.z / s, v.w / s);
        }

        /// <summary>
        /// Are these vectors equal.
        /// </summary>
        public static bool operator ==(Vector4D v1, Vector4D v2)
        {
            return (v1.x == v2.x && v1.y == v2.y && v1.z == v2.z && v1.w == v2.w);
        }

        /// <summary>
        /// Are these vectors not equal.
        /// </summary>
        public static bool operator !=(Vector4D v1, Vector4D v2)
        {
            return (v1.x != v2.x || v1.y != v2.y || v1.z != v2.z || v1.w != v2.w);
        }

        /// <summary>
        /// Are these vectors equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector4D)) return false;

            var v = (Vector4D) obj;

            return this == v;
        }

        /// <summary>
        /// Are these vectors equal given the error.
        /// </summary>
        public bool EqualsWithError(Vector4D v, double eps)
        {
            if (Math.Abs(x - v.x) > eps) return false;
            if (Math.Abs(y - v.y) > eps) return false;
            if (Math.Abs(z - v.z) > eps) return false;
            return !(Math.Abs(w - v.w) > eps);
        }

        /// <summary>
        /// Are these vectors equal.
        /// </summary>
        public bool Equals(Vector4D v)
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
            hashcode = (hashcode * 37) + w;

            return (int) hashcode;
        }

        /// <summary>
        /// Vector as a string.
        /// </summary>
        public override string ToString()
        {
            return x + "," + y + "," + z + "," + w;
        }

        /// <summary>
        /// Vector from a string.
        /// </summary>
        public static Vector4D FromString(string s)
        {
            var v = new Vector4D();
            try
            {
                var separators = new string[] {","};
                var result = s.Split(separators, StringSplitOptions.None);

                v.x = double.Parse(result[0]);
                v.y = double.Parse(result[1]);
                v.z = double.Parse(result[2]);
                v.w = double.Parse(result[3]);
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
        public static double Dot(Vector4D v0, Vector4D v1)
        {
            return (v0.x * v1.x + v0.y * v1.y + v0.z * v1.z + v0.w * v1.w);
        }

        /// <summary>
        /// Distance between two vectors.
        /// </summary>
        public static double Distance(Vector4D v0, Vector4D v1)
        {
            return Math.Sqrt(SqrDistance(v0, v1));
        }

        /// <summary>
        /// Square distance between two vectors.
        /// </summary>
        private static double SqrDistance(Vector4D v0, Vector4D v1)
        {
            var x = v0.x - v1.x;
            var y = v0.y - v1.y;
            var z = v0.z - v1.z;
            var w = v0.w - v1.w;
            return x * x + y * y + z * z + w * w;
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        public void Normalize()
        {
            var invLength = 1.0 / Math.Sqrt(x * x + y * y + z * z + w * w);
            x *= invLength;
            y *= invLength;
            z *= invLength;
            w *= invLength;
        }

        /// <summary>
        /// The minimum value between s and each component in vector.
        /// </summary>
        public void Min(double s)
        {
            x = Math.Min(x, s);
            y = Math.Min(y, s);
            z = Math.Min(z, s);
            w = Math.Min(w, s);
        }

        /// <summary>
        /// The minimum value between each component in vectors.
        /// </summary>
        public void Min(Vector4D v)
        {
            x = Math.Min(x, v.x);
            y = Math.Min(y, v.y);
            z = Math.Min(z, v.z);
            w = Math.Min(w, v.w);
        }

        /// <summary>
        /// The maximum value between s and each component in vector.
        /// </summary>
        public void Max(double s)
        {
            x = Math.Max(x, s);
            y = Math.Max(y, s);
            z = Math.Max(z, s);
            w = Math.Max(w, s);
        }

        /// <summary>
        /// The maximum value between each component in vectors.
        /// </summary>
        public void Max(Vector4D v)
        {
            x = Math.Max(x, v.x);
            y = Math.Max(y, v.y);
            z = Math.Max(z, v.z);
            w = Math.Max(w, v.w);
        }

        /// <summary>
        /// The absolute vector.
        /// </summary>
        public void Abs()
        {
            x = Math.Abs(x);
            y = Math.Abs(y);
            z = Math.Abs(z);
            w = Math.Abs(w);
        }

        /// <summary>
        /// Clamp the each component to specified min and max.
        /// </summary>
        public void Clamp(double min, double max)
        {
            x = Math.Max(Math.Min(x, max), min);
            y = Math.Max(Math.Min(y, max), min);
            z = Math.Max(Math.Min(z, max), min);
            w = Math.Max(Math.Min(w, max), min);
        }

        /// <summary>
        /// Clamp the each component to specified min and max.
        /// </summary>
        public void Clamp(Vector4D min, Vector4D max)
        {
            x = Math.Max(Math.Min(x, max.x), min.x);
            y = Math.Max(Math.Min(y, max.y), min.y);
            z = Math.Max(Math.Min(z, max.z), min.z);
            w = Math.Max(Math.Min(w, max.w), min.w);
        }

        /// <summary>
        /// Lerp between two vectors.
        /// </summary>
        public static Vector4D Lerp(Vector4D v1, Vector4D v2, double a)
        {
            var a1 = 1.0 - a;
            var v = new Vector4D
            {
                x = v1.x * a1 + v2.x * a,
                y = v1.y * a1 + v2.y * a,
                z = v1.z * a1 + v2.z * a,
                w = v1.w * a1 + v2.w * a
            };
            return v;
        }
    }
}