using System.Collections.Generic;
using Common.LinearAlgebra;
using UnityEngine;

namespace Common.Unity
{
    public static class MathConverter
    {
        public static Vector3 ToVector3(Vector3D v)
        {
            return new Vector3((float) v.x, (float) v.y, (float) v.z);
        }

        public static Vector3D ToVector3D(Vector3 v)
        {
            return new Vector3D(v.x, v.y, v.z);
        }

        public static Vector4 ToVector4(Vector3D v)
        {
            return new Vector4((float) v.x, (float) v.y, (float) v.z, 1.0f);
        }

        public static Quaternion ToQuaternion(Quaternion3D q)
        {
            return new Quaternion((float) q.x, (float) q.y, (float) q.z, (float) q.w);
        }

        public static Quaternion3D ToQuaternion3D(Quaternion q)
        {
            return new Quaternion3D(q.x, q.y, q.z, q.w);
        }

        public static Matrix4x4 ToMatrix4X4(Matrix4X4D m)
        {
            var mat = new Matrix4x4
            {
                m00 = (float) m[0, 0],
                m01 = (float) m[0, 1],
                m02 = (float) m[0, 2],
                m03 = (float) m[0, 3],
                m10 = (float) m[1, 0],
                m11 = (float) m[1, 1],
                m12 = (float) m[1, 2],
                m13 = (float) m[1, 3],
                m20 = (float) m[2, 0],
                m21 = (float) m[2, 1],
                m22 = (float) m[2, 2],
                m23 = (float) m[2, 3],
                m30 = (float) m[3, 0],
                m31 = (float) m[3, 1],
                m32 = (float) m[3, 2],
                m33 = (float) m[3, 3]
            };


            return mat;
        }

        public static IList<Vector3> ToVector3(IList<Vector3D> list)
        {
            var vectors = new Vector3[list.Count];
            for (var i = 0; i < list.Count; i++)
                vectors[i] = new Vector3((float) list[i].x, (float) list[i].y, (float) list[i].z);

            return vectors;
        }
    }
}