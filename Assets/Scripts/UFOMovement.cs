using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UFOMovement : MonoBehaviour
{
    [SerializeField] float ufoSpeed;
    [SerializeField] bool randomAttacking;

    Vector2 movementDirection = Vector2.zero;
    Rigidbody2D rigidbody_2D;

    bool isPaused = false;

    GameObject target;

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

    }

    void FixedUpdate()
    {
        if(!isPaused)
        {
            Vector2 targetVelocity = movementDirection * ufoSpeed;
            rigidbody_2D.velocity = targetVelocity;
        }    
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
