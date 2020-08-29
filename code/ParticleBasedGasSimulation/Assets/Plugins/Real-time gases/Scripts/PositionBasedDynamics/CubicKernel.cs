using System;
using Common.LinearAlgebra;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <summary>
    /// Kernel used for the position-based dynamics. 
    /// </summary>
    public class CubicKernel
    {
        public readonly double WZero;
        public readonly double K;
        public readonly double L;
        public readonly double Radius;
        public readonly double InvRadius;

        public CubicKernel(double radius)
        {
            Radius = radius;
            InvRadius = 1.0 / Radius;

            var h3 = Radius * Radius * Radius;

            K = 8.0 / (Math.PI * h3);
            L = 48.0f / (Math.PI * h3);

            WZero = W(0, 0, 0);
        }

        /// <summary>
        /// Kernel function.
        /// </summary>
        /// <returns>kernel double value</returns>
        public double W(double x, double y, double z)
        {
            var res = 0.0;
            var rl = Math.Sqrt(x * x + y * y + z * z);
            var q = rl * InvRadius;

            if (!(q <= 1.0)) return res;
            if (q <= 0.5)
            {
                var q2 = q * q;
                var q3 = q2 * q;
                res = K * (6.0 * q3 - 6.0f * q2 + 1.0);
            }
            else
            {
                var v = 1.0 - q;
                res = K * 2.0 * v * v * v;
            }

            return res;
        }

        /// <summary>
        /// Kernel gradient function.
        /// </summary>
        /// <returns>kernel Vector value</returns>
        public Vector3D GradW(double x, double y, double z)
        {
            var res = Vector3D.Zero;
            var rl = Math.Sqrt(x * x + y * y + z * z);
            var q = rl * InvRadius;

            if (!(q <= 1.0)) return res;
            if (!(rl > 1.0e-6)) return res;
            Vector3D gradq;

            var factor = 1.0 / (rl * Radius);

            gradq.x = x * factor;
            gradq.y = y * factor;
            gradq.z = z * factor;

            if (q <= 0.5)
            {
                factor = L * q * (3.0 * q - 2.0);

                res.x = gradq.x * factor;
                res.y = gradq.y * factor;
                res.z = gradq.z * factor;
            }
            else
            {
                factor = 1.0 - q;
                factor = L * (-factor * factor);

                res.x = gradq.x * factor;
                res.y = gradq.y * factor;
                res.z = gradq.z * factor;
            }

            return res;
        }
    }
}