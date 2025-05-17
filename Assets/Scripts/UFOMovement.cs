using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UFOMovement : MonoBehaviour
{
    [SerializeField] float ufoSpeed;

    Vector2 movementDirection = Vector2.zero;
    Rigidbody2D rigidbody_2D;

    bool isPaused = false;

    private void OnEnable()
    {
        PlayerMovement_Retro.OnPauseMenuActive += SetPauseState;
    }

    private void OnDisable()
    {
        PlayerMovement_Retro.OnPauseMenuActive -= SetPauseState;
    }

    private void Start()
    {
        rigidbody_2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(!isPaused)
        {
            Vector2 targetVelocity = movementDirection * ufoSpeed;
            rigidbody_2D.velocity = targetVelocity;
        }
        else rigidbody_2D.velocity = Vector2.zero;
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }

    public void SetMovementDirection(Vector2 newMovementDirection)
    {
        movementDirection = newMovementDirection;
    }
}
