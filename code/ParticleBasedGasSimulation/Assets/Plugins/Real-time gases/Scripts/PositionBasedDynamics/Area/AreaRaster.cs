using System.Collections.Generic;
using System.Linq;
using Common.LinearAlgebra;
using UnityEngine;

namespace Plugins.Scripts.PositionBasedDynamics.Area
{
    /// <inheritdoc />
    /// <summary>
    /// This class holds the logic for the areas.
    /// </summary>
    public class AreaRaster : MonoBehaviour
    {
        public static AreaRaster RasterArea;

        public GameObject areaGasObject;

        public List<GameObject> areaPool;
        public List<GameObject> placedAreas;
        public List<AreaGas> areaGas;

        public Color emptyColor;
        public Color denseColor;

        public int amountOfAreaPool;

        public float areaRadius;

        public bool staticAreaMode;
        public bool placedAreaMode;

        private readonly int _density = Shader.PropertyToID("_Density");
        private readonly int _baseColor = Shader.PropertyToID("_FogBaseColor");
        private readonly int _denseColor = Shader.PropertyToID("_FogDenseColor");

        private void Awake()
        {
            RasterArea = this;

            areaPool = new List<GameObject>();
            for (var i = 0; i < amountOfAreaPool; i++)
            {
                var obj = Instantiate(areaGasObject, transform);
                obj.SetActive(false);
                areaPool.Add(obj);
            }

            if (placedAreaMode) InitializePlacedAreas();
            else areaGas = new List<AreaGas> {new AreaGas(GetPooledObject(), areaRadius)};
        }

        /// <summary>
        /// Adds all the manually placed areas.
        /// </summary>
        private void InitializePlacedAreas()
        {
            areaGas = new List<AreaGas>();
            foreach (var area in placedAreas)
            {
                var position = area.transform.position;
                var scale = area.transform.lossyScale;
                areaGas.Add(new AreaGas(GetPooledObject(),
                    new Vector3D(position.x, position.y, position.z),
                    scale));
            }
        }

        /// <summary>
        /// Add new gas object out of pool.
        /// </summary>
        public void NewGas(Vector3D position)
        {
            areaGas.Add(new AreaGas(GetPooledObject(), position,
                new Vector3(areaRadius * 2, areaRadius * 2, areaRadius * 2)));
        }

        /// <summary>
        /// Changes the area render density, based on the particles intersecting the area.
        /// </summary>
        public void UpdateGasArea(Vector3D location, int numberOfParticles)
        {
            foreach (var area in areaGas)
            {
                if (area.Density >= area.RenderDensity) area.RenderDensity += 0.001f;
                else area.RenderDensity -= 0.001f;

                area.Renderer.material.SetFloat(_density, area.RenderDensity);

                if (area.Density >= 0.5 && !area.IsToxic)
                {
                    area.Renderer.material.SetColor(_baseColor, denseColor);
                    area.Renderer.material.SetColor(_denseColor, denseColor);
                    area.IsToxic = true;
                }
                else if (area.Density < 0.5 && area.IsToxic)
                {
                    area.Renderer.material.SetColor(_baseColor, emptyColor);
                    area.Renderer.material.SetColor(_denseColor, emptyColor);
                    area.IsToxic = false;
                }

                area.Density = 0.0f;
            }

            if (staticAreaMode) return;
            if (areaGas[0].Density > 0.6)
            {
                areaRadius -= 0.01f;
            }
            else if (areaGas[0].Density < 0.4)
            {
                areaRadius += 0.01f;
            }

            areaGas[0].GasArea.transform.position = location.ToVec3();
            areaGas[0].VolumeLocation = location;
            areaGas[0].GasArea.transform.localScale =
                new Vector3(areaRadius * 2, areaRadius * 2, areaRadius * 2);
        }

        private GameObject GetPooledObject()
        {
            return areaPool.FirstOrDefault(t => !t.activeInHierarchy);
        }
    }
}