using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <inheritdoc />
    /// <summary>
    /// Object pool for particle objects.
    /// </summary>
    public class GasPool : MonoBehaviour
    {
        public static GasPool SharedInstance;

        public List<GameObject> pooledGas;
        public GameObject gasToPool;
        public int amountToPool;

        private void Awake()
        {
            SharedInstance = this;
            pooledGas = new List<GameObject>();
            for (var i = 0; i < amountToPool; i++)
            {
                var obj = Instantiate(gasToPool, transform);
                obj.SetActive(false);
                pooledGas.Add(obj);
            }
        }

        public GameObject GetPooledObject()
        {
            return pooledGas.FirstOrDefault(t => !t.activeInHierarchy);
        }
    }
}