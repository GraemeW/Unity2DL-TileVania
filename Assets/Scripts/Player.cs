using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    const string COLLISION_LAYER_GROUND = "Ground";
    const string COLLISION_LAYER_LADDER = "Ladder";
    const string COLLISION_LAYER_HAZARDS = "Hazards";
    const string COLLISION_BASE_NAME = "Feet";

    // Tunables
    [Header("Movement Detail")]
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float maxVelocity = 1.0f;
    [SerializeField] float groundHaltFactor = 0.1f;
    [SerializeField] float directionFlipHaltFactor = 0.1f;
    [SerializeField] float moveSpeedInAir = 1.0f;
    [Header("Climbing Detail")]
    [SerializeField] float climbSpeed = 1.0f;
    [SerializeField] float climbLateralVelocityThrottle = 0.1f;
    [SerializeField] float climbDownVelocityFloor = -1.0f;
    [Header("Jumping Detail")]
    [SerializeField] float jumpForce = 1.0f;
    [Header("Misc")]
    float maxAnimationSpeed = 3.0f;
    [SerializeField] float deathExplosionForce = 100f;
    [SerializeField] float delayBeforeGameOverScreen = 3.0f;
    [Header("Audio")]
    [SerializeField] AudioClip audioJump = null;
    [SerializeField] float audioJumpVolume = 0.4f;
    [SerializeField] AudioClip audioClimbing = null;
    [SerializeField] float timeBetweenLadderAudioSFX = 0.5f;
    [SerializeField] AudioClip audioDeath = null;
    [SerializeField] float audioDeathVolume = 0.6f;

    // State
    float playerSpeedDefault = 1.0f;
    float animationSpeedDefault = 1.0f;
    int lastLookDirection = 1;
    bool isJumpPressed = false;
    bool isClimbing = false;
    bool climbingAudioQueued = false;
    bool isAlive = true;

    // Cached references
    float horizontal;
    float vertical;
    Rigidbody2D playerRigidbody2D = null;
    Collider2D playerCollider2D = null;
    Collider2D playerFootCollider2D = null;
    Animator animator = null;
    GameSession gameSession = null;
    HeadsUpDisplayController headsUpDisplayController = null;
    AudioSource audioSource = null;

    private void Start()
    {
        playerRigidbody2D = GetComponent<Rigidbody2D>();
        playerCollider2D = GetComponent<Collider2D>();
        playerFootCollider2D = transform.Find(COLLISION_BASE_NAME).GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        playerSpeedDefault = moveSpeed;
        animationSpeedDefault = animator.speed;
        gameSession = FindObjectOfType<GameSession>();
        headsUpDisplayController = FindObjectOfType<HeadsUpDisplayController>();
        audioSource = GetComponent<AudioSource>();
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

        // Clamping to speed limits
        Vector2 cappedVelocity = new Vector2(Mathf.Clamp(playerRigidbody2D.velocity.x, -maxVelocity, maxVelocity), Mathf.Clamp(playerRigidbody2D.velocity.y, -maxVelocity, maxVelocity));
        playerRigidbody2D.velocity = cappedVelocity;
    }

    private void FixedUpdate()
    {
        if (!isAlive) { return; }
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
            Vector3 lookVector = new Vector3(Mathf.Sign(horizontal) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            transform.localScale = lookVector;
        }
    }

    private void Move()
    {
        float deltaX = horizontal * Time.deltaTime * moveSpeed;
        VelocityHaltOnDirectionChange(deltaX);

        if (!Mathf.Approximately(deltaX, 0f))
        {
            animator.SetBool("isRunning", true);
            Vector2 velocityIncrement = new Vector2(deltaX, 0f);
            playerRigidbody2D.velocity += velocityIncrement;
        }
        else
        {
            animator.SetBool("isRunning", false);
            SlowPlayerForNoInput();
        }
        AdjustAnimationSpeed();
    }

    private void VelocityHaltOnDirectionChange(float deltaX)
    {
        int currentLookDirection = (int)Mathf.Sign(deltaX);
        if (currentLookDirection != lastLookDirection)
        {
            Vector2 directionChangeHalt = new Vector2(playerRigidbody2D.velocity.x * directionFlipHaltFactor, playerRigidbody2D.velocity.y);
            playerRigidbody2D.velocity = directionChangeHalt;
        }
        lastLookDirection = currentLookDirection;
    }

    private void SlowPlayerForNoInput()
    {
        if (playerFootCollider2D.IsTouchingLayers(LayerMask.GetMask(COLLISION_LAYER_GROUND)))
        {
            Vector2 haltVelocity = new Vector2(playerRigidbody2D.velocity.x * groundHaltFactor, playerRigidbody2D.velocity.y);
            playerRigidbody2D.velocity = haltVelocity;
        }
    }

    private void AdjustAnimationSpeed()
    {
        float velocityDependentAnimationSpeed = animationSpeedDefault + (maxAnimationSpeed - animationSpeedDefault) * (Mathf.Abs(playerRigidbody2D.velocity.x) / maxVelocity);
        animator.speed = velocityDependentAnimationSpeed;
    }

    private void Jump()
    {
        if (isJumpPressed)
        {
            if (playerFootCollider2D.IsTouchingLayers(LayerMask.GetMask(COLLISION_LAYER_GROUND)))
            {
                Vector2 jumpVector = new Vector2(0f, jumpForce);
                playerRigidbody2D.AddForce(jumpVector);
                AudioSource.PlayClipAtPoint(audioJump, Camera.main.transform.position, audioJumpVolume);
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
        if (!isClimbing && !playerFootCollider2D.IsTouchingLayers(LayerMask.GetMask(COLLISION_LAYER_GROUND)))
        {
            Vector2 entryVelocity = new Vector2(climbLateralVelocityThrottle * playerRigidbody2D.velocity.x, 0f);
            playerRigidbody2D.velocity = entryVelocity;
        }

        // Only do climbing animation if not touching ground
        if (!playerFootCollider2D.IsTouchingLayers(LayerMask.GetMask(COLLISION_LAYER_GROUND)))
        {
            animator.SetBool("isClimbing", true);
        }
        else
        {
            animator.SetBool("isClimbing", false);
        }

        isClimbing = true;
        StartCoroutine(PlayLadderClimbSFX());
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

    private IEnumerator PlayLadderClimbSFX()
    {
        if (!climbingAudioQueued)
        {
            climbingAudioQueued = true;
            audioSource.clip = audioClimbing;
            audioSource.Play();
            yield return new WaitForSeconds(timeBetweenLadderAudioSFX);
            climbingAudioQueued = false;
        }
        else { yield break; }
    }

    private void OnCollisionEnter2D(Collision2D otherCollider)
    {
        Enemy enemy = otherCollider.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            TriggerDeath();
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.gameObject.layer == LayerMask.NameToLayer(COLLISION_LAYER_HAZARDS))
        {
            TriggerDeath();
        }
    }

    private void TriggerDeath()
    {
        if (isAlive)
        {
            isAlive = false;
            animator.SetTrigger("isDead");
            Vector2 deathExplosionVector = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(0.5f, 1.0f)) * deathExplosionForce;
            AudioSource.PlayClipAtPoint(audioDeath, Camera.main.transform.position, audioDeathVolume);
            playerRigidbody2D.AddForce(deathExplosionVector);
            StartCoroutine(TriggerGameOver());
        }
    }

    private IEnumerator TriggerGameOver()
    {
        yield return new WaitForSeconds(delayBeforeGameOverScreen);
        gameSession.ProcessPlayerDeath();
        if (headsUpDisplayController != null)
        {
            headsUpDisplayController.UpdateLivesText();
        }
    }

}
