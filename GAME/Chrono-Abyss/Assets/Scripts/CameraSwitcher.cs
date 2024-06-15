using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    public CameraController cameraController;
    public float newZOffset;
    public float transitionDuration = 1f; // Duration for the smooth transition
    private float originalZOffset;
    private Coroutine transitionCoroutine;

    void Start()
    {
        // Store the original Z offset
        originalZOffset = cameraController.offset.z;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop any ongoing transition
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            // Start the smooth transition to the new Z offset
            transitionCoroutine = StartCoroutine(SmoothTransition(newZOffset));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop any ongoing transition
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            // Start the smooth transition back to the original Z offset
            transitionCoroutine = StartCoroutine(SmoothTransition(originalZOffset));
        }
    }

    private IEnumerator SmoothTransition(float targetZOffset)
    {
        float elapsedTime = 0f;
        float currentZOffset = cameraController.offset.z;

        while (elapsedTime < transitionDuration)
        {
            cameraController.offset = new Vector3(cameraController.offset.x, cameraController.offset.y, Mathf.Lerp(currentZOffset, targetZOffset, elapsedTime / transitionDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cameraController.offset = new Vector3(cameraController.offset.x, cameraController.offset.y, targetZOffset);
    }
}
