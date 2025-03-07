using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static Action<int> OnPlayerDeath;
    bool isOneHit = true;
    bool isDead = false;

    private void OnEnable()
    {
        isDead = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
        {
            if (isOneHit && !isDead)
            {
                Debug.Log("Dead");
                isDead = true;
                //Activate Death particles / sound here
                OnPlayerDeath?.Invoke(1);
                gameObject.SetActive(false);
            }
        }
    }
}
