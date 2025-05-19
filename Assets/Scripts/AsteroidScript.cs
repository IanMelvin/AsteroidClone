using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D),typeof(AudioSource))]
public class AsteroidScript : MonoBehaviour
{
    public static Action<AsteroidScript> OnAsteroidBroken;
    public static Action<AsteroidScript> OnAsteroidBorn;
    [SerializeField] Sprite[] asteroidSprites;
    [SerializeField] GameObject asteroidNextSizeDown;
    [SerializeField] float speed;
    [SerializeField] private AudioSource explosionAudio;
    
    Rigidbody2D rigidbody_2D;
    ParticleSystem pSystem;
    Vector2 movementDirection = Vector2.zero;

    bool isPaused = false;

    private void OnEnable()
    {
        UniversalPauseManager.OnPauseStateChanged += SetPauseState;
    }

    private void OnDisable()
    {
        UniversalPauseManager.OnPauseStateChanged -= SetPauseState;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile") || collision.CompareTag("Player") || collision.CompareTag("Saucer"))
        {
            if (asteroidNextSizeDown != null)
            {
                float slope = (collision.transform.position.y - transform.position.y) / (collision.transform.position.x - transform.position.x);
                Vector2 crossProd = slope > 0f ?  Vector3.Cross(rigidbody_2D.velocity, collision.attachedRigidbody.velocity) : Vector3.Cross(collision.attachedRigidbody.velocity, rigidbody_2D.velocity);// + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                Instantiate(asteroidNextSizeDown, transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.3f, 0.3f), 0.0f), Quaternion.identity)
                    .GetComponent<AsteroidScript>()
                    .SetMovementDirection((crossProd + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f))).normalized * Random.Range(0.8f, 1.2f));//GetMovementDirection() + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)));
                Instantiate(asteroidNextSizeDown, transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.3f, 0.3f), 0.0f), Quaternion.identity)
                    .GetComponent<AsteroidScript>()
                    .SetMovementDirection((crossProd + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f))).normalized * Random.Range(0.8f, 1.2f));//GetMovementDirection() + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)));
            }
            StartCoroutine(DestroyAsteroid());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        OnAsteroidBorn?.Invoke(this);

        if (movementDirection == Vector2.zero) movementDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;

        SetAsteroidSprite(Random.Range(0, asteroidSprites.Length));

        rigidbody_2D = GetComponent<Rigidbody2D>();
        pSystem = GetComponent<ParticleSystem>();

        if (!explosionAudio) explosionAudio = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (!isPaused)
        {
            Vector2 targetVelocity = movementDirection * speed;
            rigidbody_2D.velocity = targetVelocity;
        }
        else rigidbody_2D.velocity = Vector2.zero;
    }

    public void InitializeAsteroid(Vector2 movementDirection, int spriteIndex)
    {
        SetMovementDirection(movementDirection);
        SetAsteroidSprite(spriteIndex);
    }

    public void SetMovementDirection(Vector2 newMovementDirection)
    {
        movementDirection = newMovementDirection;
    }

    void SetAsteroidSprite(int index)
    {
        GetComponent<SpriteRenderer>().sprite = asteroidSprites[index];
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

    public Vector2 GetMovementDirection()
    {
        return movementDirection;
    }

    IEnumerator DestroyAsteroid()
    {
        OnAsteroidBroken?.Invoke(this);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        explosionAudio?.Play();
        pSystem?.Play();
        yield return new WaitForSeconds(explosionAudio.clip.length);
        Destroy(gameObject);
    }
}
