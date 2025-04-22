using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float firingDelay;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int maxNumActiveProjectiles = 4;

    Transform projectileSpawn;
    bool fireDelayElapsed = true;
    bool reachedMaxProjectiles = false;
    bool isPaused = false;
    bool isHyperspacing = false;
    int numActiveProjectiles = 0;

    private void OnEnable()
    {
        PlayerMovement.OnPauseMenuActive += SetPauseState;
        PlayerMovement_Retro.OnPauseMenuActive += SetPauseState;
        PlayerMovement_Retro.OnHyperspaceActive += SetPauseState;
        ProjectileScript.OnProjectilesDespawned += RemoveActiveProjectile;

        fireDelayElapsed = true;
        reachedMaxProjectiles = false;
    }

    private void OnDisable()
    {
        PlayerMovement.OnPauseMenuActive -= SetPauseState;
        PlayerMovement_Retro.OnPauseMenuActive -= SetPauseState;
        PlayerMovement_Retro.OnHyperspaceActive -= SetPauseState;
        ProjectileScript.OnProjectilesDespawned -= RemoveActiveProjectile;
    }

    // Start is called before the first frame update
    void Start()
    {
        projectileSpawn = transform.GetChild(0);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (isPaused || isHyperspacing) return;

        if (context.phase != InputActionPhase.Canceled && fireDelayElapsed && !reachedMaxProjectiles) StartCoroutine("Fire");
    }

    IEnumerator Fire()
    {
        fireDelayElapsed = false;
        ProjectileScript projectile = Instantiate(projectilePrefab, projectileSpawn.position, transform.rotation).GetComponent<ProjectileScript>();
        projectile.SetDirection(new Vector2(Mathf.Sin((transform.rotation.eulerAngles.z + 90.0f) * Mathf.Deg2Rad), Mathf.Cos((transform.rotation.eulerAngles.z - 90.0f) * Mathf.Deg2Rad)));
        projectile.SetShooter(1);
        numActiveProjectiles++;
        if (numActiveProjectiles >= maxNumActiveProjectiles) reachedMaxProjectiles = true;
        yield return new WaitForSeconds(firingDelay);
        fireDelayElapsed = true;
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }

    private void SetHyperspaceStatus(bool hyperspaceStatus)
    {
        isHyperspacing = hyperspaceStatus;
    }

    private void RemoveActiveProjectile()
    {
        numActiveProjectiles--;
        if (numActiveProjectiles < maxNumActiveProjectiles) reachedMaxProjectiles = false;
    }
}
