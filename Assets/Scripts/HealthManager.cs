using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HealthManager : MonoBehaviour
{
    public static Action<int> OnPlayerRespawn;

    [SerializeField] TextMeshProUGUI[] playerLifeText;
    //[SerializeField] TextMeshProUGUI player2LifeText;
    //[SerializeField] TextMeshProUGUI player3LifeText;
    //[SerializeField] TextMeshProUGUI player4LifeText;

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
    bool objectsInRespawnRadius = false;
    float extraLifeAudioTimer = 0.0f;
    float respawnTimer = 0.0f;
    float respawnTimeKeeper = 0.0f;

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += RemoveALife;
        ScoreManager.OnExtraLifeAwarded += AwardExtraLife;
        UniversalPauseManager.OnPauseStateChanged += SetPauseState;
        playerLives = new int[] { defaultNumLives, defaultNumLives, defaultNumLives, defaultNumLives };
        spawner = GetComponent<WaveSpawner>();
        if (!extraLifeAudio) extraLifeAudio = GetComponent<AudioSource>();

        if (player1) UpdateLifeUI(1);
        if (player2) UpdateLifeUI(2);
        if (player3) UpdateLifeUI(3);
        if (player4) UpdateLifeUI(4);
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= RemoveALife;
        ScoreManager.OnExtraLifeAwarded -= AwardExtraLife;
        UniversalPauseManager.OnPauseStateChanged -= SetPauseState;
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
        }
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
        /*switch (playerIndex)
        {
            case 1:
                player1LifeText.text = $"x {playerLives[playerIndex-1]}";
                break;
            case 2:
                player2LifeText.text = $"x {playerLives[playerIndex-1]}";
                break;
            case 3:
                player3LifeText.text = $"x {playerLives[playerIndex - 1]}";
                break;
            case 4:
                player4LifeText.text = $"x {playerLives[playerIndex - 1]}";
                break;
        }*/
    }

    private void RespawnDelay(int playerIndex)
    {
        objectsInRespawnRadius = false;
        for (int i = 0; i < spawner.GetAsteroids().Count; i++)
        {
            if ((Vector3.zero - spawner.GetAsteroids()[i].transform.position).magnitude < spawnRadiusSafeZone)
            {
                objectsInRespawnRadius = true;
                break;
            }
        }
        
        if (objectsInRespawnRadius)
        {
            respawnTimer = respawnDelay / 2.0f;
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

        if (playerLives[playerIndex - 1] <= 0) Debug.Log("GameOver");
        else
        {
            respawnTimer = respawnDelay;
            respawnTimerOn = true;
            respawnTimeKeeper += respawnTimer;
        }
    }

    private void PlayExtraLifeAudio()
    {
        extraLifeAudioOn = true;
        extraLifeAudio.Play();
        extraLifeAudio.loop = true;
        extraLifeAudioTimer = extraLifeAudio.clip.length * 12f;
    }
}
