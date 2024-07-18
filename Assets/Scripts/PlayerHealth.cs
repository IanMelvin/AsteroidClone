using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    bool isOneHit = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Asteroid"))
        {
            if (isOneHit)
            {
                Debug.Log("Dead");
            }
        }
    }
}
