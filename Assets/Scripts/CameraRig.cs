using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [Header("Targets")]
    public Transform a;
    public Transform b;

    [Header("Framing")]
    public Vector3 offset = new Vector3(0, 18, -12);
    public float minZoom = 0.8f;
    public float maxZoom = 1.6f;
    public float zoomPerUnit = 0.04f;
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (!a || !b) return;

        Vector3 mid = (a.position + b.position) * 0.5f;
        float dist = Vector3.Distance(a.position, b.position);
        float zoom = Mathf.Clamp(minZoom + dist * zoomPerUnit, minZoom, maxZoom);

        Vector3 target = mid + offset * zoom;
        transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);
    }
}