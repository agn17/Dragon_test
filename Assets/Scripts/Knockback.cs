using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Knockback : MonoBehaviour
{
    CharacterController cc;
    Vector3 vel;

    void Awake() { cc = GetComponent<CharacterController>(); }

    public void Push(Vector3 dir, float force) { vel = dir * force; }

    void Update()
    {
        if (vel.sqrMagnitude < 0.01f || !cc.enabled) return;
        cc.Move(vel * Time.deltaTime);
        vel = Vector3.Lerp(vel, Vector3.zero, 8f * Time.deltaTime);
    }
}