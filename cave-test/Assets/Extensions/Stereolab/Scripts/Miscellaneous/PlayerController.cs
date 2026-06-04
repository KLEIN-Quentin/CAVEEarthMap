using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stereolab.Miscellaneous
{
    /// <summary>
    /// Simple player controller to test the parallax effect in builds without tracking anything.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Speed in meter per second.
        /// </summary>
        [SerializeField]
        private float speed = 1.0f;

        [SerializeField]
        private Space space = Space.Self;
        
        void Update()
        {
            transform.Translate(
                new Vector3(
                    Input.GetAxis("Horizontal"),
                    Input.GetAxis("Upward"),
                    Input.GetAxis("Vertical")
                ) * speed * Time.deltaTime, space
            );
        }
    }
}
