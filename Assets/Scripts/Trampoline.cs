using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float bounceForce = 10f; // Fuerza del impulso hacia arriba

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica si el objeto tiene un Rigidbody
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Aplica una fuerza hacia arriba
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reinicia la velocidad vertical
            rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualización opcional en el editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}