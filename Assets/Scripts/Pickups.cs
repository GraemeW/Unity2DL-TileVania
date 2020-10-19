using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    // Tunables
    [SerializeField] int pickupValue = 5;
    [SerializeField] AudioClip audioClip = null;
    [SerializeField] float audioClipVolume = 0.2f;

    // State
    bool isActive = true;

    // Cached References
    GameSession gameSession = null;
    Animator animator = null;
    HeadsUpDisplayController headsUpDisplayController = null;

    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        headsUpDisplayController = FindObjectOfType<HeadsUpDisplayController>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            isActive = false;
            animator.SetTrigger("pickedUp");
            gameSession.AddToScore(pickupValue);
            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, audioClipVolume);
            headsUpDisplayController.UpdateScoreText();
            Destroy(gameObject, 0.2f);
        }

    }
}
