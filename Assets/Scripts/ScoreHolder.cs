using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHolder : MonoBehaviour
{
    public static Action<int, int> OnScoreSentOut;

    [SerializeField] int scoreValue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            SentOutScore(collision.gameObject.GetComponent<ProjectileScript>().GetShooterIndex());
        }
        if (collision.CompareTag("Player"))
        {
            SentOutScore(1);
        }
    }

    public void SentOutScore(int playerIndex)
    {
        OnScoreSentOut?.Invoke(scoreValue, playerIndex);
    }
}
