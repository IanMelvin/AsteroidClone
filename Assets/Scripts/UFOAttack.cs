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
    Rigidbody2D rigidbody_2D;
    bool isPaused = false;
    bool accuracyImprovementMilestoneReached = false;
    bool firingDelayTimerOn = false;
    float firingDelayTimer = 0.0f;

    private void OnEnable()
    {
        UniversalPauseManager.OnPauseStateChanged += SetPauseState;
        ScoreManager.OnAccuracyImprovementMilestone += ImprovedAccuracyMilestoneReached;
        players = GameObject.FindGameObjectsWithTag("Player");
        rigidbody_2D = GetComponent<Rigidbody2D>();
        firingDelayTimer = firingDelay;
        firingDelayTimerOn = true;
    }

    private void OnDisable()
    {
        UniversalPauseManager.OnPauseStateChanged -= SetPauseState;
        ScoreManager.OnAccuracyImprovementMilestone -= ImprovedAccuracyMilestoneReached;
    }

    private void FixedUpdate()
    {
        if (!isPaused)
        {
            if (firingDelayTimer > 0.0f) firingDelayTimer -= Time.deltaTime;
            else if (firingDelayTimerOn)
            {
                firingDelayTimerOn = false;
                Fire();
            }
        }
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }

    void ImprovedAccuracyMilestoneReached()
    {
        accuracyImprovementMilestoneReached = true;
    }

    private void Fire()
    {
        Vector2 spawnPosition = transform.position;
        Vector2 direction;
        if (randomAttacking) direction = Random.insideUnitCircle.normalized;
        else
        {
            direction = players[Random.Range(0, players.Length)].transform.position - transform.position;
            if(!accuracyImprovementMilestoneReached) direction += new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            else direction += new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            direction = direction.normalized;
        }
        spawnPosition += direction * firingRadius;

        ProjectileScript projectile = Instantiate(projectilePrefab, spawnPosition, transform.rotation).GetComponent<ProjectileScript>();
        projectile.SetVelocityAndInitalForce(rigidbody_2D.velocity, direction * projectile.GetMaxProjectileSpeed());
        projectile.SetShooter(-1);

        firingDelayTimer = firingDelay;
        firingDelayTimerOn = true;
    }
}
