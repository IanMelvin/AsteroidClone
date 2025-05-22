using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileScript : MonoBehaviour
{
    public static Action<int> OnProjectilesDespawned;
    [SerializeField] float speed;
    [SerializeField] float damage;
    [SerializeField] float lifeSpan;

    Vector2 movementDirection;
    Vector2 prePauseVelocity;
    AudioSource projectileAudio;
    Rigidbody2D rigidbody_2D;
    int shooter = 0;
    bool isPaused = false;
    bool destructionTimerOn = false;
    float destructionTimer = 0.0f;

    private void OnEnable()
    {
        UniversalPauseManager.OnPauseStateChanged += SetPauseState;
    }

    private void OnDisable()
    {
        UniversalPauseManager.OnPauseStateChanged -= SetPauseState;
    }

    void Start()
    {
        rigidbody_2D = GetComponent<Rigidbody2D>();
        projectileAudio = GetComponent<AudioSource>();

        destructionTimer = lifeSpan;
        destructionTimerOn = true;
    }

    void FixedUpdate()
    {
        if (!isPaused)
        {
            prePauseVelocity = rigidbody_2D.velocity;
            //Debug.Log($"Projecile speed: {rigidbody_2D.velocity.magnitude}");

            if (destructionTimer > 0.0f) destructionTimer -= Time.deltaTime;
            else if (destructionTimerOn)
            {
                destructionTimerOn = false;
                OnProjectilesDespawned?.Invoke(shooter);
                Destroy(gameObject);
            }
        }
        else rigidbody_2D.velocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid") || collision.CompareTag("Saucer"))
        {
            destructionTimerOn = false;
            destructionTimer = 0.0f;
            OnProjectilesDespawned?.Invoke(shooter);
            Destroy(gameObject);
        }
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
        if (isPaused)
        {
            if (projectileAudio.isPlaying) projectileAudio?.Pause();
        }
        else
        {
            rigidbody_2D.velocity = prePauseVelocity;
            projectileAudio?.UnPause();
        }
    }

    public void SetDirection(Vector2 newMovementDirection)
    {
        movementDirection = newMovementDirection;
    }

    public Vector2 GetMovementDirection()
    {
        return movementDirection;
    }

    public void SetVelocityAndInitalForce(Vector2 baseVelocity, Vector2 initalForce)
    {
        if(!rigidbody_2D) rigidbody_2D = GetComponent<Rigidbody2D>();
        rigidbody_2D.velocity = baseVelocity;
        movementDirection = initalForce / speed;
        rigidbody_2D.AddForce(initalForce, ForceMode2D.Impulse);
    }

    public void SetShooter(int shooterIndex)
    {
        shooter = shooterIndex;
    }

    public int GetShooterIndex()
    {
        return shooter;
    }

    public float GetMaxProjectileSpeed()
    {
        return speed;
    }
}
