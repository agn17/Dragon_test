using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public string displayName = "Dragon";
    public float maxHealth = 100f;
    [SerializeField] float current;   // visible in the Inspector for debugging

    public float Current => current;
    public bool IsDead => current <= 0f;

    public event Action<float, float> OnChanged;  // current, max
    public event Action<float> OnDamaged;         // damage amount
    public event Action<Health> OnDied;

    void Awake() { current = maxHealth; }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        current = Mathf.Max(0f, current - amount);
        Debug.Log($"{displayName} took {amount} damage ({current}/{maxHealth})");
        OnDamaged?.Invoke(amount);
        OnChanged?.Invoke(current, maxHealth);
        if (IsDead) OnDied?.Invoke(this);
    }

    [ContextMenu("Test Damage 10")]
    void TestDamage() => TakeDamage(10f);
}