using UnityEngine;

public class AlignPlayingField : MonoBehaviour
{
    private void Update()
    {
        // Normalized vector from hit point to camera and then projected onto floor plane
        Vector3 toCam = (Camera.main.transform.position - transform.position).normalized;
        Vector3 projToCam = Vector3.ProjectOnPlane(toCam, Vector3.up).normalized;

        // Align game area so that it is always orthogonal to user but still gravity aligned
        transform.up = Vector3.up;
        transform.forward = projToCam;
    }
}
