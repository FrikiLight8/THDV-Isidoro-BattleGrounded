using UnityEngine;

public class TeleportBlock : MonoBehaviour
{
    public Transform teleportDestination;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected, teleporting...");
            TeleportPlayer(other);
        }
    }
    private void TeleportPlayer(Collider player)
    {
        if (teleportDestination != null)
        {
            // Desactivar el CharacterController temporalmente
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false; // Desactiva el CharacterController
                player.transform.position = teleportDestination.position; // Teletransporta
                controller.enabled = true; // Reactiva el CharacterController
            }

            Debug.Log("Player teleported to: " + teleportDestination.position);
            Debug.Log("Current player position: " + player.transform.position);
        }
        else
        {
            Debug.LogError("Teleport destination is not assigned!");
        }
    }
}