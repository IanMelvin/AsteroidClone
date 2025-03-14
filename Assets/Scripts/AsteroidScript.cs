using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class AsteroidScript : MonoBehaviour
{
    public static Action<AsteroidScript> OnAsteroidBroken;
    public static Action<AsteroidScript> OnAsteroidBorn;
    [SerializeField] Sprite[] asteroidSprites;
    [SerializeField] GameObject asteroidNextSizeDown;
    [SerializeField] float speed;

    Rigidbody2D rigidbody_2D;
    Vector2 movementDirection = Vector2.zero;

    bool isPaused = false;

    private void OnEnable()
    {
        PlayerMovement_Retro.OnPauseMenuActive += SetPauseState;
    }

    private void OnDisable()
    {
        PlayerMovement_Retro.OnPauseMenuActive -= SetPauseState;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile") || collision.CompareTag("Player"))
        {
            if (asteroidNextSizeDown != null)
            {
                Instantiate(asteroidNextSizeDown, transform.position + new Vector3(Random.Range(0.0f, 0.2f), Random.Range(0.0f, 0.3f), 0.0f), Quaternion.identity)
                    .GetComponent<AsteroidScript>()
                    .SetMovementDirection(GetMovementDirection() + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)));
                Instantiate(asteroidNextSizeDown, transform.position + new Vector3(Random.Range(0.0f, 0.2f), Random.Range(0.0f, 0.3f), 0.0f), Quaternion.identity)
                    .GetComponent<AsteroidScript>()
                    .SetMovementDirection(GetMovementDirection() + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)));
            }
            OnAsteroidBroken?.Invoke(this);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        OnAsteroidBorn?.Invoke(this);

        if (movementDirection == Vector2.zero)
        {
            movementDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        }

        SetAsteroidSprite(Random.Range(0, asteroidSprites.Length));

        rigidbody_2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(!isPaused)
        {
            Vector2 targetVelocity = movementDirection * speed;
            rigidbody_2D.velocity = targetVelocity;
        }
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
    }

    public Vector2 GetMovementDirection()
    {
        return movementDirection;
    }
}
