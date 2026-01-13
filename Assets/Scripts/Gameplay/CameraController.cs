using UnityEngine;
using UnityEngine.Animations;

public class CameraController : MonoBehaviour
{
    public Transform spaceship;
    public Vector3 offset = new Vector3(0f, 5f, 15f);
    public float smoothSpeed = 15f;

    void LateUpdate()
    {
        Vector3 targetPosition = spaceship.position - spaceship.forward * offset.z + Vector3.up * offset.y;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.LookAt(spaceship.position);
    }
}
