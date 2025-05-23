using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthManager : MonoBehaviour
{
    public static Action<int> OnPlayerRespawn;
    public static Action OnGameOver;

    [SerializeField] TextMeshProUGUI[] playerLifeText;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] Image[] playerLifeIcon;

    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject player3;
    [SerializeField] GameObject player4;

    [SerializeField] int defaultNumLives = 3;
    [SerializeField] float respawnDelay = 2f;
    [SerializeField] float spawnRadiusSafeZone = 2f;
    [SerializeField] AudioSource extraLifeAudio;

    WaveSpawner spawner;
    int[] playerLives;
    bool isPaused = false;
    bool extraLifeAudioOn = false;
    bool respawnTimerOn = false;
    bool gameOverTimerOn = false;
    bool objectsInRespawnRadius = false;
    float extraLifeAudioTimer = 0.0f;
    float respawnTimer = 0.0f;
    float gameOverTimer = 0.0f;
    float respawnTimeKeeper = 0.0f;

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += RemoveALife;
        MenuManager.OnGameStarted += StartGame;
        ScoreManager.OnExtraLifeAwarded += AwardExtraLife;
        UniversalPauseManager.OnPauseStateChanged += SetPauseState;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= RemoveALife;
        MenuManager.OnGameStarted -= StartGame;
        ScoreManager.OnExtraLifeAwarded -= AwardExtraLife;
        UniversalPauseManager.OnPauseStateChanged -= SetPauseState;
    }

    private void Start()
    {
        playerLives = new int[] { -1, -1, -1, -1 };
        spawner = GetComponent<WaveSpawner>();
        gameOverText.enabled = false;
        if (!extraLifeAudio) extraLifeAudio = GetComponent<AudioSource>();

        /*if (player1) UpdateLifeUI(1);
        if (player2) UpdateLifeUI(2);
        if (player3) UpdateLifeUI(3);
        if (player4) UpdateLifeUI(4);*/
    }

    void FixedUpdate()
    {
        if (!isPaused)
        {
            if (extraLifeAudioTimer > 0.0f) extraLifeAudioTimer -= Time.deltaTime;
            else if (extraLifeAudioOn)
            {
                extraLifeAudioOn = false;
                extraLifeAudio.Stop();
            }

            if (respawnTimer > 0.0f) respawnTimer -= Time.deltaTime;
            else if (respawnTimerOn)
            {
                respawnTimerOn = false;
                RespawnDelay(1);
            }

            if (gameOverTimer > 0.0f) gameOverTimer -= Time.deltaTime;
            else if (gameOverTimerOn)
            {
                gameOverText.enabled = false;
                gameOverTimerOn = false;
                playerLifeText[0].enabled = false;
                playerLifeIcon[0].enabled = false;
                OnGameOver?.Invoke();
            }
        }
    }

    private void StartGame()
    {
        playerLives = new int[] { -1, -1, -1, -1 };
        playerLifeText[0].enabled = true;
        playerLifeIcon[0].enabled = true;
        playerLives[0] = defaultNumLives;
        UpdateLifeUI(1);
        respawnTimer = 2f;
        respawnTimerOn = true;
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
        if (isPaused && extraLifeAudioOn) extraLifeAudio.Pause();
        if (!isPaused && extraLifeAudioOn) extraLifeAudio.UnPause();
    }

    void UpdateLifeUI(int playerIndex)
    {
        if (playerLives[playerIndex - 1] < 0) return;

        playerLifeText[playerIndex - 1].text = $"x {playerLives[playerIndex - 1]}";
    }

    private void RespawnDelay(int playerIndex)
    {
        objectsInRespawnRadius = false;
        List<AsteroidScript> asteroids = spawner.GetAsteroids();
        for (int i = 0; i < asteroids.Count; i++)
        {
            if ((Vector3.zero - asteroids[i].transform.position).magnitude < spawnRadiusSafeZone)
            {
                objectsInRespawnRadius = true;
                break;
            }
        }
        
        if (objectsInRespawnRadius)
        {
            respawnTimer = 1f;
            respawnTimerOn = true;
            respawnTimeKeeper += respawnTimer;
            return;
        }

        Debug.Log($"Respawn: {respawnTimeKeeper} seconds");
        OnPlayerRespawn?.Invoke(playerIndex);
    }

    void AwardExtraLife(int playerIndex)
    {
        PlayExtraLifeAudio();
        playerLives[playerIndex - 1]++;
        UpdateLifeUI(playerIndex);
    }

    void RemoveALife(int playerIndex)
    {
        playerLives[playerIndex - 1]--;
        UpdateLifeUI(playerIndex);

        if (CheckForGameOver())
        {
            Debug.Log("GameOver");
            gameOverTimer = 2f;
            gameOverTimerOn = true;
            gameOverText.enabled = true;
        }
        if (playerLives[playerIndex - 1] <= 0)
        {
            Debug.Log("Out of lives");
        }
        else
        {
            respawnTimer = respawnDelay;
            respawnTimerOn = true;
            respawnTimeKeeper += respawnTimer;
        }
    }

    private bool CheckForGameOver()
    {
        bool isGameOver = true;
        for (int i = 0; i < playerLives.Length; i++)
        {
            if (playerLives[i] > 0)
            {
                isGameOver = false;
                break;
            }
        }
        return isGameOver;
    }

    private void PlayExtraLifeAudio()
    {
        extraLifeAudioOn = true;
        extraLifeAudio.Play();
        extraLifeAudio.loop = true;
        extraLifeAudioTimer = extraLifeAudio.clip.length * 12f;
    }
}
