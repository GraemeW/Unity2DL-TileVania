using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    const string COLLISION_LAYER_GROUND = "Ground";
    const string COLLISION_LAYER_LADDER = "Ladder";

    // Tunables
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float moveSpeedInAir = 1.0f;
    [SerializeField] float climbSpeed = 1.0f;
    [SerializeField] float climbLateralVelocityThrottle = 0.1f;
    [SerializeField] float climbDownVelocityFloor = -1.0f;
    [SerializeField] float jumpForce = 1.0f;

    // State
    float playerSpeedDefault = 1.0f;
    bool isJumpPressed = false;
    bool isClimbing = false;
    bool isAlive = true;

    // Cached references
    float horizontal;
    float vertical;
    Rigidbody2D playerRigidbody2D = null;
    Collider2D playerCollider2D = null;
    Animator animator = null;

    private void Start()
    {
        playerRigidbody2D = GetComponent<Rigidbody2D>();
        playerCollider2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        playerSpeedDefault = moveSpeed;
    }

    private void Update()
    {
        GetUserInput();
        SpeedCheck();
    }

    private void GetUserInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpPressed = true;
        }
    }

    private void SpeedCheck()
    {
        // Adjust speed if in air vs. on ground
        if (!playerCollider2D.IsTouchingLayers(LayerMask.GetMask(COLLISION_LAYER_GROUND)))
        {
            moveSpeed = moveSpeedInAir;
        }
        else
        {
            moveSpeed = playerSpeedDefault;
        }
    }

    private void FixedUpdate()
    {
        FlipPlayerLeftRight();
        Move();
        Jump();
        ClimbLadder();
    }

    private void FlipPlayerLeftRight()
    {
        bool playerStationary = Mathf.Approximately(Mathf.Abs(playerRigidbody2D.velocity.x), 0f);
        if (!playerStationary)
        {
            Vector3 lookVector = new Vector3(Mathf.Sign(playerRigidbody2D.velocity.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            transform.localScale = lookVector;
        }
    }

    private void Move()
    {
        float deltaX = horizontal * Time.deltaTime * moveSpeed;
        if (!Mathf.Approximately(deltaX, 0f))
        {
            animator.SetBool("isRunning", true);
            Vector2 velocityIncrement = new Vector2(deltaX, 0f);
            playerRigidbody2D.velocity += velocityIncrement;
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void Jump()
    {
        if (isJumpPressed)
        {
            if (playerCollider2D.IsTouchingLayers(LayerMask.GetMask(COLLISION_LAYER_GROUND)))
            {
                Vector2 jumpVector = new Vector2(0f, jumpForce);
                playerRigidbody2D.AddForce(jumpVector);
            }
            isJumpPressed = false;
        }

    }

    private void ClimbLadder()
    {
        if (playerCollider2D.IsTouchingLayers(LayerMask.GetMask(COLLISION_LAYER_LADDER)))
        {
            ClimbEntry();
            float deltaY = vertical * Time.deltaTime * climbSpeed;
            if (deltaY > 0)
            {
                ClimbUp(deltaY);
            }
            else
            {
                ClimbDown(deltaY);
            }
        }
        else
        {
            animator.SetBool("isClimbing", false);
            isClimbing = false;
        }
    }
    private void ClimbEntry()
    {
        // Initial speed throttling on first collision to ladder (if in the air)
        if (!isClimbing && !playerCollider2D.IsTouchingLayers(LayerMask.GetMask(COLLISION_LAYER_GROUND)))
        {
            playerRigidbody2D.velocity = climbLateralVelocityThrottle * playerRigidbody2D.velocity;
        }

        // Only do climbing animation if not touching ground
        if (!playerCollider2D.IsTouchingLayers(LayerMask.GetMask(COLLISION_LAYER_GROUND)))
        {
            animator.SetBool("isClimbing", true);
        }
        else
        {
            animator.SetBool("isClimbing", false);
        }

        isClimbing = true;
    }

    private void ClimbUp(float deltaY)
    {
        Vector2 velocityIncrement = new Vector2(0f, deltaY);
        playerRigidbody2D.velocity += velocityIncrement;
    }

    private void ClimbDown(float deltaY)
    {
        Vector2 climbDownVelocity = new Vector2(playerRigidbody2D.velocity.x, Mathf.Clamp(playerRigidbody2D.velocity.y + deltaY, climbDownVelocityFloor, 0f));
        playerRigidbody2D.velocity = climbDownVelocity;
    }

}
