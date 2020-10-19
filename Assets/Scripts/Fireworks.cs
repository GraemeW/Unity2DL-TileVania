using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireworks : MonoBehaviour
{
    [SerializeField] GameObject fireworksVFX = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject currentFireworksVFX = Instantiate(fireworksVFX, transform.position, transform.rotation);
        Destroy(currentFireworksVFX, 1f);
    }
}
