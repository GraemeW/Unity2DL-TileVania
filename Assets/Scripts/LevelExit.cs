using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float delayBeforeNextLevel = 1.0f;
    [SerializeField] GameObject levelCompleteEffect = null;
    [SerializeField] float levelCompleteSloMo = 0.4f;
    [SerializeField] AudioClip audioClip = null;
    [SerializeField] float levelCompleteSFXVolume = 0.3f;

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        Player player = otherCollider.gameObject.GetComponent<Player>();
        if (player != null)
        {
            StartCoroutine(GoToNextLevel());
        }
    }

    private IEnumerator GoToNextLevel()
    {
        Time.timeScale = levelCompleteSloMo;
        GameObject currentFireworksVFX = Instantiate(levelCompleteEffect, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, levelCompleteSFXVolume);
        yield return new WaitForSeconds(delayBeforeNextLevel);
        Destroy(currentFireworksVFX);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
