using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float rollSpeed = 8f;

    Vector3 velocity;
    bool isGrounded;
    bool isRolling = false;

    public float rollDuration = 0.6f;
    private float rollCooldown = 1f;
    private float lastRollTime = -1f;

    void Update()
    {
        // Chequear si está en el suelo
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Mantener el personaje en el suelo
        }

        // Movimiento normal
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f && !isRolling)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Rueda (Roll)
        if (Input.GetButtonDown("Roll") && Time.time - lastRollTime > rollCooldown && !isRolling)
        {
            StartCoroutine(Roll(direction));
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    IEnumerator Roll(Vector3 direction)
    {
        isRolling = true;
        lastRollTime = Time.time;

        Vector3 rollDirection = direction.magnitude >= 0.1f ? direction : transform.forward;
        float startTime = Time.time;

        while (Time.time < startTime + rollDuration)
        {
            controller.Move(rollDirection * rollSpeed * Time.deltaTime);
            yield return null;
        }

        isRolling = false;
    }
}
