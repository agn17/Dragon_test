using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityRunner : MonoBehaviour
{
    public const int Fire = 0, Tail = 1, Fly = 2;

    [Header("Setup")]
    public AbilityData[] abilities;   // 0 Fire, 1 Tail, 2 Fly
    public Transform firePoint;
    public LayerMask targetMask;      // set to Dragon
    public Health target;             // the opponent (used by Fly)

    [Header("Fire")]
    public float fireWindUp = 0.4f;
    public float fireDuration = 1.2f;
    public float fireTickRate = 0.3f;
    public float fireAngle = 35f;

    [Header("Tail")]
    public float tailWindUp = 0.4f;
    public float tailRecovery = 0.4f;
    public float tailKnockback = 8f;

    [Header("Fly")]
    public float flyHeight = 4f;
    public float riseTime = 0.6f;
    public float travelTime = 0.8f;
    public float diveTime = 0.3f;
    public float landRadius = 4f;
    public float landOffset = 2f;     // lands this far short of the target

    public bool IsBusy { get; private set; }

    float[] readyAt;
    Animator anim;
    AudioSource audioSrc;
    CharacterController cc;
    Health myHealth;

    void Awake()
    {
        readyAt = new float[abilities.Length];
        anim = GetComponentInChildren<Animator>();
        audioSrc = GetComponent<AudioSource>();
        cc = GetComponent<CharacterController>();
        myHealth = GetComponent<Health>();
    }

    public float CooldownRemaining(int i) => Mathf.Max(0f, readyAt[i] - Time.time);
    public float CooldownNormalized(int i) => CooldownRemaining(i) / abilities[i].cooldown;
    public bool IsReady(int i) => !IsBusy && !myHealth.IsDead && Time.time >= readyAt[i];

    public bool TryUse(int i)
    {
        if (!IsReady(i)) return false;
        readyAt[i] = Time.time + abilities[i].cooldown;
        StartCoroutine(Run(i));
        return true;
    }

    IEnumerator Run(int i)
    {
        IsBusy = true;
        AbilityData a = abilities[i];
        anim.SetTrigger(a.animTrigger);
        if (a.sfx && audioSrc) audioSrc.PlayOneShot(a.sfx);

        switch (i)
        {
            case Fire: yield return DoFire(a); break;
            case Tail: yield return DoTail(a); break;
            case Fly: yield return DoFly(a); break;
        }
        IsBusy = false;
    }

    // ---------- Fire: cone damage over time ----------
    IEnumerator DoFire(AbilityData a)
    {
        yield return new WaitForSeconds(fireWindUp);

        Transform spawn = firePoint ? firePoint : transform;
        GameObject vfx = a.vfxPrefab ? Instantiate(a.vfxPrefab, spawn.position, spawn.rotation, spawn) : null;

        int ticks = Mathf.Max(1, Mathf.RoundToInt(fireDuration / fireTickRate));
        float damagePerTick = a.damage / ticks;

        for (int t = 0; t < ticks; t++)
        {
            foreach (Health hp in FindTargets(a.range))
            {
                Vector3 to = hp.transform.position - transform.position;
                to.y = 0f;
                if (Vector3.Angle(transform.forward, to) <= fireAngle)
                    hp.TakeDamage(damagePerTick);
            }
            yield return new WaitForSeconds(fireTickRate);
        }
        if (vfx) Destroy(vfx);
    }

    // ---------- Tail: close-range hit + knockback ----------
    IEnumerator DoTail(AbilityData a)
    {
        yield return new WaitForSeconds(tailWindUp);

        foreach (Health hp in FindTargets(a.range))
        {
            hp.TakeDamage(a.damage);
            Vector3 dir = hp.transform.position - transform.position;
            dir.y = 0f;
            hp.GetComponent<Knockback>()?.Push(dir.normalized, tailKnockback);
        }
        if (a.vfxPrefab) Destroy(Instantiate(a.vfxPrefab, transform.position, Quaternion.identity), 2f);

        yield return new WaitForSeconds(tailRecovery);
    }

    // ---------- Fly: take off, fly over target, slam down ----------
    IEnumerator DoFly(AbilityData a)
    {
        cc.enabled = false;   // we move the transform directly

        Vector3 start = transform.position;
        Vector3 up = start + Vector3.up * flyHeight;
        yield return MoveTo(start, up, riseTime);

        Vector3 dest = target ? target.transform.position : start;
        Vector3 flat = dest - start; flat.y = 0f;
        Vector3 dir = flat.sqrMagnitude > 0.01f ? flat.normalized : transform.forward;
        transform.rotation = Quaternion.LookRotation(dir);

        Vector3 land = new Vector3(dest.x, start.y, dest.z) - dir * landOffset;
        Vector3 air = new Vector3(land.x, up.y, land.z);
        yield return MoveTo(up, air, travelTime);
        yield return MoveTo(air, land, diveTime);

        foreach (Health hp in FindTargets(landRadius)) hp.TakeDamage(a.damage);
        if (a.vfxPrefab) Destroy(Instantiate(a.vfxPrefab, transform.position, Quaternion.identity), 2f);

        cc.enabled = true;
        yield return new WaitForSeconds(0.4f);
    }

    IEnumerator MoveTo(Vector3 from, Vector3 to, float time)
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime / time)
        {
            transform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        transform.position = to;
    }

    // ---------- Helper: opponents within a radius, each counted once ----------
    List<Health> FindTargets(float radius)
    {
        var found = new List<Health>();
        foreach (Collider c in Physics.OverlapSphere(transform.position, radius, targetMask))
        {
            Health hp = c.GetComponentInParent<Health>();
            if (hp && hp != myHealth && !hp.IsDead && !found.Contains(hp)) found.Add(hp);
        }
        return found;
    }
}