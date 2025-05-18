using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static Action<int> OnPlayerDeath;
    AudioSource explosionAudio;
    ParticleSystem pSystem;
    bool isOneHit = true;
    bool isDead = false;
    bool isPaused = false;

    int playerIndex = 1;

    private void OnEnable()
    {
        HealthManager.OnPlayerRespawn += Respawn;
        PlayerMovement_Retro.OnPauseMenuActive += SetPauseState;
        isDead = false;
        explosionAudio = GetComponent<AudioSource>();
        pSystem = GetComponent<ParticleSystem>();
    }

    private void OnDisable()
    {
        HealthManager.OnPlayerRespawn -= Respawn;
        PlayerMovement_Retro.OnPauseMenuActive -= SetPauseState;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid") || collision.CompareTag("Projectile") || collision.CompareTag("Player") || collision.CompareTag("Saucer"))
        {
            if (isOneHit && !isDead)
            {
                Debug.Log("Dead");
                isDead = true;
                //Activate Death particles / sound here
                explosionAudio?.Play();
                pSystem?.Play();
                OnPlayerDeath?.Invoke(playerIndex);
                //gameObject.SetActive(false);
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<PolygonCollider2D>().enabled = false;
                //gameObject.GetComponent<PlayerMovement_Retro>().enabled = false;
            }
        }
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
        if (isPaused)
        {
            if (explosionAudio.isPlaying) explosionAudio.Pause();
            if (pSystem.isPlaying) pSystem.Pause();
        }
        else
        {
            explosionAudio.UnPause();
            if (pSystem.isPaused) pSystem.Play();
        }
    }

    public void Respawn(int playerIndex)
    {
        if(this.playerIndex == playerIndex)
        {
            isDead = false;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        }
    }
}
