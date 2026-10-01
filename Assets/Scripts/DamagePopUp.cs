using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    public float lifetime = 0.8f;
    public float riseSpeed = 2f;

    TextMeshPro text;
    Color startColor;
    float age;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        startColor = text.color;
    }

    public void Init(float amount)
    {
        text.text = Mathf.RoundToInt(amount).ToString();
    }

    void LateUpdate()
    {
        age += Time.deltaTime;
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        if (Camera.main) transform.rotation = Camera.main.transform.rotation;  // face the camera

        Color c = startColor;
        c.a = 1f - age / lifetime;
        text.color = c;

        if (age >= lifetime) Destroy(gameObject);
    }
}