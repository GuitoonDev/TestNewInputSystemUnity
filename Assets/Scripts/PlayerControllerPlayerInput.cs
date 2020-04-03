using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControllerPlayerInput : MonoBehaviour
{
    private static readonly int animSpeed = Animator.StringToHash("speed");
    private static readonly int animVelocityY = Animator.StringToHash("velocityY");
    private static readonly int animIsSlashing = Animator.StringToHash("isSlashing");
    private static readonly int animJump = Animator.StringToHash("jump");
    private static readonly int animIsOnGround = Animator.StringToHash("isOnGround");

    private Animator animator;
    private new Rigidbody2D rigidbody;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 250f;
    [SerializeField] private float jumpForce = 700f;

    [Header("Collisions")]
    [SerializeField] private LayerMask groundLayer = default(LayerMask);
    [SerializeField] private float collisionRadius = 0.1f;
    [SerializeField] private Vector2 groundOffset = Vector2.zero;

    private float directionX;
    private bool isJumpButtonPressed;
    private bool isSlashButtonWasPressedThisFrame;
    private bool isJumpButtonPressedThisFrame;

    private bool isSlashing;
    private bool isOnGround;
    private bool isFacingLeft;
    private bool isGroundDetectionDisabled;

    private void OnDrawGizmos() {
        if (!isGroundDetectionDisabled) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere((Vector2) transform.position + groundOffset, collisionRadius);
            Gizmos.color = Color.white;
        }
    }

    private void Awake() {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (isSlashButtonWasPressedThisFrame && isOnGround) {
            isSlashing = true;
        }

        if (isSlashing) {
            isJumpButtonPressedThisFrame = false;
        }

        if (directionX > 0 && isFacingLeft || directionX < 0 && !isFacingLeft) {
            isFacingLeft = !isFacingLeft;
            transform.Rotate(0f, 180f, 0f);
        }

        if (isOnGround && isJumpButtonPressedThisFrame) {
            isOnGround = false;
            isJumpButtonPressed = isJumpButtonPressed || isJumpButtonPressedThisFrame;
            StartCoroutine(DisableGroundDetection());
            animator.SetTrigger(animJump);
        }

        animator.SetBool(animIsSlashing, isSlashing);
        animator.SetBool(animIsOnGround, isOnGround);
        animator.SetFloat(animSpeed, Mathf.Abs(rigidbody.velocity.x));
        animator.SetFloat(animVelocityY, rigidbody.velocity.y);
    }

    private void FixedUpdate() {
        Gamepad currentGamepad = Gamepad.current;

        float currentDirectionX = isSlashing ? 0 : directionX;
        float velocityX = currentDirectionX * moveSpeed * Time.fixedDeltaTime;
        float velocityY = rigidbody.velocity.y;

        if (isJumpButtonPressed && !isOnGround) {
            velocityY += jumpForce * Time.fixedDeltaTime;
        }

        if (!isGroundDetectionDisabled && !isOnGround && Physics2D.OverlapCircle((Vector2) transform.position + groundOffset, collisionRadius, groundLayer)) {
            isOnGround = true;
        }

        isJumpButtonPressed = false;

        rigidbody.velocity = new Vector2(velocityX, velocityY);
    }

    private IEnumerator DisableGroundDetection() {
        isGroundDetectionDisabled = true;
        yield return new WaitForSeconds(0.2f);
        isGroundDetectionDisabled = false;
    }

    // Player Input Unity Event callbacks
    /////////////////////////
    // IL FAUT LIRE ICI !! //
    public void OnJump(InputAction.CallbackContext _context) {
        if (_context.started) {
            Debug.Log("OnJump : _context.performed");
        }

        isJumpButtonPressedThisFrame = _context.performed;

        Debug.Log("isJumpButtonPressedThisFrame : " + isJumpButtonPressedThisFrame);
    }

    public void OnMove(InputAction.CallbackContext _context) {
        directionX = _context.ReadValue<float>();
    }

    public void OnSlash(InputAction.CallbackContext _context) {
        isSlashButtonWasPressedThisFrame = _context.performed;
    }

    public void OnDebug(InputAction.CallbackContext _context) {
        if (_context.started) {
            Debug.Log("OnDebug : _context.started");
        }

        if (_context.performed) {
            Debug.Log("OnDebug : _context.performed");
        }

        if (_context.canceled) {
            Debug.Log("OnDebug : _context.canceled");
        }
    }
    // STOP, C'EST FINI !!  //
    //////////////////////////

    // Animation event callback functions
    private void EndSlashing() {
        isSlashing = false;
    }
}
