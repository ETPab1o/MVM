using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;
    public float zoomAmount = 2f;
    public float zoomSpeed = 5f;

    private Vector3 originalOffset;
    private float shakeTimer = 0f;
    private bool isZoomed = false;

    void Start()
    {
        originalOffset = offset;
    }

    void LateUpdate()
    {
        // Desired position with fixed Z axis
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z);
        // Smoothed position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // Apply position
        transform.position = smoothedPosition;

        if (shakeTimer > 0)
        {
            transform.localPosition = transform.position + Random.insideUnitSphere * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }
    }

    public void TriggerShake()
    {
        shakeTimer = shakeDuration;
    }

    public void SetZoom(bool zoomIn)
    {
        if (zoomIn)
        {
            if (!isZoomed)
            {
                offset.z -= zoomAmount;
                isZoomed = true;
            }
        }
        else
        {
            if (isZoomed)
            {
                offset.z += zoomAmount;
                isZoomed = false;
            }
        }
    }
}
