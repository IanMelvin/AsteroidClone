using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class PlayerHealth : MonoBehaviour
{
    public static Action<int> OnPlayerDeath;
    AudioSource explosionAudio;
    ParticleSystem pSystem;
    bool isOneHit = true;
    bool isDead = false;

    private void OnEnable()
    {
        isDead = false;
        explosionAudio = GetComponent<AudioSource>();
        pSystem = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid") || collision.CompareTag("Projectile"))
        {
            if (isOneHit && !isDead)
            {
                Debug.Log("Dead");
                isDead = true;
                //Activate Death particles / sound here
                if (collision.CompareTag("Projectile")) explosionAudio?.Play();
                pSystem?.Play();
                OnPlayerDeath?.Invoke(1);
                //gameObject.SetActive(false);
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<PolygonCollider2D>().enabled = false;
                //gameObject.GetComponent<PlayerMovement_Retro>().enabled = false;
            }
        }
    }

    public void Respawn()
    {
        isDead = false;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        //gameObject.GetComponent<PlayerMovement_Retro>().enabled = true;
    }
}
