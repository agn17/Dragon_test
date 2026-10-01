using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIconUI : MonoBehaviour
{
    public AbilityRunner runner;
    public int index;
    public Image icon;
    public Image overlay;
    public TMP_Text cooldownText;

    void Start()
    {
        Sprite s = runner.abilities[index].icon;
        if (s) icon.sprite = s;
    }

    void Update()
    {
        float n = runner.CooldownNormalized(index);
        overlay.fillAmount = n;
        if (cooldownText)
            cooldownText.text = n > 0f ? runner.CooldownRemaining(index).ToString("0.0") : "";
    }
}