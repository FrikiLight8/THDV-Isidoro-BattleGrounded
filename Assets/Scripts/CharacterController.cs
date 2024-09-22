using UnityEngine;
using System.Collections;

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
    public float rollHeight = 0.5f; // Altura reducida durante el roll
    private float originalHeight; // Altura original del Character Controller

    // Variables para Wall Run
    public float wallRunSpeed = 8f;
    public float wallRunDuration = 2f; // Duración máxima del wall run
    public float wallRunGravity = -2f;
    public float wallDetectionDistance = 1.5f;
    public LayerMask wallLayer; // Qué capas se consideran paredes
    private bool isWallRunning = false;
    private Vector3 wallNormal;
    private float wallRunTimer = 0f;
    private bool canWallRun = true; // Controla si puede volver a pegarse a una pared

    // Variables para Wall Jump
    public float wallJumpForce = 8f;
    private bool isNearWall = false; // Verifica si estamos cerca de una pared
    private bool canWallJump = false;
    private Vector3 lastWallNormal = Vector3.zero; // Última normal de la pared tocada


    // Cambiar color del personaje
    private Renderer playerRenderer;
    private Color originalColor;

    void Start()
    {
        originalHeight = controller.height; // Guardamos la altura original del Character Controller
        playerRenderer = GetComponent<Renderer>(); // Obtenemos el renderer del personaje
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color; // Guardamos el color original del personaje
        }
    }

    void Update()
    {
        // Verificamos si el personaje está en el suelo
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Mantener el personaje en el suelo

            // Resetear la normal de la última pared al tocar el suelo
            lastWallNormal = Vector3.zero;
        }

        // Movimiento normal
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f && !isRolling && !isWallRunning)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // Salto y Wall Jump
        if (Input.GetKey(KeyCode.Space))
        {
            if (isGrounded)
            {
                // Salto normal
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

        }
        if (Input.GetKey(KeyCode.Space) && !isGrounded)
        {
            if (canWallJump)
            {
                // Wall Jump
                WallJump();
            }
        }
        // Rodar (Roll)
        if (Input.GetButtonDown("Roll") && Time.time - lastRollTime > rollCooldown && !isRolling)
        {
            StartCoroutine(Roll(direction));
        }

        // Aplicar gravedad
        if (!isWallRunning)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            // Gravedad reducida durante el Wall Run
            velocity.y += wallRunGravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        // Limitar la duración del Wall Run
        if (isWallRunning)
        {
            wallRunTimer += Time.deltaTime;
            if (wallRunTimer >= wallRunDuration || isGrounded)
            {
                StopWallRun(); // Detener el Wall Run si dura demasiado o toca el suelo
            }
        }

        // Detectar paredes para Wall Run y Wall Jump
        CheckWallRunOrJump();

        // Mover al personaje
        controller.Move(velocity * Time.deltaTime);
    }

    IEnumerator Roll(Vector3 direction)
    {
        isRolling = true;
        lastRollTime = Time.time;

        // Reducir la altura del Character Controller
        controller.height = rollHeight;

        Vector3 rollDirection;

        // Si el jugador se está moviendo (usa WASD)
        if (direction.magnitude >= 0.1f)
        {
            // Convertir la dirección de movimiento del jugador a la perspectiva de la cámara
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // El roll se hará en la dirección en la que el jugador se está moviendo según la cámara
            rollDirection = moveDirection.normalized;
        }
        else
        {
            // Si no hay movimiento, el roll se hace hacia la dirección de la cámara
            rollDirection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        }

        float startTime = Time.time;
        while (Time.time < startTime + rollDuration)
        {
            controller.Move(rollDirection * rollSpeed * Time.deltaTime);
            yield return null;
        }

        // Volver a la altura original
        controller.height = originalHeight;

        isRolling = false;
    }



    void CheckWallRunOrJump()
    {
        RaycastHit hit;

        // Detectar pared a la derecha
        if (Physics.Raycast(transform.position, transform.right, out hit, wallDetectionDistance, wallLayer))
        {
            wallNormal = hit.normal;

            // Si la pared actual es distinta de la última pared tocada, permitir Wall Jump y resetear lastWallNormal
            if (wallNormal != lastWallNormal)
            {
                lastWallNormal = wallNormal;
                canWallJump = true; // Permitir Wall Jump en esta nueva pared
            }

            isNearWall = true;
            if (!isGrounded && Input.GetAxisRaw("Vertical") > 0 && Input.GetKey(KeyCode.LeftShift))
            {
                StartWallRun(hit.normal);
            }
        }
        // Detectar pared a la izquierda
        else if (Physics.Raycast(transform.position, -transform.right, out hit, wallDetectionDistance, wallLayer))
        {
            wallNormal = hit.normal;

            // Si la pared actual es distinta de la última pared tocada, permitir Wall Jump y resetear lastWallNormal
            if (wallNormal != lastWallNormal)
            {
                lastWallNormal = wallNormal;
                canWallJump = true; // Permitir Wall Jump en esta nueva pared
            }

            isNearWall = true;
            if (!isGrounded && Input.GetAxisRaw("Vertical") > 0 && Input.GetKey(KeyCode.LeftShift))
            {
                StartWallRun(hit.normal);
            }
        }
        else
        {
            isNearWall = false;
            canWallJump = false;

            // Si el personaje está en el suelo, detener el Wall Run
            if (isGrounded)
            {
                StopWallRun();
            }
        }
    }

    void StartWallRun(Vector3 normal)
    {
        if (!isWallRunning)
        {
            isWallRunning = true;
            wallRunTimer = 0f; // Reiniciar el temporizador del Wall Run
            wallNormal = normal; // Guardar la normal de la pared

            // Cambiar el color del personaje a azul (Wall Run)
            if (playerRenderer != null)
            {
                playerRenderer.material.color = Color.blue;
            }
        }

        // Movimiento en paralelo a la pared
        Vector3 wallRunDirection = Vector3.Cross(wallNormal, Vector3.up);
        if (Vector3.Dot(transform.forward, wallRunDirection) < 0)
        {
            wallRunDirection = -wallRunDirection;
        }

        controller.Move(wallRunDirection * wallRunSpeed * Time.deltaTime);
    }

    void StopWallRun()
    {
        if (isWallRunning)
        {
            isWallRunning = false;
            canWallRun = false; // No permitir Wall Run hasta que vuelva al aire

            // Volver al color original
            if (playerRenderer != null)
            {
                playerRenderer.material.color = originalColor;
            }

            // Permitir Wall Run nuevamente al estar en el aire
            StartCoroutine(EnableWallRunAfterAirborne());
        }
    }


    IEnumerator EnableWallRunAfterAirborne()
    {
        // Esperar a que el personaje esté en el aire para habilitar el Wall Run de nuevo
        yield return new WaitUntil(() => !isGrounded);
        canWallRun = true;
    }

    void WallJump()
    {
        // Saltar en la dirección opuesta a la pared
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        Vector3 jumpDirection = wallNormal + Vector3.up; // Saltar hacia atrás y arriba
        controller.Move(jumpDirection * wallJumpForce * Time.deltaTime);

        // Cambiar el color del personaje a rojo (Wall Jump)
        if (playerRenderer != null)
        {
            playerRenderer.material.color = Color.red;
        }

        // Detener el Wall Run
        StopWallRun();
    }
}
