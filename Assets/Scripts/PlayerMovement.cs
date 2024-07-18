using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static Action<bool> OnPauseMenuActive;

    [SerializeField] private float playerSpeed;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite movingSprite;

    //float screenEdgeThreshold = .1f;
    Vector2 inputMovement;
    Vector3 previousLookVector = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    Rigidbody2D rigidbody_2D;
    SpriteRenderer spriteRenderer;

    bool isPaused = false;
    bool isMoving = false;

    private void OnEnable()
    {
        OnPauseMenuActive += SetPauseState;
    }

    private void OnDisable()
    {
        OnPauseMenuActive -= SetPauseState;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody_2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!isPaused)
        {
            if (inputMovement == Vector2.zero) isMoving = false;
            else isMoving = true;

            Vector3 targetVelocity = inputMovement * playerSpeed;
            rigidbody_2D.velocity = Vector3.SmoothDamp(rigidbody_2D.velocity, targetVelocity, ref velocity, 0.5f);

            if (rigidbody_2D.velocity != Vector2.zero)
            {
                previousLookVector = rigidbody_2D.velocity;
                Vector2 dir = rigidbody_2D.velocity;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                Vector2 dir = previousLookVector;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            if (isMoving) spriteRenderer.sprite = movingSprite;
            else spriteRenderer.sprite = defaultSprite;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        if(context.phase != InputActionPhase.Canceled) inputMovement = context.ReadValue<Vector2>();
        else inputMovement = Vector2.zero;
    }

    public void OnPaused()
    {
        OnPauseMenuActive?.Invoke(!isPaused);
    }

    private void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }
}
