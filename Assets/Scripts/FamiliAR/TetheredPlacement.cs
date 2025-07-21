// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591


using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Helper script to re-spawn objects if they go too far from their original position. Adapted from MRTK3
    /// </summary>
    /// <remarks>
    /// The helper is useful for objects that will fall forever.
    /// </remarks>
    [AddComponentMenu("MRTK/Examples/Tethered Placement")]
    internal class TetheredPlacement : MonoBehaviour
    {
        [SerializeField, Tooltip("The distance from the GameObject's spawn position at which will trigger a respawn.")]
        private float distanceThreshold = 20.0f;

        [SerializeField, Tooltip("Timer for object if not visible anymore. Gets gespawned then")]
        private float spawnTime = 4f;

        [SerializeField, Tooltip("Flag if objects are to be respawned to then execute during FixedUpdate")]
        public bool Respawn {  get; set; }

        private Pose respawnPose;
        private Rigidbody rigidBody;

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            respawnPose.position = transform.localPosition;
            respawnPose.rotation = transform.localRotation;
        }

        
        /// <summary>
        /// Must use fixed update for rigibody related stuff
        /// </summary>
        private void FixedUpdate()
        {
            if (Respawn)
            {
                RespawnObject();

                // Flag respawn as done
                Respawn = false;
            }
        }
       

        /// <summary>
        /// Respawn mechanism
        /// </summary>
        public void RespawnObject()
        {
            // Reset any velocity from falling or moving when re-spawning to original location
            if (rigidBody != null)
            {
                rigidBody.linearVelocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
            }

            transform.SetLocalPositionAndRotation(respawnPose.position, respawnPose.rotation);

        }
    }
}
#pragma warning restore CS1591