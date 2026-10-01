using UnityEngine;

[CreateAssetMenu(menuName = "Dragon/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    public float damage = 10f;
    public float cooldown = 3f;
    public float range = 4f;
    public string animTrigger;      // must match an Animator trigger exactly
    public GameObject vfxPrefab;    // optional for now
    public AudioClip sfx;           // optional for now
}