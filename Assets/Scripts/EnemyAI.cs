using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(AbilityRunner), typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    enum State { Idle, Chase, Attack }

    [Header("Target")]
    public Health player;

    [Header("Movement")]
    public float speed = 4f;
    public float turnSpeed = 8f;
    public float stopDistance = 2.5f;

    [Header("Behaviour")]
    public float idleTime = 1.5f;
    public float decisionDelay = 0.4f;   // pause between attacks so it isn't relentless
    public float tailRange = 3.5f;
    public float fireRange = 9f;
    public float facingTolerance = 20f;  // degrees; must face the player before attacking

    State state = State.Idle;
    float stateTimer;
    AbilityRunner abilities;
    CharacterController cc;
    Animator anim;
    Health hp;

    void Awake()
    {
        abilities = GetComponent<AbilityRunner>();
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        hp = GetComponent<Health>();
    }

    void Update()
    {
        if (hp.IsDead || player.IsDead || GameManager.IsOver)
        {
            anim.SetFloat("Speed", 0f);
            return;
        }

        // While an ability is playing, do nothing else.
        if (abilities.IsBusy)
        {
            anim.SetFloat("Speed", 0f);
            return;
        }
        if (state == State.Attack) Go(State.Chase);

        stateTimer += Time.deltaTime;

        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0f;
        float dist = toPlayer.magnitude;

        switch (state)
        {
            case State.Idle:
                anim.SetFloat("Speed", 0f);
                if (stateTimer >= idleTime) Go(State.Chase);
                break;

            case State.Chase:
                Face(toPlayer);
                if (dist > stopDistance)
                {
                    cc.SimpleMove(transform.forward * speed);
                    anim.SetFloat("Speed", 1f);
                }
                else anim.SetFloat("Speed", 0f);

                bool facing = Vector3.Angle(transform.forward, toPlayer) <= facingTolerance;
                if (stateTimer >= decisionDelay && facing && TryAttack(dist))
                    Go(State.Attack);
                break;
        }
    }

    // Choose an ability by distance; fall back to others if on cooldown.
    bool TryAttack(float dist)
    {
        if (dist <= tailRange) return abilities.TryUse(AbilityRunner.Tail) || abilities.TryUse(AbilityRunner.Fire);
        if (dist <= fireRange) return abilities.TryUse(AbilityRunner.Fire) || abilities.TryUse(AbilityRunner.Fly);
        return abilities.TryUse(AbilityRunner.Fly);   // far away: fly in
    }

    void Face(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
    }

    void Go(State s) { state = s; stateTimer = 0f; }
}