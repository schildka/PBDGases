using UnityEngine;

namespace Plugins.Plugins.Cartoon_SportCar_B01.Standard_Assets.script
{
    public class SimpleActivatorMenu : MonoBehaviour
    {
        // An incredibly simple menu which, when given references
        // to gameobjects in the scene
        public GUIText camSwitchButton;
        public GameObject[] objects;


        private int _mCurrentActiveObject;


        private void OnEnable()
        {
            // active object starts from first in array
            _mCurrentActiveObject = 0;
            camSwitchButton.text = objects[_mCurrentActiveObject].name;
        }


        public void NextCamera()
        {
            var nextactiveobject = _mCurrentActiveObject + 1 >= objects.Length ? 0 : _mCurrentActiveObject + 1;

            for (var i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(i == nextactiveobject);
            }

            _mCurrentActiveObject = nextactiveobject;
            camSwitchButton.text = objects[_mCurrentActiveObject].name;
        }
    }
}