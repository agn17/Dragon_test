using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float turnSpeed = 12f;

    [Header("References")]
    public AbilityRunner abilities;

    CharacterController cc;
    Animator anim;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (abilities != null && abilities.IsBusy)
        {
            anim.SetFloat("Speed", 0f);
        }
        else
        {
            Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

            cc.SimpleMove(dir * speed);

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion look = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
            }
            anim.SetFloat("Speed", dir.magnitude);
        }

        if (abilities == null) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) abilities.TryUse(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) abilities.TryUse(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) abilities.TryUse(2);
    }
}