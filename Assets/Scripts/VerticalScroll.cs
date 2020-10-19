using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalScroll : MonoBehaviour
{
    // Tunables
    [Tooltip ("Number of rate entries must match delays")]
    [SerializeField] float[] scrollRates = { 0.3f, 0.5f, 0.8f, 1.0f };
    [Tooltip("Number of delay entries must match rates")]
    [SerializeField] float[] scrollDelays = { 5.0f, 5.0f, 10.0f, 20.0f }; 

    // Cached References
    Rigidbody2D myRigidbody2D = null;

    private void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        StartCoroutine(BeginScrolling());
    }

    private IEnumerator BeginScrolling()
    {
        for (int rateIndex = 0; rateIndex < scrollRates.Length; rateIndex++)
        {
            yield return new WaitForSeconds(scrollDelays[rateIndex]);
            Vector2 scrollVelocityVector = new Vector2(0f, scrollRates[rateIndex]);
            myRigidbody2D.velocity = scrollVelocityVector;
        }
    }
}
