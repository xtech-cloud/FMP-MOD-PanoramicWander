using UnityEngine;

namespace XTC.FMP.MOD.PanoramicWander.LIB.Unity
{
    public class MouseInputMock : MonoBehaviour
    {
        private Transform mainCamera_;
        private float rotationX_;
        private float rotationY_;

        private void Start()
        {
            mainCamera_ = Camera.main.transform;
        }

        private void Update()
        {
            rotationX_ += Input.GetAxis("Mouse X");
            rotationY_ += Input.GetAxis("Mouse Y");
            mainCamera_.rotation = Quaternion.Euler(-rotationY_, rotationX_, 0);
        }

    }
}
