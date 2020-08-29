using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Common.LinearAlgebra
{
    /// <summary>
    /// A single precision 3 dimension matrix
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3X3D
    {
        /// <summary>
        /// The matrix
        /// </summary>
        public double m00, m01, m02;

        public double m10, m11, m12;
        public double m20, m21, m22;

        /// <summary>
        /// The Matrix Identity.
        /// </summary>
        private static readonly Matrix3X3D Identity = new Matrix3X3D(1, 0, 0, 0, 1, 0, 0, 0, 1);

        /// <summary>
        /// A matrix from the following variables.
        /// </summary>
        public Matrix3X3D(double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;
            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;
            this.m20 = m20;
            this.m21 = m21;
            this.m22 = m22;
        }

        /// <summary>
        /// A matrix from the following variables.
        /// </summary>
        public Matrix3X3D(double v)
        {
            m00 = v;
            m01 = v;
            m02 = v;
            m10 = v;
            m11 = v;
            m12 = v;
            m20 = v;
            m21 = v;
            m22 = v;
        }

        /// <summary>
        /// A matrix copied from a array of variables.
        /// </summary>
        public Matrix3X3D(double[,] m)
        {
            m00 = m[0, 0];
            m01 = m[0, 1];
            m02 = m[0, 2];
            m10 = m[1, 0];
            m11 = m[1, 1];
            m12 = m[1, 2];
            m20 = m[2, 0];
            m21 = m[2, 1];
            m22 = m[2, 2];
        }

        /// <summary>
        /// A matrix copied from a array of variables.
        /// </summary>
        public Matrix3X3D(IReadOnlyList<double> m)
        {
            m00 = m[0 + 0 * 3];
            m01 = m[0 + 1 * 3];
            m02 = m[0 + 2 * 3];
            m10 = m[1 + 0 * 3];
            m11 = m[1 + 1 * 3];
            m12 = m[1 + 2 * 3];
            m20 = m[2 + 0 * 3];
            m21 = m[2 + 1 * 3];
            m22 = m[2 + 2 * 3];
        }

        /// <summary>
        /// A matrix copied from the matrix m.
        /// </summary>
        public Matrix3X3D(Matrix3X3D m)
        {
            m00 = m.m00;
            m01 = m.m01;
            m02 = m.m02;
            m10 = m.m10;
            m11 = m.m11;
            m12 = m.m12;
            m20 = m.m20;
            m21 = m.m21;
            m22 = m.m22;
        }

        /// <summary>
        /// Access the variable at index i
        /// </summary>
        private double this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return m00;
                    case 1: return m10;
                    case 2: return m20;
                    case 3: return m01;
                    case 4: return m11;
                    case 5: return m21;
                    case 6: return m02;
                    case 7: return m12;
                    case 8: return m22;
                    default: throw new IndexOutOfRangeException("Matrix3x3d index out of range: " + i);
                }
            }
            set
            {
                switch (i)
                {
                    case 0:
                        m00 = value;
                        break;
                    case 1:
                        m10 = value;
                        break;
                    case 2:
                        m20 = value;
                        break;
                    case 3:
                        m01 = value;
                        break;
                    case 4:
                        m11 = value;
                        break;
                    case 5:
                        m21 = value;
                        break;
                    case 6:
                        m02 = value;
                        break;
                    case 7:
                        m12 = value;
                        break;
                    case 8:
                        m22 = value;
                        break;
                    default: throw new IndexOutOfRangeException("Matrix3x3d index out of range: " + i);
                }
            }
        }

        /// <summary>
        /// Access the variable at index ij
        /// </summary>
        private double this[int i, int j]
        {
            get
            {
                var k = i + j * 3;
                switch (k)
                {
                    case 0: return m00;
                    case 1: return m10;
                    case 2: return m20;
                    case 3: return m01;
                    case 4: return m11;
                    case 5: return m21;
                    case 6: return m02;
                    case 7: return m12;
                    case 8: return m22;
                    default: throw new IndexOutOfRangeException("Matrix3x3d index out of range: " + k);
                }
            }
            set
            {
                var k = i + j * 3;
                switch (k)
                {
                    case 0:
                        m00 = value;
                        break;
                    case 1:
                        m10 = value;
                        break;
                    case 2:
                        m20 = value;
                        break;
                    case 3:
                        m01 = value;
                        break;
                    case 4:
                        m11 = value;
                        break;
                    case 5:
                        m21 = value;
                        break;
                    case 6:
                        m02 = value;
                        break;
                    case 7:
                        m12 = value;
                        break;
                    case 8:
                        m22 = value;
                        break;
                    default: throw new IndexOutOfRangeException("Matrix3x3d index out of range: " + k);
                }
            }
        }

        /// <summary>
        /// The transpose of the matrix. The rows and columns are flipped.
        /// </summary>
        public Matrix3X3D Transpose
        {
            get
            {
                var transpose = new Matrix3X3D
                {
                    m00 = m00,
                    m01 = m10,
                    m02 = m20,
                    m10 = m01,
                    m11 = m11,
                    m12 = m21,
                    m20 = m02,
                    m21 = m12,
                    m22 = m22
                };


                return transpose;
            }
        }

        /// <summary>
        /// The determinate of a matrix. 
        /// </summary>
        public double Determinant
        {
            get
            {
                var cofactor00 = m11 * m22 - m12 * m21;
                var cofactor10 = m12 * m20 - m10 * m22;
                var cofactor20 = m10 * m21 - m11 * m20;

                var det = m00 * cofactor00 + m01 * cofactor10 + m02 * cofactor20;

                return det;
            }
        }

        /// <summary>
        /// The inverse of the matrix.
        /// A matrix multiplied by its inverse is the identity.
        /// </summary>
        public Matrix3X3D Inverse
        {
            get
            {
                Matrix3X3D inverse;
                TryInverse(out inverse);
                return inverse;
            }
        }

        public double Trace => m00 + m11 + m22;

        /// <summary>
        /// Add two matrices.
        /// </summary>
        public static Matrix3X3D operator +(Matrix3X3D m1, Matrix3X3D m2)
        {
            var kSum = new Matrix3X3D
            {
                m00 = m1.m00 + m2.m00,
                m01 = m1.m01 + m2.m01,
                m02 = m1.m02 + m2.m02,
                m10 = m1.m10 + m2.m10,
                m11 = m1.m11 + m2.m11,
                m12 = m1.m12 + m2.m12,
                m20 = m1.m20 + m2.m20,
                m21 = m1.m21 + m2.m21,
                m22 = m1.m22 + m2.m22
            };


            return kSum;
        }

        /// <summary>
        /// Subtract two matrices.
        /// </summary>
        public static Matrix3X3D operator -(Matrix3X3D m1, Matrix3X3D m2)
        {
            var kSum = new Matrix3X3D
            {
                m00 = m1.m00 - m2.m00,
                m01 = m1.m01 - m2.m01,
                m02 = m1.m02 - m2.m02,
                m10 = m1.m10 - m2.m10,
                m11 = m1.m11 - m2.m11,
                m12 = m1.m12 - m2.m12,
                m20 = m1.m20 - m2.m20,
                m21 = m1.m21 - m2.m21,
                m22 = m1.m22 - m2.m22
            };


            return kSum;
        }

        /// <summary>
        /// Multiply two matrices.
        /// </summary>
        public static Matrix3X3D operator *(Matrix3X3D m1, Matrix3X3D m2)
        {
            var kProd = new Matrix3X3D
            {
                m00 = m1.m00 * m2.m00 + m1.m01 * m2.m10 + m1.m02 * m2.m20,
                m01 = m1.m00 * m2.m01 + m1.m01 * m2.m11 + m1.m02 * m2.m21,
                m02 = m1.m00 * m2.m02 + m1.m01 * m2.m12 + m1.m02 * m2.m22,
                m10 = m1.m10 * m2.m00 + m1.m11 * m2.m10 + m1.m12 * m2.m20,
                m11 = m1.m10 * m2.m01 + m1.m11 * m2.m11 + m1.m12 * m2.m21,
                m12 = m1.m10 * m2.m02 + m1.m11 * m2.m12 + m1.m12 * m2.m22,
                m20 = m1.m20 * m2.m00 + m1.m21 * m2.m10 + m1.m22 * m2.m20,
                m21 = m1.m20 * m2.m01 + m1.m21 * m2.m11 + m1.m22 * m2.m21,
                m22 = m1.m20 * m2.m02 + m1.m21 * m2.m12 + m1.m22 * m2.m22
            };


            return kProd;
        }

        /// <summary>
        /// Multiply  a vector by a matrix.
        /// </summary>
        public static Vector3D operator *(Matrix3X3D m, Vector3D v)
        {
            var kProd = new Vector3D
            {
                x = m.m00 * v.x + m.m01 * v.y + m.m02 * v.z,
                y = m.m10 * v.x + m.m11 * v.y + m.m12 * v.z,
                z = m.m20 * v.x + m.m21 * v.y + m.m22 * v.z
            };


            return kProd;
        }

        /// <summary>
        /// Multiply a matrix by a scalar.
        /// </summary>
        public static Matrix3X3D operator *(Matrix3X3D m1, double s)
        {
            var kProd = new Matrix3X3D
            {
                m00 = m1.m00 * s,
                m01 = m1.m01 * s,
                m02 = m1.m02 * s,
                m10 = m1.m10 * s,
                m11 = m1.m11 * s,
                m12 = m1.m12 * s,
                m20 = m1.m20 * s,
                m21 = m1.m21 * s,
                m22 = m1.m22 * s
            };


            return kProd;
        }

        /// <summary>
        /// Are these matrices equal.
        /// </summary>
        public static bool operator ==(Matrix3X3D m1, Matrix3X3D m2)
        {
            if (m1.m00 != m2.m00) return false;
            if (m1.m01 != m2.m01) return false;
            if (m1.m02 != m2.m02) return false;

            if (m1.m10 != m2.m10) return false;
            if (m1.m11 != m2.m11) return false;
            if (m1.m12 != m2.m12) return false;

            if (m1.m20 != m2.m20) return false;
            if (m1.m21 != m2.m21) return false;
            return !(m1.m22 != m2.m22);
        }

        /// <summary>
        /// Are these matrices not equal.
        /// </summary>
        public static bool operator !=(Matrix3X3D m1, Matrix3X3D m2)
        {
            if (m1.m00 != m2.m00) return true;
            if (m1.m01 != m2.m01) return true;
            if (m1.m02 != m2.m02) return true;

            if (m1.m10 != m2.m10) return true;
            if (m1.m11 != m2.m11) return true;
            if (m1.m12 != m2.m12) return true;

            if (m1.m20 != m2.m20) return true;
            if (m1.m21 != m2.m21) return true;
            return m1.m22 != m2.m22;
        }

        /// <summary>
        /// Are these matrices equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Matrix3X3D)) return false;

            var mat = (Matrix3X3D) obj;

            return this == mat;
        }

        /// <summary>
        /// Are these matrices equal.
        /// </summary>
        public bool Equals(Matrix3X3D mat)
        {
            return this == mat;
        }

        /// <summary>
        /// Are these matrices equal.
        /// </summary>
        public bool EqualsWithError(Matrix3X3D m, double eps)
        {
            if (Math.Abs(m00 - m.m00) > eps) return false;
            if (Math.Abs(m10 - m.m10) > eps) return false;
            if (Math.Abs(m20 - m.m20) > eps) return false;

            if (Math.Abs(m01 - m.m01) > eps) return false;
            if (Math.Abs(m11 - m.m11) > eps) return false;
            if (Math.Abs(m21 - m.m21) > eps) return false;

            if (Math.Abs(m02 - m.m02) > eps) return false;
            if (Math.Abs(m12 - m.m12) > eps) return false;
            return !(Math.Abs(m22 - m.m22) > eps);
        }

        /// <summary>
        /// Matrices hash code. 
        /// </summary>
        public override int GetHashCode()
        {
            var hash = 0;

            for (var i = 0; i < 9; i++)
                hash ^= this[i].GetHashCode();

            return hash;
        }

        /// <summary>
        /// A matrix as a string.
        /// </summary>
        public override string ToString()
        {
            return this[0, 0] + "," + this[0, 1] + "," + this[0, 2] + "\n" +
                   this[1, 0] + "," + this[1, 1] + "," + this[1, 2] + "\n" +
                   this[2, 0] + "," + this[2, 1] + "," + this[2, 2];
        }

        /// <summary>
        /// The Inverse of the matrix copied into mInv.
        /// Returns false if the matrix has no inverse.
        /// A matrix multiplied by its inverse is the identity.
        /// </summary>
        private bool TryInverse(out Matrix3X3D mInv)
        {
            // Invert a 3x3 using cofactors.  This is about 8 times faster than
            // the Numerical Recipes code which uses Gaussian elimination.
            mInv.m00 = m11 * m22 - m12 * m21;
            mInv.m01 = m02 * m21 - m01 * m22;
            mInv.m02 = m01 * m12 - m02 * m11;
            mInv.m10 = m12 * m20 - m10 * m22;
            mInv.m11 = m00 * m22 - m02 * m20;
            mInv.m12 = m02 * m10 - m00 * m12;
            mInv.m20 = m10 * m21 - m11 * m20;
            mInv.m21 = m01 * m20 - m00 * m21;
            mInv.m22 = m00 * m11 - m01 * m10;

            var fDet = m00 * mInv.m00 + m01 * mInv.m10 + m02 * mInv.m20;

            if (Math.Abs(fDet) <= 1e-09)
            {
                mInv = Identity;
                return false;
            }

            var fInvDet = 1.0f / fDet;

            mInv.m00 *= fInvDet;
            mInv.m01 *= fInvDet;
            mInv.m02 *= fInvDet;
            mInv.m10 *= fInvDet;
            mInv.m11 *= fInvDet;
            mInv.m12 *= fInvDet;
            mInv.m20 *= fInvDet;
            mInv.m21 *= fInvDet;
            mInv.m22 *= fInvDet;

            return true;
        }

        /// <summary>
        /// Get the ith column as a vector.
        /// </summary>
        public Vector3D GetColumn(int iCol)
        {
            return new Vector3D(this[0, iCol], this[1, iCol], this[2, iCol]);
        }

        /// <summary>
        /// Set the ith column from avector.
        /// </summary>
        public void SetColumn(int iCol, Vector3D v)
        {
            this[0, iCol] = v.x;
            this[1, iCol] = v.y;
            this[2, iCol] = v.z;
        }

        /// <summary>
        /// Get the ith row as a vector.
        /// </summary>
        public Vector3D GetRow(int iRow)
        {
            return new Vector3D(this[iRow, 0], this[iRow, 1], this[iRow, 2]);
        }

        /// <summary>
        /// Set the ith row from avector.
        /// </summary>
        public void SetRow(int iRow, Vector3D v)
        {
            this[iRow, 0] = v.x;
            this[iRow, 1] = v.y;
            this[iRow, 2] = v.z;
        }

        /// <summary>
        /// Create a scale out of a vector.
        /// </summary>
        public static Matrix3X3D Scale(double s)
        {
            return new Matrix3X3D(s, 0, 0,
                0, s, 0,
                0, 0, 1);
        }

        /// <summary>
        /// Create a rotation out of a angle.
        /// </summary>
        public static Matrix3X3D RotateX(double angle)
        {
            var ca = Math.Cos(angle * Math.PI / 180.0);
            var sa = Math.Sin(angle * Math.PI / 180.0);

            return new Matrix3X3D(1, 0, 0,
                0, ca, -sa,
                0, sa, ca);
        }

        /// <summary>
        /// Create a rotation out of a angle.
        /// </summary>
        public static Matrix3X3D RotateY(double angle)
        {
            var ca = Math.Cos(angle * Math.PI / 180.0);
            var sa = Math.Sin(angle * Math.PI / 180.0);

            return new Matrix3X3D(ca, 0, sa,
                0, 1, 0,
                -sa, 0, ca);
        }

        /// <summary>
        /// Create a rotation out of a angle.
        /// </summary>
        public static Matrix3X3D RotateZ(double angle)
        {
            var ca = Math.Cos(angle * Math.PI / 180.0);
            var sa = Math.Sin(angle * Math.PI / 180.0);

            return new Matrix3X3D(ca, -sa, 0,
                sa, ca, 0,
                0, 0, 1);
        }

        /// <summary>
        /// Create a rotation out of a vector.
        /// </summary>
        public static Matrix3X3D Rotate(Vector3D euler)
        {
            return Quaternion3D.FromEuler(euler).ToMatrix3X3D();
        }
    }
}