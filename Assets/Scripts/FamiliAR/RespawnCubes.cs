using System.Collections.Generic;
using UnityEngine;

public class RespawnCubes : MonoBehaviour
{
    #region Class properties

    [Tooltip("Cubes to be respawned")]
    public GameObject[] cubes;

    [Tooltip("Initial (local) positions of cubes")]
    public List<Pose> cubeInitPoses;

    [Tooltip("Flag if objects are to be respawned to then execute during FixedUpdate")]
    public bool Respawn { get; set; }

    #endregion


    #region Unity lifecycle

    // Start is called before the first frame update
    void Start()
    {
        // Cache initial positions of cubes
        foreach (var cube in cubes)
        {
            cubeInitPoses.Add(new Pose(cube.transform.localPosition, cube.transform.localRotation));
        }
    }

    // FixedUpdate for physics stuff
    void FixedUpdate()
    {
        if (Respawn)
        {
            ResetCubes();

            // Flag respawn as done
            Respawn = false;
        }
    }

    #endregion


    #region Class methods

    public void ResetCubes()
    {
        for (int i = 0; i < cubes.Length; i++)
        {
            // Reset pose
            cubes[i].transform.SetLocalPositionAndRotation(cubeInitPoses[i].position, cubeInitPoses[i].rotation);

            // Reset rigidbody velocities
            Rigidbody rb = cubes[i].GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    #endregion
}
