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

    private void OnEnable()
    {
        WaveSpawner.OnWaveStarted += StartMusic;
        WaveSpawner.OnWaveEnded += StopMusic;
        PlayerMovement_Retro.OnPauseMenuActive += SetPauseState;
        StartMusic();
    }
    private void OnDisable()
    {
        WaveSpawner.OnWaveStarted -= StartMusic;
        WaveSpawner.OnWaveEnded -= StopMusic;
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
        timeBetweenBeats = timeBetweenBeats <= 0.25f ? 0.25f : (1f - 0.0336134f * Mathf.Pow(elapsedTime, 0.793745f));//1-0.0336134x^{0.793745}
        Debug.Log($"{elapsedTime} : {timeBetweenBeats}");
    }

    void SetPauseState(bool pausedState)
    {
        isPaused = pausedState;
    }

    IEnumerator PlayMusic()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenBeats);
            if (!isPaused)
            {
                elapsedTime += timeBetweenBeats;
                if (audioSource.clip.name != beatOne.name)
                {
                    Debug.Log("Switching Clip");
                    audioSource.clip = beatOne;
                }
                else audioSource.clip = beatTwo;
                audioSource.Play();
                UpdateTimeBetweenBeats();
            }
        }
    }
}
