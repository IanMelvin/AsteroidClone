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

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        pSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    IEnumerator DestroySaucer()
    {
        OnSaucerDestroyed?.Invoke();
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        audioSource.clip = explosionAudio;
        audioSource.Play();
        pSystem.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(gameObject);
    }
}
