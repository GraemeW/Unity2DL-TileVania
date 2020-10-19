using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersist : MonoBehaviour
{
    // States
    int startingSceneIndex = -1;

    private void Awake()
    {
        startingSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int numberOfScenePersists = FindObjectsOfType<ScenePersist>().Length;
        if (numberOfScenePersists > 1)
        {
            ScenePersist[] scenePersistArray = FindObjectsOfType<ScenePersist>();
            ScenePersist scenePersistA = scenePersistArray[0];
            ScenePersist scenePersistB = scenePersistArray[1];
            // Two scene persists exist -- we only want the higher one, which will be the more recent one
                // Cannot guarantee which one is higher by pulling via FindObjects, so manually check
            if (scenePersistA.GetStartingSceneIndex() > scenePersistB.GetStartingSceneIndex())
            {
                scenePersistB.gameObject.SetActive(false);
                Destroy(scenePersistB.gameObject);
                DontDestroyOnLoad(gameObject);
            }
            else if (scenePersistB.GetStartingSceneIndex() > scenePersistA.GetStartingSceneIndex())
            {
                scenePersistA.gameObject.SetActive(false);
                Destroy(scenePersistA.gameObject);
                DontDestroyOnLoad(gameObject);
            }
            else
            // If both scene persists have same index, then keep the existing one (destroy current game object)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public int GetStartingSceneIndex()
    {
        return startingSceneIndex;
    }
}
