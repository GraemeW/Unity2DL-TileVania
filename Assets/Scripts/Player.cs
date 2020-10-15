using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    // Tunables
    [SerializeField] float playerSpeed = 1.0f;
    [SerializeField] float jumpForce = 1.0f;
    float jumpThresholdSpeed = 0.1f;

    // State
    float horizontal;
    float vertical;
    bool isJumpPressed = false;

    // Cached references
    Rigidbody2D playerRigidbody2D = null;
    Animator animator = null;

    private void Start()
    {
        playerRigidbody2D = GetComponent<Rigidbody2D>();
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
            Vector2 updatedVelocity = new Vector2(deltaX, playerRigidbody2D.velocity.y);
            playerRigidbody2D.velocity = updatedVelocity;
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
            //if (Mathf.Approximately(playerRigidbody2D.velocity.y,0f)) // -- too sensitive
            if (playerRigidbody2D.velocity.y < jumpThresholdSpeed)
            {
                Vector2 jumpVector = new Vector2(0f, jumpForce);
                playerRigidbody2D.AddForce(jumpVector);
            }
            isJumpPressed = false;
        }

    }
}
