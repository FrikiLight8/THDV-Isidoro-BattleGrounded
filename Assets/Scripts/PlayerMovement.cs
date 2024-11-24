// PlayerMovement
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    [Header("Assignables")]
    //Assignables
	public Transform playerCam;
	public Transform orientation;
	private Collider playerCollider;
	public Rigidbody rb;

    [SerializeField]
    private PlayerDataSO playerDataSO;

    [Space(10)]

	public LayerMask whatIsGround;
	public LayerMask whatIsWallrunnable;

    [Header("MovementSettings")]
    //Movement Settings 
	public bool grounded;
	public bool onWall;

    //Private Floats
    private float wallRunGravity = 5f;
	private float maxSlopeAngle = 35f;
	private float wallRunRotation;
    private float slideSlowdown = 0.2f;
	private float actualWallRotation;
	private float wallRotationVel;
	private float desiredX;
	private float xRotation;
	private float sensMultiplier = 1f;
	private float jumpCooldown = 0.1f;
	private float jumpForce = 550f;
    private float MAXjumpForce;
    private float x;
	private float y;
	private float vel;
	private float speedTimer;
	private float originalSpeed;

    //Private bools
	private bool readyToJump;
	private bool jumping;
	private bool sprinting;
    private bool crouching;
	private bool wallRunning;
    private bool cancelling;
	private bool readyToWallrun = true;
    private bool airborne;
    private bool onGround;
	private bool surfing;
	private bool cancellingGrounded;
	private bool cancellingWall;
	private bool cancellingSurf;
	private bool cancellMouseMovement;

    //Private Vector3's
	private Vector3 grapplePoint;
	private Vector3 normalVector;
	private Vector3 wallNormalVector;
	private Vector3 wallRunPos;
	private Vector3 previousLookdir;

    //Private int
	private int nw;

    // UI
    public UIManager uiManager;

    //Instance
    public static PlayerMovement Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		rb = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		playerCollider = GetComponent<Collider>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		readyToJump = true;
		wallNormalVector = Vector3.up;
		originalSpeed = playerDataSO.walkSpeed;
		MAXjumpForce = jumpForce;
	}

	private void LateUpdate()
	{
        //For wallrunning
	    WallRunning();
	}

	private void FixedUpdate()
	{
        //For moving
		Movement();
	}

	private void Update()
	{
        //Input
		MyInput();
        //Looking around
		Look();
		
		if (speedTimer > 0 )
		{
            speedTimer -= Time.deltaTime;
        }
		else 
		{
			playerDataSO.walkSpeed = originalSpeed;
        }
	}

	public int PlayerSpeed()
	{
		float mag = MathF.Abs(rb.velocity.sqrMagnitude);
		return (int)mag;
	}

    //Player input
	private void MyInput()
	{
		x = Input.GetAxisRaw("Horizontal");
		y = Input.GetAxisRaw("Vertical");
		jumping = Input.GetButton("Jump");
		crouching = Input.GetKey(KeyCode.LeftControl);
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			StartCrouch();
		}
		if (Input.GetKeyUp(KeyCode.LeftControl))
		{
			StopCrouch();
		}
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uiManager.isPaused)
            {
                uiManager.DesactivatePause();
            }
            else
            {
                uiManager.ActivatePause();
            }
        }
    }

    //Scale player down
	private void StartCrouch()
	{
		float num = 400f;
		base.transform.localScale = new Vector3(1f, 0.5f, 1f);
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 0.5f, base.transform.position.z);
		if (rb.velocity.magnitude > 0.1f && grounded)
		{
			rb.AddForce(orientation.transform.forward * num);
		}
	}

    //Scale player to original size
	private void StopCrouch()
	{
		base.transform.localScale = new Vector3(1f, 1.5f, 1f);
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 0.5f, base.transform.position.z);
	}

    //Moving around with WASD
	private void Movement()
	{
		rb.AddForce(Vector3.down * Time.deltaTime * 10f);
		Vector2 mag = FindVelRelativeToLook();
		float num = mag.x;
		float num2 = mag.y;
		CounterMovement(x, y, mag);
		if (readyToJump && jumping)
		{
			Jump();
		}
		float num3 = playerDataSO.walkSpeed;
		if (sprinting)
		{
			num3 = playerDataSO.runSpeed;
		}
		if (crouching && grounded && readyToJump)
		{
			rb.AddForce(Vector3.down * Time.deltaTime * 3000f);
			return;
		}
		if (x > 0f && num > num3)
		{
			x = 0f;
		}
		if (x < 0f && num < 0f - num3)
		{
			x = 0f;
		}
		if (y > 0f && num2 > num3)
		{
			y = 0f;
		}
		if (y < 0f && num2 < 0f - num3)
		{
			y = 0f;
		}
		float num4 = 1f;
		float num5 = 1f;
		if (!grounded)
		{
			num4 = 0.5f;
			num5 = 0.5f;
		}
		if (grounded && crouching)
		{
			num5 = 0f;
		}
		if (wallRunning)
		{
			num5 = 0.3f;
			num4 = 0.3f;
		}
		if (surfing)
		{
			num4 = 0.7f;
			num5 = 0.3f;
		}
		rb.AddForce(orientation.transform.forward * y * playerDataSO.moveSpeed * Time.deltaTime * num4 * num5);
		rb.AddForce(orientation.transform.right * x * playerDataSO.moveSpeed * Time.deltaTime * num4);
	}

	public void	IncreaseSpeed(float speed, float seconds)
	{
		speedTimer = seconds;
		playerDataSO.walkSpeed += speed;
	}

    //Ready to jump again
	private void ResetJump()
	{
		readyToJump = true;
		jumpForce = MAXjumpForce;
	}

    //Player go fly
	private void Jump()
	{
        if ((grounded || wallRunning || surfing) && readyToJump)
		{
		    MonoBehaviour.print("jumping");
		    Vector3 velocity = rb.velocity;
		    readyToJump = false;
		    rb.AddForce(Vector2.up * jumpForce * 1.5f);
		    rb.AddForce(normalVector * jumpForce * 0.5f);
		    if (rb.velocity.y < 0.5f)
		    {
			    rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
		    }
		    else if (rb.velocity.y > 0f)
		    {
			    rb.velocity = new Vector3(velocity.x, velocity.y / 2f, velocity.z);
		    }
		    if (wallRunning)
		    {
				jumpForce -= 25;
			    rb.AddForce(wallNormalVector * jumpForce * 2f);
		    }
		    Invoke("ResetJump", jumpCooldown);
		    if (wallRunning)
		    {
			    wallRunning = false;
		    }
        }
	}

    //Looking around by using your mouse
	private void Look()
	{
		if (!cancellMouseMovement)
		{
		float num = Input.GetAxis("Mouse X") * playerDataSO.sensitivity * Time.fixedDeltaTime * sensMultiplier;
		float num2 = Input.GetAxis("Mouse Y") * playerDataSO.sensitivity * Time.fixedDeltaTime * sensMultiplier;
		desiredX = playerCam.transform.localRotation.eulerAngles.y + num;
		xRotation -= num2;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);
		FindWallRunRotation();
		actualWallRotation = Mathf.SmoothDamp(actualWallRotation, wallRunRotation, ref wallRotationVel, 0.2f);
		playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, actualWallRotation);
		orientation.transform.localRotation = Quaternion.Euler(0f, desiredX, 0f);
        }
    }

	public void EnableMouse(bool setActive)
	{
        cancellMouseMovement = setActive;
	}

    //Make the player movement feel good 
	private void CounterMovement(float x, float y, Vector2 mag)
	{
		if (!grounded || jumping)
		{
			return;
		}
		float num = 0.16f;
		float num2 = 0.01f;
		if (crouching)
		{
			rb.AddForce(playerDataSO.moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideSlowdown);
			return;
		}
		if ((Math.Abs(mag.x) > num2 && Math.Abs(x) < 0.05f) || (mag.x < 0f - num2 && x > 0f) || (mag.x > num2 && x < 0f))
		{
			rb.AddForce(playerDataSO.moveSpeed * orientation.transform.right * Time.deltaTime * (0f - mag.x) * num);
		}
		if ((Math.Abs(mag.y) > num2 && Math.Abs(y) < 0.05f) || (mag.y < 0f - num2 && y > 0f) || (mag.y > num2 && y < 0f))
		{
			rb.AddForce(playerDataSO.moveSpeed * orientation.transform.forward * Time.deltaTime * (0f - mag.y) * num);
		}
		if (Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2f) + Mathf.Pow(rb.velocity.z, 2f)) > playerDataSO.walkSpeed)
		{
			float num3 = rb.velocity.y;
			Vector3 vector = rb.velocity.normalized * playerDataSO.walkSpeed;
			rb.velocity = new Vector3(vector.x, num3, vector.z);
		}
	}

	public Vector2 FindVelRelativeToLook()
	{
		float current = orientation.transform.eulerAngles.y;
		float target = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * 57.29578f;
		float num = Mathf.DeltaAngle(current, target);
		float num2 = 90f - num;
		float magnitude = rb.velocity.magnitude;
		return new Vector2(y: magnitude * Mathf.Cos(num * ((float)Math.PI / 180f)), x: magnitude * Mathf.Cos(num2 * ((float)Math.PI / 180f)));
	}

	private void FindWallRunRotation()
	{
		if (!wallRunning)
		{
			wallRunRotation = 0f;
			return;
		}
		_ = new Vector3(0f, playerCam.transform.rotation.y, 0f).normalized;
		new Vector3(0f, 0f, 1f);
		float num = 0f;
		float current = playerCam.transform.rotation.eulerAngles.y;
		if (Math.Abs(wallNormalVector.x - 1f) < 0.1f)
		{
			num = 90f;
		}
		else if (Math.Abs(wallNormalVector.x - -1f) < 0.1f)
		{
			num = 270f;
		}
		else if (Math.Abs(wallNormalVector.z - 1f) < 0.1f)
		{
			num = 0f;
		}
		else if (Math.Abs(wallNormalVector.z - -1f) < 0.1f)
		{
			num = 180f;
		}
		num = Vector3.SignedAngle(new Vector3(0f, 0f, 1f), wallNormalVector, Vector3.up);
		float num2 = Mathf.DeltaAngle(current, num);
		wallRunRotation = (0f - num2 / 90f) * 15f;
		if (!readyToWallrun)
		{
			return;
		}
		if ((Mathf.Abs(wallRunRotation) < 4f && y > 0f && Math.Abs(x) < 0.1f) || (Mathf.Abs(wallRunRotation) > 22f && y < 0f && Math.Abs(x) < 0.1f))
		{
			if (!cancelling)
			{
				cancelling = true;
				CancelInvoke("CancelWallrun");
				Invoke("CancelWallrun", 0.2f);
			}
		}
		else
		{
			cancelling = false;
			CancelInvoke("CancelWallrun");
		}
	}

	private void CancelWallrun()
	{
		MonoBehaviour.print("cancelled");
		Invoke("GetReadyToWallrun", 0.1f);
		rb.AddForce(wallNormalVector * 600f);
		readyToWallrun = false;
	}

	private void GetReadyToWallrun()
	{
		readyToWallrun = true;
	}

	private void WallRunning()
	{
		if (wallRunning)
		{
			rb.AddForce(-wallNormalVector * Time.deltaTime * playerDataSO.moveSpeed);
			rb.AddForce(Vector3.up * Time.deltaTime * rb.mass * 100f * wallRunGravity);
		}
	}

	private bool IsFloor(Vector3 v)
	{
		return Vector3.Angle(Vector3.up, v) < maxSlopeAngle;
	}

	private bool IsSurf(Vector3 v)
	{
		float num = Vector3.Angle(Vector3.up, v);
		if (num < 89f)
		{
			return num > maxSlopeAngle;
		}
		return false;
	}

	private bool IsWall(Vector3 v)
	{
		return Math.Abs(90f - Vector3.Angle(Vector3.up, v)) < 0.1f;
	}

	private bool IsRoof(Vector3 v)
	{
		return v.y == -1f;
	}

	private void StartWallRun(Vector3 normal)
	{
		if (!grounded && readyToWallrun)
		{
			wallNormalVector = normal;
			float num = 20f;
			if (!wallRunning)
			{
				rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
				rb.AddForce(Vector3.up * num, ForceMode.Impulse);
			}
			wallRunning = true;
		}
	}

	private void OnCollisionStay(Collision other)
	{
		int layer = other.gameObject.layer;
		if ((int)whatIsGround != ((int)whatIsGround | (1 << layer)))
		{
			return;
		}
		for (int i = 0; i < other.contactCount; i++)
		{
			Vector3 normal = other.contacts[i].normal;
			if (IsFloor(normal))
			{
				if (wallRunning)
				{
					wallRunning = false;
				}
				grounded = true;
				normalVector = normal;
				cancellingGrounded = false;
				CancelInvoke("StopGrounded");
			}
			if (IsWall(normal) && layer == LayerMask.NameToLayer("Ground"))
			{
				StartWallRun(normal);
				onWall = true;
				cancellingWall = false;
				CancelInvoke("StopWall");
			}
			if (IsSurf(normal))
			{
				surfing = true;
				cancellingSurf = false;
				CancelInvoke("StopSurf");
			}
			IsRoof(normal);
		}
		float num = 3f;
		if (!cancellingGrounded)
		{
			cancellingGrounded = true;
			Invoke("StopGrounded", Time.deltaTime * num);
		}
		if (!cancellingWall)
		{
			cancellingWall = true;
			Invoke("StopWall", Time.deltaTime * num);
		}
		if (!cancellingSurf)
		{
			cancellingSurf = true;
			Invoke("StopSurf", Time.deltaTime * num);
		}
	}

	private void StopGrounded()
	{
		grounded = false;
	}

	private void StopWall()
	{
		onWall = false;
		wallRunning = false;
	}

	private void StopSurf()
	{
		surfing = false;
	}

	public Vector3 GetVelocity()
	{
		return rb.velocity;
	}

	public float GetFallSpeed()
	{
		return rb.velocity.y;
	}

	public Collider GetPlayerCollider()
	{
		return playerCollider;
	}

	public Transform GetPlayerCamTransform()
	{
		return playerCam.transform;
	}

	public bool IsCrouching()
	{
		return crouching;
	}

	public Rigidbody GetRb()
	{
		return rb;
	}
}
