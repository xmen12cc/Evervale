using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{

    public Transform target;
    public float posSmoothTime = 0.3f;
    public float rotSmoothTime = 0.3f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;

            Vector3 targetDirection = target.position - transform.position;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), rotSmoothTime * Time.deltaTime);

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, posSmoothTime * Time.deltaTime);
        }
    }

}
