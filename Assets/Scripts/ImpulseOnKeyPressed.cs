using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseOnKeyPress : MonoBehaviour
{
    public float impulseForce = 10f; // Fuerza del impulso
    public KeyCode activationKey = KeyCode.E; // Tecla para activar el impulso
    public Camera playerCamera; // Referencia a la cámara del jugador

    private bool isPlayerInRange = false; // Verifica si el jugador está en rango
    private Rigidbody playerRigidbody; // Referencia al Rigidbody del jugador

    private void OnTriggerEnter(Collider other)
    {
        // Comprueba si el objeto que entra en el trigger es el jugador y tiene un Rigidbody
        if (other.CompareTag("Player"))
        {
            playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                isPlayerInRange = true; // El jugador está en rango
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Cuando el jugador sale del rango del trigger
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerRigidbody = null;
        }
    }

    private void Update()
    {
        // Comprueba si el jugador está en rango y si se presiona la tecla
        if (isPlayerInRange && Input.GetKeyDown(activationKey))
        {
            ApplyImpulse();
        }
    }

    private void ApplyImpulse()
    {
        if (playerRigidbody != null && playerCamera != null)
        {
            // Usa la dirección de la cámara para calcular el impulso
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0; // Ignora la componente vertical para que sea solo en el plano horizontal
            cameraForward.Normalize();

            playerRigidbody.AddForce(cameraForward * impulseForce, ForceMode.Impulse);
        }
    }
}