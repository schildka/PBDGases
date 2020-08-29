using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Scripts.Demo
{
    public class LevelSwitch : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown("0")) SceneManager.LoadScene("Cartoon_SportCar_B01_exemple");
            if (Input.GetKeyDown("1")) SceneManager.LoadScene("outpost with snow");
        }
    }
}