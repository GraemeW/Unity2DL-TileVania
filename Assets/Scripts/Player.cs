using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    const string COLLISION_LAYER_GROUND = "Ground";

    // Tunables
    [SerializeField] float playerSpeed = 1.0f;
    [SerializeField] float jumpForce = 1.0f;

    // State
    bool isJumpPressed = false;
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
    }

    private void Update()
    {
        GetUserInput();
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

    private void FixedUpdate()
    {
        FlipPlayerLeftRight();
        PlayerMove();
        PlayerJump();
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

    private void PlayerMove()
    {
        float deltaX = horizontal * Time.deltaTime * playerSpeed;
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

    private void PlayerJump()
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
}
