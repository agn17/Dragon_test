using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Health health;
    public Image fill;
    public TMP_Text nameLabel;

    void Start()
    {
        if (nameLabel) nameLabel.text = health.displayName;
        health.OnChanged += Refresh;
        Refresh(health.Current, health.maxHealth);
    }

    void OnDestroy() { health.OnChanged -= Refresh; }

    void Refresh(float current, float max) { fill.fillAmount = current / max; }
}