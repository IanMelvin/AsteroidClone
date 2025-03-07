using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement_Retro : MonoBehaviour
{
    public static Action<bool> OnPauseMenuActive;

    [SerializeField] private float playerSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite movingSprite;

    Vector2 inputMovement;
    int rotationDirection = 0;
    float smoothTime = 0.5f;
    Vector3 velocity = Vector3.zero;
    Rigidbody2D rigidbody_2D;
    SpriteRenderer spriteRenderer;

    bool isPaused = false;
    bool isMoving = false;

    int playerIndex = 1;

    private void OnEnable()
    {
        OnPauseMenuActive += SetPauseState;
        PlayerHealth.OnPlayerDeath += ToggleDeathState;
    }

    private void OnDisable()
    {
        OnPauseMenuActive -= SetPauseState;
        PlayerHealth.OnPlayerDeath -= ToggleDeathState;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody_2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!isPaused)
        {
            if(isMoving) inputMovement = new Vector2(Mathf.Sin((transform.rotation.eulerAngles.z + 90.0f) * Mathf.Deg2Rad), Mathf.Cos((transform.rotation.eulerAngles.z - 90.0f) * Mathf.Deg2Rad));

            Vector3 targetVelocity = inputMovement * playerSpeed;
            rigidbody_2D.velocity = Vector3.SmoothDamp(rigidbody_2D.velocity, targetVelocity, ref velocity, smoothTime);

            //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + (turnSpeed * rotationDirection)));
            transform.Rotate(new Vector3(0, 0, turnSpeed * rotationDirection));
        }
    }


    public void OnForward(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        if(context.phase == InputActionPhase.Started)
        {
            smoothTime = 0.5f;
            StartCoroutine(ChangeSprite());
        }

        if (context.phase != InputActionPhase.Canceled)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
            inputMovement = Vector2.zero;
            smoothTime = 1.5f;
            StopAllCoroutines();
            spriteRenderer.sprite = defaultSprite;
        }
    }

    public void OnLeftRotation(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        if (context.phase != InputActionPhase.Canceled) rotationDirection = 1;
        else rotationDirection = 0;
    }

    public void OnRightRotation(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        if (context.phase != InputActionPhase.Canceled) rotationDirection = -1;
        else rotationDirection = 0;
    }

    public void OnPaused()
    {
        OnPauseMenuActive?.Invoke(!isPaused);
    }

    public void OnHyperspace(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        if (context.phase != InputActionPhase.Canceled) Debug.Log("Hyperspace");
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }

    private void ToggleDeathState(int playerIndex)
    {
        if(this.playerIndex == playerIndex)
        {
            velocity = Vector3.zero;
        }
    }

    IEnumerator ChangeSprite()
    {
        while(!isPaused)
        {
            yield return new WaitForSeconds(0.05f);
            if (spriteRenderer.sprite == movingSprite) spriteRenderer.sprite = defaultSprite;
            else spriteRenderer.sprite = movingSprite;
        }
    }
}
