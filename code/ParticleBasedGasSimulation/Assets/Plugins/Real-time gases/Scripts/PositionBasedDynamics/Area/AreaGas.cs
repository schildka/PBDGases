using Common.LinearAlgebra;
using UnityEngine;

namespace Plugins.Scripts.PositionBasedDynamics.Area
{
    /// <summary>
    /// Area class that holds the area renderer shader
    /// </summary>
    public class AreaGas
    {
        public readonly GameObject GasArea;
        public readonly Renderer Renderer;
        public Vector3D VolumeLocation;
        public float Density;
        public float RenderDensity;
        public bool IsToxic = false;

        public AreaGas(GameObject areaGas, float radius)
        {
            areaGas.SetActive(true);
            GasArea = areaGas;
            GasArea.transform.position = new Vector3(0, 0, 0);
            GasArea.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
            Renderer = GasArea.GetComponent<Renderer>();
            VolumeLocation = Vector3D.Zero;
            Density = 0.0f;
            RenderDensity = 0.0f;
        }

        public AreaGas(GameObject areaGas, Vector3D volumeLocation, Vector3 scale)
        {
            areaGas.SetActive(true);
            GasArea = areaGas;
            GasArea.transform.position = volumeLocation.ToVec3();
            GasArea.transform.localScale = scale;
            Renderer = GasArea.GetComponent<Renderer>();
            VolumeLocation = volumeLocation;
            Density = 0.0f;
            RenderDensity = 0.0f;
        }
    }
}