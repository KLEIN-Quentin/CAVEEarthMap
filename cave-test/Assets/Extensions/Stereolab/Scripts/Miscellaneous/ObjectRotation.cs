using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stereolab.Miscellaneous
{
    public class ObjectRotation : MonoBehaviour
    {
        [SerializeField]
        private Vector3 rotationSpeeds = Vector3.zero;

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.right, rotationSpeeds.x * Time.deltaTime, Space.Self);
            transform.Rotate(Vector3.up, rotationSpeeds.y * Time.deltaTime, Space.Self);
            transform.Rotate(Vector3.forward, rotationSpeeds.z * Time.deltaTime, Space.Self);
        }
    }
}