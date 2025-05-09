using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileScript : MonoBehaviour
{
    public static Action OnProjectilesDespawned;

    [SerializeField] float speed;
    [SerializeField] float damage;
    [SerializeField] float lifeSpan;

    Vector2 movementDirection;
    Rigidbody2D rigidbody_2D;
    int shooter = 0;
    bool isPaused;

    private void OnEnable()
    {
        PlayerMovement_Retro.OnPauseMenuActive += SetPauseState;
    }

    private void OnDisable()
    {
        PlayerMovement_Retro.OnPauseMenuActive -= SetPauseState;
    }

    void Start()
    {
        rigidbody_2D = GetComponent<Rigidbody2D>();
        
        StartCoroutine("LifeSpan");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
        {
            StopAllCoroutines();
            OnProjectilesDespawned?.Invoke();
            Destroy(gameObject);
        }
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }

    IEnumerator LifeSpan()
    {
        yield return new WaitForSeconds(lifeSpan);
        OnProjectilesDespawned?.Invoke();
        Destroy(gameObject);
    }

    public void SetDirection(Vector2 newMovementDirection)
    {
        movementDirection = newMovementDirection;
    }

    public Vector2 GetMovementDirection()
    {
        return movementDirection;
    }

    public void SetShooter(int shooterIndex)
    {
        shooter = shooterIndex;
    }

    public int GetShooterIndex() { return shooter; }
}
