using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZiplineController : MonoBehaviour
{
    [Header("Zipline Settings")]
    public Transform startPoint; // Punto inicial de la tirolesa
    public Transform endPoint;   // Punto final de la tirolesa
    public float ziplineSpeed = 5f; // Velocidad del transporte
    public KeyCode activationKey = KeyCode.E; // Tecla para activar la tirolesa

    private bool isUsingZipline = false;
    private Transform player;
    private Vector3 direction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }

    private void Update()
    {
        if (player != null && !isUsingZipline && Input.GetKeyDown(activationKey))
        {
            StartZipline();
        }

        if (isUsingZipline)
        {
            MovePlayerAlongZipline();
        }
    }

    private void StartZipline()
    {
        isUsingZipline = true;
        direction = (endPoint.position - startPoint.position).normalized;
        player.position = startPoint.position; // Asegurarse de que el jugador esté en el punto inicial
    }

    private void MovePlayerAlongZipline()
    {
        if (player == null) return;

        player.position += direction * ziplineSpeed * Time.deltaTime;

        // Verificar si el jugador ha llegado al final
        if (Vector3.Distance(player.position, endPoint.position) < 0.1f)
        {
            EndZipline();
        }
    }

    private void EndZipline()
    {
        isUsingZipline = false;
        player = null; // Liberar la referencia del jugador
    }

    private void OnDrawGizmos()
    {
        // Dibuja una línea para visualizar la tirolesa en el editor
        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
        }
    }
}