using System;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.Scripts.PositionBasedDynamics.Area
{
    /// <inheritdoc />
    /// <summary>
    /// Observes the areas position, scale and density and prints the information into the UI.
    /// </summary>
    public class Observer : MonoBehaviour
    {
        public Text text;

        public Slider health;

        public GameObject player;

        private AreaRaster _areaRaster;

        private float _time = 0.0f;
        public float interpolationPeriod = 2.0f;

        private void Start()
        {
            _areaRaster = AreaRaster.RasterArea;
        }

        private void Update()
        {
            _time += Time.deltaTime;

            var areas = _areaRaster.areaGas;

            var tmp = "";

            for (var i = 0; i < areas.Count; i++)
            {
                tmp += " Area." + i + " Position: " + areas[i].GasArea.transform.position + " Radius: " +
                       areas[i].GasArea.transform.localScale / 2 + " Density: " +
                       Math.Round(areas[i].RenderDensity, 2) + "\n";

                if (!(areas[i].RenderDensity >= 0.5) || !(_time >= interpolationPeriod) ||
                    ReferenceEquals(player, null) || ReferenceEquals(health, null)) continue;
                var location = player.transform.position;
                var position = Math.Sqrt(Math.Pow((location.x - areas[i].VolumeLocation.x), 2) +
                                         Math.Pow((location.y - areas[i].VolumeLocation.y), 2) +
                                         Math.Pow((location.z - areas[i].VolumeLocation.z), 2));

                if (!(position <= (areas[i].GasArea.transform.lossyScale.x / 2))) continue;
                health.value -= 0.1f;
                _time = 0.0f;
            }

            text.text = tmp;
        }
    }
}