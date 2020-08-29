using UnityEngine;

#pragma warning disable 618

namespace Plugins.Scripts.Demo
{
    public class CameraController : MonoBehaviour
    {
        public float cameraSensitivity = 90;
        public float climbSpeed = 4;
        public float normalMoveSpeed = 10;
        public float slowMoveFactor = 0.25f;
        public float fastMoveFactor = 3;

        private float _rotationX = 0.0f;
        private float _rotationY = 0.0f;

        private void Start()
        {
            Screen.lockCursor = true;
        }

        private void Update()
        {
            _rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            _rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
            _rotationY = Mathf.Clamp(_rotationY, -90, 90);

            var localRotation = Quaternion.AngleAxis(_rotationX, Vector3.up);
            localRotation *= Quaternion.AngleAxis(_rotationY, Vector3.left);
            transform.localRotation = localRotation;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                var transform1 = transform;
                var position = transform1.position;
                position += transform1.forward * (normalMoveSpeed * fastMoveFactor) *
                            Input.GetAxis("Vertical") * Time.deltaTime;
                position += transform.right * (normalMoveSpeed * fastMoveFactor) *
                            Input.GetAxis("Horizontal") * Time.deltaTime;
                transform.position = position;
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                var transform1 = transform;
                var position = transform1.position;
                position += transform1.forward * (normalMoveSpeed * slowMoveFactor) *
                            Input.GetAxis("Vertical") * Time.deltaTime;
                position += transform.right * (normalMoveSpeed * slowMoveFactor) *
                            Input.GetAxis("Horizontal") * Time.deltaTime;
                transform.position = position;
            }
            else
            {
                Transform transform1;
                (transform1 = transform).position +=
                    transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
                transform.position += transform1.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
            }


            if (Input.GetKey(KeyCode.R))
            {
                var transform1 = transform;
                transform1.position += transform1.up * climbSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.F))
            {
                var transform1 = transform;
                transform1.position -= transform1.up * climbSpeed * Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.End))
            {
                Screen.lockCursor = (Screen.lockCursor == false) ? true : false;
            }
        }
    }
}