/// <summary>
/// Used to stabilize rigidbody jitter on collision between rigidbodies by setting rigidbody to isKinematic when close to hold
/// </summary>

using UnityEngine;

public class StabilizeRigidbody : MonoBehaviour
{
    [Tooltip("Velocity threshold to consider the object as not moving")]
    public float velocityThreshold = 0.05f;

    private Rigidbody rb;
    private bool checkVelo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //void FixedUpdate()
    //{
    //    if (checkVelo)
    //    {
    //        CheckVelocity();
    //    }
    //}

    private void CheckVelocity()
    {
        if (rb.linearVelocity.magnitude < velocityThreshold && rb.angularVelocity.magnitude < velocityThreshold)
        {
            // Set to kinematic
            rb.isKinematic = true;

            // Stop checking velocity
            checkVelo = false;

            Debug.Log("Disabled rb");
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    // Reactivate rigidbody
    //    rb.isKinematic = false;

    //    // Check for velocity again
    //    Invoke(nameof(CheckVeloAgain), 0.5f);

    //    Debug.Log("Enabled rb (collision)");
    //}

    // Callback
    public void EnableRigidbody()
    {
        rb.isKinematic = false;

        Debug.Log("Enabled rb (manipulation)");
    }

    // Callback
    public void DisableRigidbody()
    {
        rb.isKinematic = true;

        Debug.Log("Disabled rb (manipulation)");
    }

    // Callback 
    public void CheckVeloAgain()
    {
        checkVelo = true;
    }
}