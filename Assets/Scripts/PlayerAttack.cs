using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float firingDelay;
    [SerializeField] private GameObject projectilePrefab;

    Rigidbody2D rigidbody_2D;
    Transform projectileSpawn;
    bool canFire = true;
    bool isPaused = false;

    private void OnEnable()
    {
        PlayerMovement.OnPauseMenuActive += SetPauseState;
        PlayerMovement_Retro.OnPauseMenuActive += SetPauseState;
    }

    private void OnDisable()
    {
        PlayerMovement.OnPauseMenuActive -= SetPauseState;
        PlayerMovement_Retro.OnPauseMenuActive -= SetPauseState;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody_2D = GetComponent<Rigidbody2D>();
        projectileSpawn = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        if (context.phase != InputActionPhase.Canceled && canFire) StartCoroutine("Fire");
    }

    IEnumerator Fire()
    {
        canFire = false;
        ProjectileScript projectile = Instantiate(projectilePrefab, projectileSpawn.position, transform.rotation).GetComponent<ProjectileScript>();
        projectile.SetDirection(new Vector2(Mathf.Sin((transform.rotation.eulerAngles.z + 90.0f) * Mathf.Deg2Rad), Mathf.Cos((transform.rotation.eulerAngles.z - 90.0f) * Mathf.Deg2Rad)));//rigidbody_2D.velocity.normalized);
        projectile.SetShooter(1);
        yield return new WaitForSeconds(firingDelay);
        canFire = true;
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }
}
