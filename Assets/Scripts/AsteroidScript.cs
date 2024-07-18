using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AsteroidScript : MonoBehaviour
{
    [SerializeField] GameObject asteroidNextSizeDown;
    [SerializeField] float speed;

    Rigidbody2D rigidbody_2D;
    Vector2 movementDirection = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            if (asteroidNextSizeDown != null)
            {
                Vector2 newDirection = GetMovementDirection() + (collision.gameObject.GetComponent<ProjectileScript>().GetMovementDirection() / 2.0f);
                Vector2 newDirection2 = GetMovementDirection() + (collision.gameObject.GetComponent<ProjectileScript>().GetMovementDirection() / 1.5f);
                Instantiate(asteroidNextSizeDown, transform.position, Quaternion.identity).GetComponent<AsteroidScript>().SetMovementDirection(newDirection.normalized);
                Instantiate(asteroidNextSizeDown, transform.position, Quaternion.identity).GetComponent<AsteroidScript>().SetMovementDirection(newDirection2.normalized);
            }
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(movementDirection == Vector2.zero)
        {
            movementDirection = new Vector2(Random.Range(-1.0f,1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        }

        rigidbody_2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = movementDirection * speed;
        rigidbody_2D.velocity = targetVelocity;
    }

    public void SetMovementDirection(Vector2 newMovementDirection)
    {
        movementDirection = newMovementDirection;
    }

    public Vector2 GetMovementDirection()
    {
        return movementDirection;
    }
}
