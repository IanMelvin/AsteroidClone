using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileScript : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float damage;
    [SerializeField] float lifeSpan;

    Vector2 movementDirection;
    Rigidbody2D rigidbody_2D;
    int shooter = 0;

    void Start()
    {
        rigidbody_2D = GetComponent<Rigidbody2D>();
        
        StartCoroutine("LifeSpan");
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = movementDirection * speed;
        rigidbody_2D.velocity = targetVelocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    IEnumerator LifeSpan()
    {
        yield return new WaitForSeconds(lifeSpan);
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
