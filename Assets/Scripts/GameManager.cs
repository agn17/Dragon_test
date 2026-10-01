using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool IsOver { get; private set; }

    [Header("References")]
    public Health player;
    public Health enemy;
    public GameObject winPanel;
    public Image panelBackground;     // the Image on WinPanel
    public TMP_Text resultTitle;      // VICTORY / DEFEAT
    public TMP_Text winText;          // "<Dragon name> Wins!"

    [Header("Look")]
    public Color victoryColor = new Color(0.1f, 0.35f, 0.1f, 0.8f);
    public Color defeatColor = new Color(0.4f, 0.05f, 0.05f, 0.8f);
    public AudioClip victorySfx;
    public AudioClip defeatSfx;

    [Header("Settings")]
    public float panelDelay = 1.5f;

    void Awake()
    {
        IsOver = false;
        Time.timeScale = 1f;
    }

    void Start()
    {
        if (winPanel) winPanel.SetActive(false);
        player.OnDied += End;
        enemy.OnDied += End;
    }

    void OnDestroy()
    {
        player.OnDied -= End;
        enemy.OnDied -= End;
    }

    void End(Health dead)
    {
        if (IsOver) return;
        IsOver = true;
        bool playerWon = dead == enemy;
        StartCoroutine(ShowResult(playerWon ? player : enemy, playerWon));
    }

    IEnumerator ShowResult(Health winner, bool playerWon)
    {
        yield return new WaitForSeconds(panelDelay);

        resultTitle.text = playerWon ? "VICTORY" : "DEFEAT";
        winText.text = winner.displayName + " Wins!";
        if (panelBackground) panelBackground.color = playerWon ? victoryColor : defeatColor;

        AudioClip clip = playerWon ? victorySfx : defeatSfx;
        if (clip) AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);

        winPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}