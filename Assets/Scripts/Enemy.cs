using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    const string COLLISION_LAYER_GROUND = "Ground";

    // Tunables
    [SerializeField] float distanceThresholdToAttack = 1.0f;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float hopForce = 1.0f;

    // State
    bool queueHop = false;

    // Cached References
    Player player = null;
    Rigidbody2D enemyRigidbody2D = null;
    Collider2D enemyCollider2D = null;
    Animator animator = null;

    void Start()
    {
        player = FindObjectOfType<Player>();
        enemyRigidbody2D = GetComponent<Rigidbody2D>();
        enemyCollider2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckDistanceToPlayerAndQueueAttack();
    }

    private void CheckDistanceToPlayerAndQueueAttack()
    {
        float distanceToPlayer = Mathf.Abs((transform.position - player.transform.position).magnitude);
        if (distanceToPlayer < distanceThresholdToAttack)
        {
            queueHop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Flip();
    }

    private void Flip()
    {
        // Movement flip
        Vector2 velocity = new Vector2(-enemyRigidbody2D.velocity.x, enemyRigidbody2D.velocity.y);
        enemyRigidbody2D.velocity = velocity;
        // Animation flip
        Vector3 lookVector = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        transform.localScale = lookVector;
    }

    private void FixedUpdate()
    {
        Move();
        Hop();
    }

    private void Hop()
    {
        if (queueHop && enemyCollider2D.IsTouchingLayers(LayerMask.GetMask(COLLISION_LAYER_GROUND)))
        {
            float playerDirection = Mathf.Sign(player.transform.position.x - transform.position.x);
            float currentDirection = Mathf.Sign(enemyRigidbody2D.velocity.x);
            if (playerDirection != currentDirection)
            {
                Flip();
            }
            animator.SetTrigger("hop");
            Vector3 enemyToPlayerTrajectory = (player.transform.position - transform.position).normalized;
            enemyRigidbody2D.AddForce(enemyToPlayerTrajectory * hopForce);
            queueHop = false;
        }
    }

    private void Move()
    {
        Vector2 velocity = new Vector2(Mathf.Sign(enemyRigidbody2D.velocity.x) * moveSpeed * Time.deltaTime, enemyRigidbody2D.velocity.y);
        enemyRigidbody2D.velocity = velocity;
    }
}
