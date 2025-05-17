using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOAttack : MonoBehaviour
{
    [SerializeField] private float firingDelay = 0.75f;
    [SerializeField] private float firingRadius = 3f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] bool randomAttacking;

    GameObject[] players;
    bool isPaused = false;

    private void OnEnable()
    {
        PlayerMovement_Retro.OnPauseMenuActive += SetPauseState;
        players = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine("Fire");
    }

    private void OnDisable()
    {
        PlayerMovement_Retro.OnPauseMenuActive -= SetPauseState;
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }

    IEnumerator Fire()
    {
        while (true)
        {
            yield return new WaitForSeconds(firingDelay);
            if (!isPaused)
            {
                Vector2 spawnPosition = transform.position;
                Vector2 direction;
                if (randomAttacking) direction = Random.insideUnitCircle.normalized;
                else
                {
                    direction = players[Random.Range(0, players.Length)].transform.position - transform.position;
                    direction += new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
                    direction = direction.normalized;
                }
                spawnPosition += direction * firingRadius;

                ProjectileScript projectile = Instantiate(projectilePrefab, spawnPosition, transform.rotation).GetComponent<ProjectileScript>();
                projectile.SetDirection(direction);
                projectile.SetShooter(-1);
            }
        }
    }
}
