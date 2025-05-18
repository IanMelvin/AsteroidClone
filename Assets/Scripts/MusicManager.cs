using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip beatOne;
    [SerializeField] AudioClip beatTwo;

    float timeBetweenBeats = 1.0f;
    float elapsedTime = 0.0f;
    bool isPaused = false;
    int numPlayers = 0;
    int numDeadPlayers = 0;

    private void OnEnable()
    {
        WaveSpawner.OnWaveStarted += StartMusic;
        WaveSpawner.OnWaveEnded += StopMusic;
        PlayerHealth.OnPlayerDeath += CheckIfPauseMusic;
        HealthManager.OnPlayerRespawn += CheckIfUnPauseMusic;
        PlayerMovement_Retro.OnPauseMenuActive += SetPauseState;

        numPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
    }
    private void OnDisable()
    {
        WaveSpawner.OnWaveStarted -= StartMusic;
        WaveSpawner.OnWaveEnded -= StopMusic;
        PlayerHealth.OnPlayerDeath -= CheckIfPauseMusic;
        HealthManager.OnPlayerRespawn -= CheckIfUnPauseMusic;
        PlayerMovement_Retro.OnPauseMenuActive -= SetPauseState;
    }

    void StartMusic()
    {
        timeBetweenBeats = 1.0f;
        elapsedTime = 0.0f;
        StartCoroutine("PlayMusic");
    }

    void StopMusic()
    {
        StopAllCoroutines();
    }

    void UpdateTimeBetweenBeats()
    {
        timeBetweenBeats = timeBetweenBeats <= 0.25f ? 0.25f : (1f - 0.0336134f * Mathf.Pow(elapsedTime, 0.793745f));
    }

    void SetPauseState(bool pausedState)
    {
        isPaused = pausedState;
    }

    void CheckIfPauseMusic(int integer)
    {
        numDeadPlayers++;
        if (numDeadPlayers == numPlayers) isPaused = true;
    }

    void CheckIfUnPauseMusic(int integer)
    {
        numDeadPlayers--;
        isPaused = false;
    }

    IEnumerator PlayMusic()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenBeats);
            if (!isPaused)
            {
                elapsedTime += timeBetweenBeats;
                if (audioSource.clip.name != beatOne.name) audioSource.clip = beatOne;
                else audioSource.clip = beatTwo;
                audioSource.Play();
                UpdateTimeBetweenBeats();
            }
        }
    }
}
