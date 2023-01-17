using UnityEngine;
using System.Collections;

namespace nv
{

    public class RotateTransform : MonoBehaviour
    {

        public Vector3 rotateAxis = Vector3.zero;

        public float rotationRate = 1.0f;

        public bool worldRotation = false;

        // Update is called once per frame
        void Update()
        {

            if(worldRotation)
                transform.rotation *= Quaternion.AngleAxis(rotationRate * Time.deltaTime, rotateAxis);
            else
                transform.localRotation *= Quaternion.AngleAxis(rotationRate * Time.deltaTime, rotateAxis);

        }
    }

}