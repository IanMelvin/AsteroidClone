using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOHealth : MonoBehaviour
{
    public static Action OnSaucerDestroyed;
    [SerializeField] AudioClip explosionAudio;
    AudioSource audioSource;
    ParticleSystem pSystem;
    bool isOneHit = true;
    bool isPaused = false;

    private void OnEnable()
    {
        UniversalPauseManager.OnPauseStateChanged += SetPauseState;
        audioSource = GetComponent<AudioSource>();
        pSystem = GetComponent<ParticleSystem>();
    }

    private void OnDisable()
    {
        UniversalPauseManager.OnPauseStateChanged -= SetPauseState;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile") || collision.CompareTag("Player") || collision.CompareTag("Asteroid"))
        {
            if (isOneHit)
            {
                Debug.Log("Saucer Down");
                StartCoroutine(DestroySaucer());
            }
        }
    }

    void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
        if (isPaused)
        {
            if (audioSource.isPlaying) audioSource.Pause();
            if (pSystem.isPlaying) pSystem.Pause();
        }
        else
        {
            audioSource.UnPause();
            if (pSystem.isPaused) pSystem.Play();
        }
    }

    IEnumerator DestroySaucer()
    {
        OnSaucerDestroyed?.Invoke();
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<UFOAttack>().enabled = false;
        GetComponent<UFOMovement>().enabled = false;
        audioSource.clip = explosionAudio;
        audioSource?.Play();
        pSystem?.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(gameObject);
    }
}
