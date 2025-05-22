using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UFOMovement : MonoBehaviour
{
    [SerializeField] float ufoSpeed;
    [SerializeField] float timeBetweenDirectionChecks = 0.75f;

    Vector2 movementDirection = Vector2.zero;
    Rigidbody2D rigidbody_2D;

    bool isPaused = false;
    bool movementTimerOn = false;
    float movementTimer = 0.0f;

    private void OnEnable()
    {
        UniversalPauseManager.OnPauseStateChanged += SetPauseState;
        rigidbody_2D = GetComponent<Rigidbody2D>();
        if (movementDirection == Vector2.zero) movementDirection = new Vector2(transform.position.x > 0 ? -1 : 1,0);
        movementTimer = timeBetweenDirectionChecks;
        movementTimerOn = true;
    }

    private void OnDisable()
    {
        UniversalPauseManager.OnPauseStateChanged -= SetPauseState;
    }

    void FixedUpdate()
    {
        if(!isPaused)
        {
            Vector2 targetVelocity = movementDirection * ufoSpeed;
            rigidbody_2D.velocity = targetVelocity;

            if (movementTimer > 0.0f) movementTimer -= Time.deltaTime;
            else if (movementTimerOn)
            {
                movementTimerOn = false;
                AdjustMovementDirection();
            }
        }
        else rigidbody_2D.velocity = Vector2.zero;

        CheckIfDead();
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }

    public void SetMovementDirection(Vector2 newMovementDirection)
    {
        movementDirection = newMovementDirection;
    }

    private void CheckIfDead()
    {
        if (!isPaused) 
        {
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            if ((screenPosition.x >= Screen.width + 20 && rigidbody_2D.velocity.x > 0) || (screenPosition.x <= -20 && rigidbody_2D.velocity.x < 0))
            {
                UFOHealth.OnSaucerDestroyed?.Invoke();
                Destroy(gameObject);
            }
        }
    }

    private void AdjustMovementDirection()
    {
        movementDirection = new Vector2(movementDirection.x, Random.Range(-1, 2));
        movementTimer = timeBetweenDirectionChecks;
        movementTimerOn = true;    
    }
}
