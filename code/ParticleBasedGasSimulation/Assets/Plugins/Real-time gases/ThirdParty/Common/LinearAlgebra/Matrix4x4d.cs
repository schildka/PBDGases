using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Common.LinearAlgebra
{
    /// <summary>
    /// A single precision 3 dimension matrix
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4X4D
    {
        /// <summary>
        /// The matrix
        /// </summary>
        private double m00, m01, m02, m03;

        private double m10, m11, m12, m13;
        private double m20, m21, m22, m23;
        private double m30, m31, m32, m33;

        /// <summary>
        /// The Matrix Identity.
        /// </summary>
        public static readonly Matrix4X4D Identity = new Matrix4X4D(1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);

        /// <summary>
        /// A matrix from the following variables.
        /// </summary>
        public Matrix4X4D(double m00, double m01, double m02, double m03,
            double m10, double m11, double m12, double m13,
            double m20, double m21, double m22, double m23,
            double m30, double m31, double m32, double m33)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;
            this.m03 = m03;
            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m20 = m20;
            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m30 = m30;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        /// <summary>
        /// A matrix from the following variables.
        /// </summary>
        public Matrix4X4D(double v)
        {
            m00 = v;
            m01 = v;
            m02 = v;
            m03 = v;
            m10 = v;
            m11 = v;
            m12 = v;
            m13 = v;
            m20 = v;
            m21 = v;
            m22 = v;
            m23 = v;
            m30 = v;
            m31 = v;
            m32 = v;
            m33 = v;
        }

        /// <summary>
        /// A matrix copied from a array of variables.
        /// </summary>
        public Matrix4X4D(double[,] m)
        {
            m00 = m[0, 0];
            m01 = m[0, 1];
            m02 = m[0, 2];
            m03 = m[0, 3];
            m10 = m[1, 0];
            m11 = m[1, 1];
            m12 = m[1, 2];
            m13 = m[1, 3];
            m20 = m[2, 0];
            m21 = m[2, 1];
            m22 = m[2, 2];
            m23 = m[2, 3];
            m30 = m[3, 0];
            m31 = m[3, 1];
            m32 = m[3, 2];
            m33 = m[3, 3];
        }

        /// <summary>
        /// A matrix copied from a array of variables.
        /// </summary>
        public Matrix4X4D(IReadOnlyList<double> m)
        {
            m00 = m[0 + 0 * 4];
            m01 = m[0 + 1 * 4];
            m02 = m[0 + 2 * 4];
            m03 = m[0 + 3 * 4];
            m10 = m[1 + 0 * 4];
            m11 = m[1 + 1 * 4];
            m12 = m[1 + 2 * 4];
            m13 = m[1 + 3 * 4];
            m20 = m[2 + 0 * 4];
            m21 = m[2 + 1 * 4];
            m22 = m[2 + 2 * 4];
            m23 = m[2 + 3 * 4];
            m30 = m[3 + 0 * 4];
            m31 = m[3 + 1 * 4];
            m32 = m[3 + 2 * 4];
            m33 = m[3 + 3 * 4];
        }

        /// <summary>
        /// A matrix copied from the matrix m.
        /// </summary>
        public Matrix4X4D(Matrix4X4D m)
        {
            m00 = m.m00;
            m01 = m.m01;
            m02 = m.m02;
            m03 = m.m03;
            m10 = m.m10;
            m11 = m.m11;
            m12 = m.m12;
            m13 = m.m13;
            m20 = m.m20;
            m21 = m.m21;
            m22 = m.m22;
            m23 = m.m23;
            m30 = m.m30;
            m31 = m.m31;
            m32 = m.m32;
            m33 = m.m33;
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
                    case 3: return m30;

                    case 4: return m01;
                    case 5: return m11;
                    case 6: return m21;
                    case 7: return m31;

                    case 8: return m02;
                    case 9: return m12;
                    case 10: return m22;
                    case 11: return m32;

                    case 12: return m03;
                    case 13: return m13;
                    case 14: return m23;
                    case 15: return m33;
                    default: throw new IndexOutOfRangeException("Matrix4x4d index out of range: " + i);
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
                        m30 = value;
                        break;

                    case 4:
                        m01 = value;
                        break;
                    case 5:
                        m11 = value;
                        break;
                    case 6:
                        m21 = value;
                        break;
                    case 7:
                        m31 = value;
                        break;

                    case 8:
                        m02 = value;
                        break;
                    case 9:
                        m12 = value;
                        break;
                    case 10:
                        m22 = value;
                        break;
                    case 11:
                        m32 = value;
                        break;

                    case 12:
                        m03 = value;
                        break;
                    case 13:
                        m13 = value;
                        break;
                    case 14:
                        m23 = value;
                        break;
                    case 15:
                        m33 = value;
                        break;
                    default: throw new IndexOutOfRangeException("Matrix4x4d index out of range: " + i);
                }
            }
        }

        /// <summary>
        /// Access the variable at index ij
        /// </summary>
        public double this[int i, int j]
        {
            get
            {
                var k = i + j * 4;
                switch (k)
                {
                    case 0: return m00;
                    case 1: return m10;
                    case 2: return m20;
                    case 3: return m30;

                    case 4: return m01;
                    case 5: return m11;
                    case 6: return m21;
                    case 7: return m31;

                    case 8: return m02;
                    case 9: return m12;
                    case 10: return m22;
                    case 11: return m32;

                    case 12: return m03;
                    case 13: return m13;
                    case 14: return m23;
                    case 15: return m33;
                    default: throw new IndexOutOfRangeException("Matrix4x4d index out of range: " + k);
                }
            }
            private set
            {
                var k = i + j * 4;
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
                        m30 = value;
                        break;

                    case 4:
                        m01 = value;
                        break;
                    case 5:
                        m11 = value;
                        break;
                    case 6:
                        m21 = value;
                        break;
                    case 7:
                        m31 = value;
                        break;

                    case 8:
                        m02 = value;
                        break;
                    case 9:
                        m12 = value;
                        break;
                    case 10:
                        m22 = value;
                        break;
                    case 11:
                        m32 = value;
                        break;

                    case 12:
                        m03 = value;
                        break;
                    case 13:
                        m13 = value;
                        break;
                    case 14:
                        m23 = value;
                        break;
                    case 15:
                        m33 = value;
                        break;
                    default: throw new IndexOutOfRangeException("Matrix4x4d index out of range: " + k);
                }
            }
        }

        /// <summary>
        /// The transpose of the matrix. The rows and columns are flipped.
        /// </summary>
        public Matrix4X4D Transpose
        {
            get
            {
                var transpose = new Matrix4X4D
                {
                    m00 = m00,
                    m01 = m10,
                    m02 = m20,
                    m03 = m30,
                    m10 = m01,
                    m11 = m11,
                    m12 = m21,
                    m13 = m31,
                    m20 = m02,
                    m21 = m12,
                    m22 = m22,
                    m23 = m32,
                    m30 = m03,
                    m31 = m13,
                    m32 = m23,
                    m33 = m33
                };


                return transpose;
            }
        }

        /// <summary>
        /// The determinate of a matrix. 
        /// </summary>
        private double Determinant =>
            (m00 * Minor(1, 2, 3, 1, 2, 3) -
             m01 * Minor(1, 2, 3, 0, 2, 3) +
             m02 * Minor(1, 2, 3, 0, 1, 3) -
             m03 * Minor(1, 2, 3, 0, 1, 2));

        /// <summary>
        /// The adjoint of a matrix. 
        /// </summary>
        private Matrix4X4D Adjoint =>
            new Matrix4X4D(
                Minor(1, 2, 3, 1, 2, 3),
                -Minor(0, 2, 3, 1, 2, 3),
                Minor(0, 1, 3, 1, 2, 3),
                -Minor(0, 1, 2, 1, 2, 3),
                -Minor(1, 2, 3, 0, 2, 3),
                Minor(0, 2, 3, 0, 2, 3),
                -Minor(0, 1, 3, 0, 2, 3),
                Minor(0, 1, 2, 0, 2, 3),
                Minor(1, 2, 3, 0, 1, 3),
                -Minor(0, 2, 3, 0, 1, 3),
                Minor(0, 1, 3, 0, 1, 3),
                -Minor(0, 1, 2, 0, 1, 3),
                -Minor(1, 2, 3, 0, 1, 2),
                Minor(0, 2, 3, 0, 1, 2),
                -Minor(0, 1, 3, 0, 1, 2),
                Minor(0, 1, 2, 0, 1, 2));

        /// <summary>
        /// The inverse of the matrix.
        /// A matrix multiplied by its inverse is the identity.
        /// </summary>
        public Matrix4X4D Inverse => Adjoint * (1.0 / Determinant);

        public double Trace => m00 + m11 + m22 + m33;

        /// <summary>
        /// Add two matrices.
        /// </summary>
        public static Matrix4X4D operator +(Matrix4X4D m1, Matrix4X4D m2)
        {
            var kSum = new Matrix4X4D
            {
                m00 = m1.m00 + m2.m00,
                m01 = m1.m01 + m2.m01,
                m02 = m1.m02 + m2.m02,
                m03 = m1.m03 + m2.m03,
                m10 = m1.m10 + m2.m10,
                m11 = m1.m11 + m2.m11,
                m12 = m1.m12 + m2.m12,
                m13 = m1.m13 + m2.m13,
                m20 = m1.m20 + m2.m20,
                m21 = m1.m21 + m2.m21,
                m22 = m1.m22 + m2.m22,
                m23 = m1.m23 + m2.m23,
                m30 = m1.m30 + m2.m30,
                m31 = m1.m31 + m2.m31,
                m32 = m1.m32 + m2.m32,
                m33 = m1.m33 + m2.m33
            };


            return kSum;
        }

        /// <summary>
        /// Subtract two matrices.
        /// </summary>
        public static Matrix4X4D operator -(Matrix4X4D m1, Matrix4X4D m2)
        {
            var kSum = new Matrix4X4D
            {
                m00 = m1.m00 - m2.m00,
                m01 = m1.m01 - m2.m01,
                m02 = m1.m02 - m2.m02,
                m03 = m1.m03 - m2.m03,
                m10 = m1.m10 - m2.m10,
                m11 = m1.m11 - m2.m11,
                m12 = m1.m12 - m2.m12,
                m13 = m1.m13 - m2.m13,
                m20 = m1.m20 - m2.m20,
                m21 = m1.m21 - m2.m21,
                m22 = m1.m22 - m2.m22,
                m23 = m1.m23 - m2.m23,
                m30 = m1.m30 - m2.m30,
                m31 = m1.m31 - m2.m31,
                m32 = m1.m32 - m2.m32,
                m33 = m1.m33 - m2.m33
            };


            return kSum;
        }

        /// <summary>
        /// Multiply two matrices.
        /// </summary>
        public static Matrix4X4D operator *(Matrix4X4D m1, Matrix4X4D m2)
        {
            var kProd = new Matrix4X4D
            {
                m00 = m1.m00 * m2.m00 + m1.m01 * m2.m10 + m1.m02 * m2.m20 + m1.m03 * m2.m30,
                m01 = m1.m00 * m2.m01 + m1.m01 * m2.m11 + m1.m02 * m2.m21 + m1.m03 * m2.m31,
                m02 = m1.m00 * m2.m02 + m1.m01 * m2.m12 + m1.m02 * m2.m22 + m1.m03 * m2.m32,
                m03 = m1.m00 * m2.m03 + m1.m01 * m2.m13 + m1.m02 * m2.m23 + m1.m03 * m2.m33,
                m10 = m1.m10 * m2.m00 + m1.m11 * m2.m10 + m1.m12 * m2.m20 + m1.m13 * m2.m30,
                m11 = m1.m10 * m2.m01 + m1.m11 * m2.m11 + m1.m12 * m2.m21 + m1.m13 * m2.m31,
                m12 = m1.m10 * m2.m02 + m1.m11 * m2.m12 + m1.m12 * m2.m22 + m1.m13 * m2.m32,
                m13 = m1.m10 * m2.m03 + m1.m11 * m2.m13 + m1.m12 * m2.m23 + m1.m13 * m2.m33,
                m20 = m1.m20 * m2.m00 + m1.m21 * m2.m10 + m1.m22 * m2.m20 + m1.m23 * m2.m30,
                m21 = m1.m20 * m2.m01 + m1.m21 * m2.m11 + m1.m22 * m2.m21 + m1.m23 * m2.m31,
                m22 = m1.m20 * m2.m02 + m1.m21 * m2.m12 + m1.m22 * m2.m22 + m1.m23 * m2.m32,
                m23 = m1.m20 * m2.m03 + m1.m21 * m2.m13 + m1.m22 * m2.m23 + m1.m23 * m2.m33,
                m30 = m1.m30 * m2.m00 + m1.m31 * m2.m10 + m1.m32 * m2.m20 + m1.m33 * m2.m30,
                m31 = m1.m30 * m2.m01 + m1.m31 * m2.m11 + m1.m32 * m2.m21 + m1.m33 * m2.m31,
                m32 = m1.m30 * m2.m02 + m1.m31 * m2.m12 + m1.m32 * m2.m22 + m1.m33 * m2.m32,
                m33 = m1.m30 * m2.m03 + m1.m31 * m2.m13 + m1.m32 * m2.m23 + m1.m33 * m2.m33
            };


            return kProd;
        }

        /// <summary>
        /// Multiply  a vector by a matrix.
        /// </summary>
        public static Vector3D operator *(Matrix4X4D m, Vector3D v)
        {
            var kProd = new Vector3D();

            var fInvW = 1.0 / (m.m30 * v.x + m.m31 * v.y + m.m32 * v.z + m.m33);

            kProd.x = (m.m00 * v.x + m.m01 * v.y + m.m02 * v.z + m.m03) * fInvW;
            kProd.y = (m.m10 * v.x + m.m11 * v.y + m.m12 * v.z + m.m13) * fInvW;
            kProd.z = (m.m20 * v.x + m.m21 * v.y + m.m22 * v.z + m.m23) * fInvW;

            return kProd;
        }

        /// <summary>
        /// Multiply  a vector by a matrix.
        /// </summary>
        public static Vector4D operator *(Matrix4X4D m, Vector4D v)
        {
            var kProd = new Vector4D
            {
                x = m.m00 * v.x + m.m01 * v.y + m.m02 * v.z + m.m03 * v.w,
                y = m.m10 * v.x + m.m11 * v.y + m.m12 * v.z + m.m13 * v.w,
                z = m.m20 * v.x + m.m21 * v.y + m.m22 * v.z + m.m23 * v.w,
                w = m.m30 * v.x + m.m31 * v.y + m.m32 * v.z + m.m33 * v.w
            };


            return kProd;
        }

        /// <summary>
        /// Multiply a matrix by a scalar.
        /// </summary>
        public static Matrix4X4D operator *(Matrix4X4D m1, double s)
        {
            var kProd = new Matrix4X4D
            {
                m00 = m1.m00 * s,
                m01 = m1.m01 * s,
                m02 = m1.m02 * s,
                m03 = m1.m03 * s,
                m10 = m1.m10 * s,
                m11 = m1.m11 * s,
                m12 = m1.m12 * s,
                m13 = m1.m13 * s,
                m20 = m1.m20 * s,
                m21 = m1.m21 * s,
                m22 = m1.m22 * s,
                m23 = m1.m23 * s,
                m30 = m1.m30 * s,
                m31 = m1.m31 * s,
                m32 = m1.m32 * s,
                m33 = m1.m33 * s
            };


            return kProd;
        }

        /// <summary>
        /// Are these matrices equal.
        /// </summary>
        public static bool operator ==(Matrix4X4D m1, Matrix4X4D m2)
        {
            if (m1.m00 != m2.m00) return false;
            if (m1.m01 != m2.m01) return false;
            if (m1.m02 != m2.m02) return false;
            if (m1.m03 != m2.m03) return false;

            if (m1.m10 != m2.m10) return false;
            if (m1.m11 != m2.m11) return false;
            if (m1.m12 != m2.m12) return false;
            if (m1.m13 != m2.m13) return false;

            if (m1.m20 != m2.m20) return false;
            if (m1.m21 != m2.m21) return false;
            if (m1.m22 != m2.m22) return false;
            if (m1.m23 != m2.m23) return false;

            if (m1.m30 != m2.m30) return false;
            if (m1.m31 != m2.m31) return false;
            if (m1.m32 != m2.m32) return false;
            return !(m1.m33 != m2.m33);
        }

        /// <summary>
        /// Are these matrices not equal.
        /// </summary>
        public static bool operator !=(Matrix4X4D m1, Matrix4X4D m2)
        {
            if (m1.m00 != m2.m00) return true;
            if (m1.m01 != m2.m01) return true;
            if (m1.m02 != m2.m02) return true;
            if (m1.m03 != m2.m03) return true;

            if (m1.m10 != m2.m10) return true;
            if (m1.m11 != m2.m11) return true;
            if (m1.m12 != m2.m12) return true;
            if (m1.m13 != m2.m13) return true;

            if (m1.m20 != m2.m20) return true;
            if (m1.m21 != m2.m21) return true;
            if (m1.m22 != m2.m22) return true;
            if (m1.m23 != m2.m23) return true;

            if (m1.m30 != m2.m30) return true;
            if (m1.m31 != m2.m31) return true;
            return m1.m32 != m2.m32;
        }

        /// <summary>
        /// Are these matrices equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Matrix4X4D)) return false;

            var mat = (Matrix4X4D) obj;

            return this == mat;
        }

        /// <summary>
        /// Are these matrices equal.
        /// </summary>
        public bool Equals(Matrix4X4D mat)
        {
            return this == mat;
        }

        /// <summary>
        /// Are these matrices equal.
        /// </summary>
        public bool EqualsWithError(Matrix4X4D m, double eps)
        {
            if (Math.Abs(m00 - m.m00) > eps) return false;
            if (Math.Abs(m10 - m.m10) > eps) return false;
            if (Math.Abs(m20 - m.m20) > eps) return false;
            if (Math.Abs(m30 - m.m30) > eps) return false;

            if (Math.Abs(m01 - m.m01) > eps) return false;
            if (Math.Abs(m11 - m.m11) > eps) return false;
            if (Math.Abs(m21 - m.m21) > eps) return false;
            if (Math.Abs(m31 - m.m31) > eps) return false;

            if (Math.Abs(m02 - m.m02) > eps) return false;
            if (Math.Abs(m12 - m.m12) > eps) return false;
            if (Math.Abs(m22 - m.m22) > eps) return false;
            if (Math.Abs(m32 - m.m32) > eps) return false;

            if (Math.Abs(m03 - m.m03) > eps) return false;
            if (Math.Abs(m13 - m.m13) > eps) return false;
            if (Math.Abs(m23 - m.m23) > eps) return false;
            return !(Math.Abs(m33 - m.m33) > eps);
        }

        /// <summary>
        /// Matrices hash code. 
        /// </summary>
        public override int GetHashCode()
        {
            var hash = 0;

            for (var i = 0; i < 16; i++)
                hash ^= this[i].GetHashCode();

            return hash;
        }

        /// <summary>
        /// A matrix as a string.
        /// </summary>
        public override string ToString()
        {
            return this[0, 0] + "," + this[0, 1] + "," + this[0, 2] + "," + this[0, 3] + "\n" +
                   this[1, 0] + "," + this[1, 1] + "," + this[1, 2] + "," + this[1, 3] + "\n" +
                   this[2, 0] + "," + this[2, 1] + "," + this[2, 2] + "," + this[2, 3] + "\n" +
                   this[3, 0] + "," + this[3, 1] + "," + this[3, 2] + "," + this[3, 3];
        }

        /// <summary>
        /// The minor of a matrix. 
        /// </summary>
        private double Minor(int r0, int r1, int r2, int c0, int c1, int c2)
        {
            return this[r0, c0] * (this[r1, c1] * this[r2, c2] - this[r2, c1] * this[r1, c2]) -
                   this[r0, c1] * (this[r1, c0] * this[r2, c2] - this[r2, c0] * this[r1, c2]) +
                   this[r0, c2] * (this[r1, c0] * this[r2, c1] - this[r2, c0] * this[r1, c1]);
        }

        /// <summary>
        /// The inverse of the matrix.
        /// A matrix multiplied by its inverse is the identity.
        /// </summary>
        public bool TryInverse(out Matrix4X4D mInv)
        {
            var det = Determinant;

            if (Math.Abs(det) <= 1e-09)
            {
                mInv = Identity;
                return false;
            }

            mInv = Adjoint * (1.0 / det);
            return true;
        }

        /// <summary>
        /// Get the ith column as a vector.
        /// </summary>
        public Vector4D GetColumn(int iCol)
        {
            return new Vector4D(this[0, iCol], this[1, iCol], this[2, iCol], this[3, iCol]);
        }

        /// <summary>
        /// Set the ith column from a vector.
        /// </summary>
        public void SetColumn(int iCol, Vector4D v)
        {
            this[0, iCol] = v.x;
            this[1, iCol] = v.y;
            this[2, iCol] = v.z;
            this[3, iCol] = v.w;
        }

        /// <summary>
        /// Flip the ith column.
        /// </summary>
        public void FlipColumn(int iCol)
        {
            this[0, iCol] *= -1.0;
            this[1, iCol] *= -1.0;
            this[2, iCol] *= -1.0;
            this[3, iCol] *= -1.0;
        }

        /// <summary>
        /// Get the ith row as a vector.
        /// </summary>
        public Vector4D GetRow(int iRow)
        {
            return new Vector4D(this[iRow, 0], this[iRow, 1], this[iRow, 2], this[iRow, 3]);
        }

        /// <summary>
        /// Set the ith row from a vector.
        /// </summary>
        public void SetRow(int iRow, Vector4D v)
        {
            this[iRow, 0] = v.x;
            this[iRow, 1] = v.y;
            this[iRow, 2] = v.z;
            this[iRow, 3] = v.w;
        }

        /// <summary>
        /// Flip the ith row.
        /// </summary>
        public void FlipRow(int iRow)
        {
            this[iRow, 0] *= -1.0f;
            this[iRow, 1] *= -1.0f;
            this[iRow, 2] *= -1.0f;
            this[iRow, 3] *= -1.0f;
        }

        /// <summary>
        /// Convert to a 3 dimension matrix.
        /// </summary>
        public Matrix3X3D ToMatrix3X3D()
        {
            var mat = new Matrix3X3D
            {
                m00 = m00,
                m01 = m01,
                m02 = m02,
                m10 = m10,
                m11 = m11,
                m12 = m12,
                m20 = m20,
                m21 = m21,
                m22 = m22
            };


            return mat;
        }

        /// <summary>
        /// Create a translation, rotation and scale.
        /// </summary>
        public static Matrix4X4D TranslateRotateScale(Vector3D t, Quaternion3D r, Vector3D s)
        {
            var T = Translate(t);
            var R = r.ToMatrix4X4D();
            var S = Scale(s);

            return T * R * S;
        }

        /// <summary>
        /// Create a translation and rotation.
        /// </summary>
        public static Matrix4X4D TranslateRotate(Vector3D t, Quaternion3D r)
        {
            var T = Translate(t);
            var R = r.ToMatrix4X4D();

            return T * R;
        }

        /// <summary>
        /// Create a translation and scale.
        /// </summary>
        public static Matrix4X4D TranslateScale(Vector3D t, Vector3D s)
        {
            var T = Translate(t);
            var S = Scale(s);

            return T * S;
        }

        /// <summary>
        /// Create a rotation and scale.
        /// </summary>
        public static Matrix4X4D RotateScale(Quaternion3D r, Vector3D s)
        {
            var R = r.ToMatrix4X4D();
            var S = Scale(s);

            return R * S;
        }

        /// <summary>
        /// Create a translation out of a vector.
        /// </summary>
        private static Matrix4X4D Translate(Vector3D v)
        {
            return new Matrix4X4D(1, 0, 0, v.x,
                0, 1, 0, v.y,
                0, 0, 1, v.z,
                0, 0, 0, 1);
        }

        /// <summary>
        /// Create a scale out of a vector.
        /// </summary>
        private static Matrix4X4D Scale(Vector3D v)
        {
            return new Matrix4X4D(v.x, 0, 0, 0,
                0, v.y, 0, 0,
                0, 0, v.z, 0,
                0, 0, 0, 1);
        }

        /// <summary>
        /// Create a scale out of a vector.
        /// </summary>
        public static Matrix4X4D Scale(double s)
        {
            return new Matrix4X4D(s, 0, 0, 0,
                0, s, 0, 0,
                0, 0, s, 0,
                0, 0, 0, 1);
        }

        /// <summary>
        /// Create a rotation out of a angle.
        /// </summary>
        public static Matrix4X4D RotateX(double angle)
        {
            var ca = Math.Cos(angle * Math.PI / 180.0);
            var sa = Math.Sin(angle * Math.PI / 180.0);

            return new Matrix4X4D(1, 0, 0, 0,
                0, ca, -sa, 0,
                0, sa, ca, 0,
                0, 0, 0, 1);
        }

        /// <summary>
        /// Create a rotation out of a angle.
        /// </summary>
        public static Matrix4X4D RotateY(double angle)
        {
            var ca = Math.Cos(angle * Math.PI / 180.0);
            var sa = Math.Sin(angle * Math.PI / 180.0);

            return new Matrix4X4D(ca, 0, sa, 0,
                0, 1, 0, 0,
                -sa, 0, ca, 0,
                0, 0, 0, 1);
        }

        /// <summary>
        /// Create a rotation out of a angle.
        /// </summary>
        public static Matrix4X4D RotateZ(double angle)
        {
            var ca = Math.Cos(angle * Math.PI / 180.0);
            var sa = Math.Sin(angle * Math.PI / 180.0);

            return new Matrix4X4D(ca, -sa, 0, 0,
                sa, ca, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }

        /// <summary>
        /// Create a rotation out of a vector.
        /// </summary>
        public static Matrix4X4D Rotate(Vector3D euler)
        {
            return Quaternion3D.FromEuler(euler).ToMatrix4X4D();
        }

        /// <summary>
        /// Create a perspective matrix.
        /// </summary>
        public static Matrix4X4D Perspective(double fovy, double aspect, double zNear, double zFar)
        {
            var f = 1.0 / Math.Tan((fovy * Math.PI / 180.0) / 2.0);
            return new Matrix4X4D(f / aspect, 0, 0, 0,
                0, f, 0, 0,
                0, 0, (zFar + zNear) / (zNear - zFar), (2.0f * zFar * zNear) / (zNear - zFar),
                0, 0, -1, 0);
        }

        /// <summary>
        /// Create a ortho matrix.
        /// </summary>
        public static Matrix4X4D Ortho(double xRight, double xLeft, double yTop, double yBottom, double zNear,
            double zFar)
        {
            var tx = -(xRight + xLeft) / (xRight - xLeft);
            var ty = -(yTop + yBottom) / (yTop - yBottom);
            var tz = -(zFar + zNear) / (zFar - zNear);
            return new Matrix4X4D(2.0 / (xRight - xLeft), 0, 0, tx,
                0, 2.0 / (yTop - yBottom), 0, ty,
                0, 0, -2.0 / (zFar - zNear), tz,
                0, 0, 0, 1);
        }

        /// <summary>
        /// Creates the matrix need to look at target from position.
        /// </summary>
        public static Matrix4X4D LookAt(Vector3D position, Vector3D target, Vector3D Up)
        {
            var zaxis = (position - target).Normalized;
            var xaxis = Up.Cross(zaxis).Normalized;
            var yaxis = zaxis.Cross(xaxis);

            return new Matrix4X4D(xaxis.x, xaxis.y, xaxis.z, -Vector3D.Dot(xaxis, position),
                yaxis.x, yaxis.y, yaxis.z, -Vector3D.Dot(yaxis, position),
                zaxis.x, zaxis.y, zaxis.z, -Vector3D.Dot(zaxis, position),
                0, 0, 0, 1);
        }
    }
}