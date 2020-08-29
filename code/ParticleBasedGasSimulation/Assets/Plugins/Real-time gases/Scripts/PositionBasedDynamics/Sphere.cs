using System;
using System.Collections.Generic;
using Common.LinearAlgebra;
using UnityEngine;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <inheritdoc />
    /// <summary>
    /// Finds neighboring solids for the particle using raycasts between the current position and the predicted position.
    /// </summary>
    public class Sphere : MonoBehaviour
    {
        public HashSet<Vector3D> normal;
        public Dictionary<Vector3D, List<Vector3D>> point;
        public List<Vector3D> pointList;

        private static double _halfVec = 0.5;

        private readonly Quaternion3D _xDegree = Quaternion3D.FromEuler(new Vector3D(90, 0, 0));
        private readonly Quaternion3D _yDegree = Quaternion3D.FromEuler(new Vector3D(0, 90, 0));
        private readonly Quaternion3D _zDegree = Quaternion3D.FromEuler(new Vector3D(0, 0, 90));
        private Quaternion3D _degree = Quaternion3D.FromEuler(new Vector3D(0, 0, 0));

        private void Start()
        {
            normal = new HashSet<Vector3D>();
            point = new Dictionary<Vector3D, List<Vector3D>>();
            pointList = new List<Vector3D>();
            _halfVec = transform.lossyScale.x;
        }

        public void UpdateBounds(Vector3 predicted)
        {
            UpdateCurrentBounds();

            var dir = predicted - this.transform.position;

            RaycastHit hitInfo;
            var layerMask = 1 << 9;
            layerMask = ~layerMask;

            if (!Physics.Raycast(new Ray(this.transform.position, dir), out hitInfo, 2, layerMask)) return;
            var n = new Vector3D(hitInfo.normal.x, hitInfo.normal.y, hitInfo.normal.z);
            if (!normal.Contains(n))
                OnCollisionFirst(hitInfo.point, n);
        }

        private void UpdateCurrentBounds()
        {
            var layerMask = 1 << 9;
            layerMask = ~layerMask;

            foreach (var normalTmp in new HashSet<Vector3D>(normal))
            {
                var pointTmp = point[normalTmp][0];
                point.Remove(normalTmp);
                normal.Remove(normalTmp);

                RaycastHit hitInfo;
                if (!Physics.Raycast(new Ray(this.transform.position + normalTmp.ToVec3(), -normalTmp.ToVec3()),
                    out hitInfo, 2, layerMask)) continue;
                if (normal.Count > 3) continue;
                OnCollisionSecond(pointTmp, normalTmp);
            }
        }

        private void OnCollisionFirst(Vector3 pointTmp, Vector3D normalTmp)
        {
            if (Math.Abs(normalTmp.x) >= Math.Abs(normalTmp.y) && Math.Abs(normalTmp.y) >= Math.Abs(normalTmp.z))
                _degree = _zDegree;
            else if (Math.Abs(normalTmp.x) >= Math.Abs(normalTmp.y) && Math.Abs(normalTmp.z) >= Math.Abs(normalTmp.y))
                _degree = _yDegree;
            else
                _degree = _xDegree;

            var initialPoint = new Vector3D(pointTmp.x, pointTmp.y, pointTmp.z);

            pointList.Add(initialPoint);

            var norXp = (_degree * normalTmp) * _halfVec + initialPoint;
            var norXn = Vector3D.Cross((_degree * normalTmp), normalTmp) * _halfVec + initialPoint;

            var norZp = (initialPoint - norXp) + initialPoint;
            var norZn = (initialPoint - norXn) + initialPoint;

            pointList.Add(norXp);
            pointList.Add(norXn);
            pointList.Add(norZp);
            pointList.Add(norZn);

            norXp = (_degree * normalTmp) + initialPoint;

            norXn = Vector3D.Cross((_degree * normalTmp), normalTmp) + initialPoint;

            norZp = (initialPoint - norXp) + initialPoint;
            norZn = (initialPoint - norXn) + initialPoint;

            var yDegOne = (norXp + norXn) / 2.0;
            var yDegTwo = (norXp + norZn) / 2.0;
            var yDegThree = (norXn + norZp) / 2.0;
            var yDegFour = (norZp + norZn) / 2.0;

            pointList.Add(yDegOne);
            pointList.Add(yDegTwo);
            pointList.Add(yDegThree);
            pointList.Add(yDegFour);

            normal.Add(normalTmp);

            point.Add(normalTmp, new List<Vector3D>(pointList));
            pointList.Clear();
        }

        private void OnCollisionSecond(Vector3D pointTmp, Vector3D normalTmp)
        {
            var position = transform.position;
            var sphere = new Vector3D(position.x, position.y,
                position.z);

            if (Math.Abs(normalTmp.x) >= Math.Abs(normalTmp.y) && Math.Abs(normalTmp.y) >= Math.Abs(normalTmp.z))
                _degree = _zDegree;
            else if (Math.Abs(normalTmp.x) >= Math.Abs(normalTmp.y) && Math.Abs(normalTmp.z) >= Math.Abs(normalTmp.y))
                _degree = _yDegree;
            else
                _degree = _xDegree;

            var initialPoint = LevelLineIntersection(normalTmp, pointTmp, normalTmp, sphere);

            pointList.Add(initialPoint);

            var norXp = (_degree * normalTmp) * _halfVec + initialPoint;
            var norXn = Vector3D.Cross((_degree * normalTmp), normalTmp) * _halfVec + initialPoint;

            var norZp = (initialPoint - norXp) + initialPoint;
            var norZn = (initialPoint - norXn) + initialPoint;

            pointList.Add(norXp);
            pointList.Add(norXn);
            pointList.Add(norZp);
            pointList.Add(norZn);

            norXp = (_degree * normalTmp) + initialPoint;

            norXn = Vector3D.Cross((_degree * normalTmp), normalTmp) + initialPoint;

            norZp = (initialPoint - norXp) + initialPoint;
            norZn = (initialPoint - norXn) + initialPoint;

            var yDegOne = (norXp + norXn) / 2.0;
            var yDegTwo = (norXp + norZn) / 2.0;
            var yDegThree = (norXn + norZp) / 2.0;
            var yDegFour = (norZp + norZn) / 2.0;

            pointList.Add(yDegOne);
            pointList.Add(yDegTwo);
            pointList.Add(yDegThree);
            pointList.Add(yDegFour);


            normal.Add(normalTmp);

            point.Add(normalTmp, new List<Vector3D>(pointList));
            pointList.Clear();
        }

        /// <summary>
        /// Calculates intersection of initial collision with new collision, to find point on same level.
        /// </summary>
        /// <returns>Vector of intersection</returns>
        private static Vector3D LevelLineIntersection(Vector3D levelNormal, Vector3D levelPoint, Vector3D lineNormal,
            Vector3D linePoint)
        {
            var level = (levelNormal.x * levelPoint.x) + (levelNormal.y * levelPoint.y) +
                        (levelNormal.z * levelPoint.z);

            var tmp0 = (levelNormal.x * linePoint.x) + (levelNormal.y * linePoint.y) + (levelNormal.z * linePoint.z);

            var tmp1 = (levelNormal.x * -lineNormal.x) + (levelNormal.y * -lineNormal.y) +
                       (levelNormal.z * -lineNormal.z);

            var intersection = (level - tmp0) / tmp1;
            return new Vector3D(linePoint.x + (intersection * (-lineNormal.x)),
                linePoint.y + (intersection * (-lineNormal.y)), linePoint.z + (intersection * (-lineNormal.z)));
        }
    }
}