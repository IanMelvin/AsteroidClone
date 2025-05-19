using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float firingDelay;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int maxNumActiveProjectiles = 4;

    Rigidbody2D rigidbody_2D;
    Transform projectileSpawn;
    bool fireDelayElapsed = true;
    bool reachedMaxProjectiles = false;
    bool isPaused = false;
    bool isHyperspacing = false;
    bool isDead = false;
    int numActiveProjectiles = 0;

    int playerIndex = 1;

    private void OnEnable()
    {
        HealthManager.OnPlayerRespawn += Respawn;
        PlayerMovement.OnPauseMenuActive += SetPauseState;
        UniversalPauseManager.OnPauseStateChanged += SetPauseState;
        PlayerMovement_Retro.OnHyperspaceActive += SetHyperspaceStatus;
        PlayerHealth.OnPlayerDeath += ToggleDeathState;
        ProjectileScript.OnProjectilesDespawned += RemoveActiveProjectile;

        fireDelayElapsed = true;
        reachedMaxProjectiles = false;
    }

    private void OnDisable()
    {
        HealthManager.OnPlayerRespawn -= Respawn;
        PlayerMovement.OnPauseMenuActive -= SetPauseState;
        UniversalPauseManager.OnPauseStateChanged -= SetPauseState;
        PlayerMovement_Retro.OnHyperspaceActive -= SetHyperspaceStatus;
        PlayerHealth.OnPlayerDeath -= ToggleDeathState;
        ProjectileScript.OnProjectilesDespawned -= RemoveActiveProjectile;

        numActiveProjectiles = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        projectileSpawn = transform.GetChild(0);
        rigidbody_2D = GetComponent<Rigidbody2D>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (isPaused || isHyperspacing || isDead) return;

        if (context.phase != InputActionPhase.Canceled && fireDelayElapsed && !reachedMaxProjectiles) StartCoroutine("Fire");
    }

    IEnumerator Fire()
    {
        fireDelayElapsed = false;
        ProjectileScript projectile = Instantiate(projectilePrefab, projectileSpawn.position, transform.rotation).GetComponent<ProjectileScript>();
        
        Vector2 baseDirection = new Vector2(Mathf.Sin((transform.rotation.eulerAngles.z + 90.0f) * Mathf.Deg2Rad), Mathf.Cos((transform.rotation.eulerAngles.z - 90.0f) * Mathf.Deg2Rad)).normalized;
        baseDirection *= projectile.GetMaxProjectileSpeed();
        projectile.SetVelocityAndInitalForce(rigidbody_2D.velocity, baseDirection);

        //projectile.SetDirection(baseDirection);
        projectile.SetShooter(playerIndex);
        numActiveProjectiles++;
        if (numActiveProjectiles >= maxNumActiveProjectiles) reachedMaxProjectiles = true;
        yield return new WaitForSeconds(firingDelay);
        fireDelayElapsed = true;
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }

    private void SetHyperspaceStatus(int playerIndex, bool hyperspaceStatus)
    {
        if(this.playerIndex == playerIndex) isHyperspacing = hyperspaceStatus;
    }

    private void RemoveActiveProjectile(int shooterIndex)
    {
        if(shooterIndex == playerIndex)
        {
            numActiveProjectiles--;
            if (numActiveProjectiles < maxNumActiveProjectiles) reachedMaxProjectiles = false;
        }
    }

    private void ToggleDeathState(int playerIndex)
    {
        if (this.playerIndex == playerIndex)
        {
            isDead = true;
        }
    }

    private void Respawn(int playerIndex)
    {
        if (this.playerIndex == playerIndex)
        {
            isDead = false;
        }
    }
}
