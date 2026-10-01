using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [Header("Targets")]
    public Transform a;
    public Transform b;
    public Health healthA;
    public Health healthB;

    [Header("Framing")]
    public Vector3 offset = new Vector3(0, 18, -12);
    public float minZoom = 0.8f;
    public float maxZoom = 1.6f;
    public float zoomPerUnit = 0.04f;
    public float followSpeed = 5f;

    [Header("Shake")]
    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.25f;

    Vector3 basePos;
    float shakeTimer;
    float shakeAmount;

    void Start()
    {
        basePos = transform.position;
        if (healthA) healthA.OnDamaged += OnHit;
        if (healthB) healthB.OnDamaged += OnHit;
    }

    void OnDestroy()
    {
        if (healthA) healthA.OnDamaged -= OnHit;
        if (healthB) healthB.OnDamaged -= OnHit;
    }

    void OnHit(float damage)
    {
        // Bigger hits shake more, capped so fire ticks stay subtle.
        shakeAmount = Mathf.Min(shakeStrength, shakeStrength * damage / 20f);
        shakeTimer = shakeDuration;
    }

    void LateUpdate()
    {
        if (!a || !b) return;

        Vector3 mid = (a.position + b.position) * 0.5f;
        float dist = Vector3.Distance(a.position, b.position);
        float zoom = Mathf.Clamp(minZoom + dist * zoomPerUnit, minZoom, maxZoom);

        Vector3 target = mid + offset * zoom;
        basePos = Vector3.Lerp(basePos, target, followSpeed * Time.deltaTime);

        Vector3 shake = Vector3.zero;
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            shake = Random.insideUnitSphere * shakeAmount * (shakeTimer / shakeDuration);
        }
        transform.position = basePos + shake;
    }
}