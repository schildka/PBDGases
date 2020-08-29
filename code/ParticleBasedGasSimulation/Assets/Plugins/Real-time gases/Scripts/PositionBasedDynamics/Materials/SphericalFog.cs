using UnityEngine;

namespace Plugins.Scripts.PositionBasedDynamics.Materials
{
    /// <inheritdoc />
    /// <summary>
    /// Calculates the Mesh properties and passes them to the area shader.
    /// </summary>
    [ExecuteInEditMode]
    public class SphericalFog : MonoBehaviour
    {
        private MeshRenderer _sphericalFogObject;
        public Material sphericalFogMaterial;
        public float scaleFactor = 1;
        public bool clipY = true;
        public bool clipX = true;
        public bool clipMx = true;
        public bool clipZ = true;
        public bool clipMz = true;
        private static readonly int FogParam = Shader.PropertyToID("FogParam");
        private static readonly int ClipY = Shader.PropertyToID("ClipY");
        private static readonly int ClipX = Shader.PropertyToID("ClipX");
        private static readonly int ClipMx = Shader.PropertyToID("ClipMX");
        private static readonly int ClipZ = Shader.PropertyToID("ClipZ");
        private static readonly int ClipMz = Shader.PropertyToID("ClipMZ");

        private void OnEnable()
        {
            _sphericalFogObject = gameObject.GetComponent<MeshRenderer>();
            if (_sphericalFogObject == null)
                Debug.LogError("Volume Fog Object must have a MeshRenderer Component!");

            if (Camera.main != null && Camera.main.depthTextureMode == DepthTextureMode.None)
                Camera.main.depthTextureMode = DepthTextureMode.Depth;

            _sphericalFogObject.material = sphericalFogMaterial;

            int y = 0, x = 0, mx = 0, z = 0, mz = 0;

            if (clipY) y = 1;
            if (clipX) x = 1;
            if (clipMx) mx = 1;
            if (clipZ) z = 1;
            if (clipMz) mz = 1;

            _sphericalFogObject.material.SetInt(ClipY, y);
            _sphericalFogObject.material.SetInt(ClipX, x);
            _sphericalFogObject.material.SetInt(ClipMx, mx);
            _sphericalFogObject.material.SetInt(ClipZ, z);
            _sphericalFogObject.material.SetInt(ClipMz, mz);
        }

        private void Update()
        {
            var lossyScale = transform.lossyScale;
            var radius = (lossyScale.x + lossyScale.y + lossyScale.z) / 8;
            var mat = Application.isPlaying ? _sphericalFogObject.material : _sphericalFogObject.sharedMaterial;
            if (!mat) return;
            var transform1 = transform;
            var position = transform1.position;
            mat.SetVector(FogParam,
                new Vector4(position.x, position.y, position.z,
                    radius * scaleFactor));
        }
    }
}