using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DragonFeedback : MonoBehaviour
{
    public DamagePopUp popupPrefab;
    public Vector3 popupOffset = new Vector3(0f, 3f, 0f);
    public Color flashColor = Color.white;
    public float flashTime = 0.1f;
    public float hitAnimCooldown = 0.8f;   // stops fire ticks from locking the dragon in the Hit animation
    public AudioClip hitSfx;

    static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    Health health;
    AbilityRunner runner;
    Animator anim;
    AudioSource audioSrc;
    Renderer[] renderers;
    MaterialPropertyBlock block;
    Coroutine flashRoutine;
    float lastHitAnim = -99f;

    void Awake()
    {
        health = GetComponent<Health>();
        runner = GetComponent<AbilityRunner>();
        anim = GetComponentInChildren<Animator>();
        audioSrc = GetComponent<AudioSource>();
        renderers = GetComponentsInChildren<Renderer>();
        block = new MaterialPropertyBlock();
    }

    void OnEnable() { health.OnDamaged += HandleDamaged; health.OnDied += HandleDied; }
    void OnDisable() { health.OnDamaged -= HandleDamaged; health.OnDied -= HandleDied; }

    void HandleDamaged(float amount)
    {
        if (popupPrefab)
        {
            Vector3 jitter = new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
            Instantiate(popupPrefab, transform.position + popupOffset + jitter, Quaternion.identity).Init(amount);
        }

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(Flash());

        if (hitSfx && audioSrc) audioSrc.PlayOneShot(hitSfx);

        // Only play the Hit animation if the dragon isn't mid-ability and isn't dead.
        if (!health.IsDead && !runner.IsBusy && Time.time - lastHitAnim > hitAnimCooldown)
        {
            lastHitAnim = Time.time;
            anim.SetTrigger("Hit");
        }
    }

    void HandleDied(Health h) { anim.SetTrigger("Die"); }

    IEnumerator Flash()
    {
        block.SetColor(BaseColor, flashColor);
        foreach (Renderer r in renderers) r.SetPropertyBlock(block);
        yield return new WaitForSeconds(flashTime);
        foreach (Renderer r in renderers) r.SetPropertyBlock(null);   // back to the normal material
    }
}