using UnityEngine;

namespace Comp3490Project
{
    //From: https://github.com/ttammear/unitymcubes
    public class FreeCamera : MonoBehaviour
    {
        public float MoveSpeed = 10.0f;
        float _rotX = 0.0f;
        float _rotY = 0.0f;

        void LateUpdate()
        {
            Transform camtrans = Camera.main.transform;
            if (Input.GetKey(KeyCode.W))
                camtrans.position += camtrans.forward * Time.deltaTime * MoveSpeed;
            if (Input.GetKey(KeyCode.S))
                camtrans.position -= camtrans.forward * Time.deltaTime * MoveSpeed;
            if (Input.GetKey(KeyCode.D))
                camtrans.position += camtrans.right * Time.deltaTime * MoveSpeed;
            if (Input.GetKey(KeyCode.A))
                camtrans.position -= camtrans.right * Time.deltaTime * MoveSpeed;

            _rotX += Input.GetAxis("Mouse X");
            _rotY += Input.GetAxis("Mouse Y");

            camtrans.rotation = Quaternion.AngleAxis(_rotX, Vector3.up) * Quaternion.AngleAxis(_rotY, -Vector3.right);
        }
    }
}