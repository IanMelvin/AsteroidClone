using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerMovement_Retro : MonoBehaviour
{
    public static Action OnPauseButtonPressed;
    public static Action<int, bool> OnHyperspaceActive;

    [SerializeField] private float playerSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite movingSprite;
    [SerializeField] private AudioSource thrustAudio;

    Vector2 inputMovement;
    Vector2 prePauseVelocity;
    int rotationDirection = 0;
    float smoothTime = 0.5f;
    Vector3 velocity = Vector3.zero;
    Rigidbody2D rigidbody_2D;
    SpriteRenderer spriteRenderer;

    float cameraDistance;
    Vector3 bottomLeft;
    Vector3 topLeft;
    Vector3 bottomRight;
    Camera playerCam;

    bool isPaused = false;
    bool isMoving = false;
    bool isHyperspacing = false;
    bool isDead = false;

    int playerIndex = 1;

    Coroutine changeSpritesCoroutine;

    private void OnEnable()
    {
        UniversalPauseManager.OnPauseStateChanged += SetPauseState;
        HealthManager.OnPlayerRespawn += Respawn;
        PlayerHealth.OnPlayerDeath += SetDeathState;
    }

    private void OnDisable()
    {
        UniversalPauseManager.OnPauseStateChanged -= SetPauseState;
        HealthManager.OnPlayerRespawn -= Respawn;
        PlayerHealth.OnPlayerDeath -= SetDeathState;
        if(changeSpritesCoroutine != null) StopCoroutine(changeSpritesCoroutine);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody_2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCam = Camera.main;
        if (!thrustAudio) thrustAudio = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (!isPaused && !isHyperspacing && !isDead)
        {
            if (isMoving) inputMovement = new Vector2(Mathf.Sin((transform.rotation.eulerAngles.z + 90.0f) * Mathf.Deg2Rad), Mathf.Cos((transform.rotation.eulerAngles.z - 90.0f) * Mathf.Deg2Rad));

            Vector3 targetVelocity = inputMovement * playerSpeed;
            rigidbody_2D.velocity = Vector3.SmoothDamp(rigidbody_2D.velocity, targetVelocity, ref velocity, smoothTime);
            prePauseVelocity = rigidbody_2D.velocity;

            transform.Rotate(new Vector3(0, 0, turnSpeed * rotationDirection));
        }
        else if (isHyperspacing)
        {
            StopAllMovement();
        }
        else rigidbody_2D.velocity = Vector2.zero;
    }

    private void DefineScreenValues()
    {
        cameraDistance = playerCam.nearClipPlane;

        bottomLeft = playerCam.ScreenToWorldPoint(new Vector3(0, 0, cameraDistance));
        topLeft = playerCam.ScreenToWorldPoint(new Vector3(0, Screen.height, cameraDistance));
        bottomRight = playerCam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cameraDistance));
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        if ((isPaused || isHyperspacing || isDead) && context.phase == InputActionPhase.Started) return;

        if (context.phase == InputActionPhase.Started)
        {
            smoothTime = 1f;
            changeSpritesCoroutine = StartCoroutine(ChangeSprite());
            isMoving = true;
            thrustAudio.Play();
            thrustAudio.loop = true;
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            isMoving = false;
            inputMovement = Vector2.zero;
            smoothTime = 2f;
            if (changeSpritesCoroutine != null) StopCoroutine(changeSpritesCoroutine);
            spriteRenderer.sprite = defaultSprite;
            thrustAudio.Stop();
        }
    }

    public void OnLeftRotation(InputAction.CallbackContext context)
    {
        if ((isPaused || isHyperspacing || isDead) && context.phase == InputActionPhase.Started) return;

        if (context.phase == InputActionPhase.Started) rotationDirection = 1;
        else if (context.phase == InputActionPhase.Canceled) rotationDirection = 0;
    }

    public void OnRightRotation(InputAction.CallbackContext context)
    {
        if ((isPaused || isHyperspacing || isDead) && context.phase == InputActionPhase.Started) return;

        if (context.phase == InputActionPhase.Started) rotationDirection = -1;
        else if (context.phase == InputActionPhase.Canceled) rotationDirection = 0;
    }

    public void OnPaused()
    {
        OnPauseButtonPressed?.Invoke();
    }

    public void OnHyperspace(InputAction.CallbackContext context)
    {
        if ((isPaused || isHyperspacing || isDead) && context.phase == InputActionPhase.Started) return;

        if (context.phase == InputActionPhase.Started)
        {
            OnHyperspaceActive?.Invoke(playerIndex, true);
            isHyperspacing = true;
            StartCoroutine(HyperspaceAbility());
        }
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
        if(!isPaused) rigidbody_2D.velocity = prePauseVelocity;
        if(isPaused && isMoving) thrustAudio.Pause();
        if(!isPaused && isMoving) thrustAudio.UnPause();
    }

    private void SetDeathState(int playerIndex)
    {
        if(this.playerIndex == playerIndex)
        {
            isDead = true;
            StopAllMovement();
        }
    }

    private void Respawn (int playerIndex)
    {
        if(this.playerIndex == playerIndex)
        {
            isDead = false;
            StopAllMovement();
        }
    }

    private void StopAllMovement()
    {
        isMoving = false;
        rigidbody_2D.velocity = Vector2.zero;
        inputMovement = Vector2.zero;
        prePauseVelocity = Vector2.zero;
        smoothTime = 0.0f;
        if (changeSpritesCoroutine != null) StopCoroutine(changeSpritesCoroutine);
        spriteRenderer.sprite = defaultSprite;
        rotationDirection = 0;
        thrustAudio.Stop();
    }

    IEnumerator ChangeSprite()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.05f);
            if(!isPaused)
            {
                if (spriteRenderer.sprite == movingSprite) spriteRenderer.sprite = defaultSprite;
                else spriteRenderer.sprite = movingSprite;
            }
        }
    }

    IEnumerator HyperspaceAbility()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        float distanceFromEdge = -0.5f;
        DefineScreenValues();
        float x = Random.Range(bottomLeft.x - distanceFromEdge, bottomRight.x + distanceFromEdge);
        float y = Random.Range(bottomLeft.y - distanceFromEdge, topLeft.y + distanceFromEdge);

        yield return new WaitForSeconds(1.0f);
        transform.position = new Vector2(x, y);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        isHyperspacing = false;
        OnHyperspaceActive?.Invoke(playerIndex, false);
    }
}
