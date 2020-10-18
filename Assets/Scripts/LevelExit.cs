using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float delayBeforeNextLevel = 1.0f;
    [SerializeField] GameObject levelCompleteEffect = null;

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
        GameObject levelCompleteEffectSpawn = Instantiate(levelCompleteEffect, transform.position, transform.rotation);
        yield return new WaitForSeconds(delayBeforeNextLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
