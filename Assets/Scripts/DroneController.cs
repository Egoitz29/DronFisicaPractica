using UnityEngine;
using UnityEngine.InputSystem;
using MyGame.Input;

public class DroneController_Visual : MonoBehaviour
{
    [Header("Referencias Principales")]
    public Rigidbody rb;
    public CharacterController controller;
    public Transform groundCheck;
    public Light modeIndicator; // 💡 Luz LED que cambia de color
    public Transform[] propellers; // 🌀 Hélices (pivots)
    public ParticleSystem[] thrusters; // 💨 Empuje visual

    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float verticalSpeed = 3f;
    public float jumpForce = 5f;
    public float physicsForce = 10f;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Estabilidad y Física")]
    public float rotationSpeed = 3f;
    public float stabilizerForce = 2f;
    public float highDrag = 2f;
    public float lowDrag = 0.5f;
    public float uprightForce = 50f; // ⚙️ fuerza del estabilizador rotacional
    public float uprightTorqueDamping = 10f; // amortiguación del torque

    [Header("Hélices")]
    public float baseSpinSpeed = 600f;  // velocidad normal
    public float boostSpinSpeed = 1500f; // velocidad al ascender
    public float spinSmoothness = 5f;   // suavidad de transición

    private InputActions controls;
    private Vector2 moveInput;
    private float verticalInput;
    private bool isUsingCharacterController = true;
    private bool isGrounded;
    private float thrusterTimer = 0f;
    private float currentSpinSpeed; // velocidad actual de giro
    [HideInInspector] public float lastModeSwitchTime;
    private HUDManager hud;


    void Start()
    {
        hud = FindObjectOfType<HUDManager>();
    }



    void Awake()
    {
        controls = new InputActions();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Ascend.performed += _ => TryJump();
        controls.Player.Ascend.canceled += _ => verticalInput = 0f;

        controls.Player.Descend.performed += _ => verticalInput = -1f;
        controls.Player.Descend.canceled += _ => verticalInput = 0f;

        controls.Player.SwitchMode.performed += _ => SwitchMode();
        controls.Player.Fire.performed += _ => Fire(); // 🔥 <- ESTA ES LA CLAVE
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();


    void Update()
    {
        if (controls.Player.Fire.triggered)
        {
            Debug.Log("🎯 Input detectado desde Update()");
        }


        if (isUsingCharacterController && controller.enabled)
        {
            Vector3 move = new Vector3(moveInput.x, verticalInput, moveInput.y);
            controller.Move(move * moveSpeed * Time.deltaTime);
        }

        RotatePropellers();
        UpdateThrusters();
        UpdateFire(); 
    }


    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isUsingCharacterController) return;

        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);

        // ✈️ Ascenso controlado y caída suave
        if (verticalInput > 0f)
        {
            rb.AddForce(Vector3.up * (jumpForce * 0.8f), ForceMode.Acceleration);
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.up * 3f, ForceMode.Acceleration);
        }

        // Fuerza horizontal
        rb.AddForce(direction * physicsForce, ForceMode.Acceleration);

        // 🧭 Mantenerse vertical
        KeepUpright();

        // 🌀 Rotación hacia la dirección del movimiento
        if (direction.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.deltaTime * rotationSpeed));
        }

        if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            rb.AddForce(Vector3.up * stabilizerForce, ForceMode.Acceleration);

        rb.linearDamping = moveInput.magnitude > 0.1f ? lowDrag : highDrag;
    }

    void TryJump()
    {
        if (!isUsingCharacterController)
        {
            verticalInput = 1f;
            thrusterTimer = 0.3f;
        }
        else
        {
            verticalInput = 1f;
        }
    }

    void SwitchMode()
    {
        isUsingCharacterController = !isUsingCharacterController;
        controller.enabled = isUsingCharacterController;
        rb.isKinematic = isUsingCharacterController;

        if (modeIndicator)
            modeIndicator.color = isUsingCharacterController ? Color.cyan : Color.red;

        lastModeSwitchTime = Time.time;

        string modo = isUsingCharacterController ? "🟦 MODO ARCADE ACTIVADO" : "🔴 MODO FÍSICO ACTIVADO";
        Color colorTexto = isUsingCharacterController ? Color.cyan : Color.red;

        Debug.Log(modo);

        if (hud)
            hud.ShowColoredMessage(modo, colorTexto, 3f);
    }



    // 🌀 Animación de hélices (reacciona al ascenso)
    void RotatePropellers()
    {
        if (propellers == null || propellers.Length == 0) return;

        // Acelera hélices si se está ascendiendo
        float targetSpeed = verticalInput > 0.1f ? boostSpinSpeed : baseSpinSpeed;
        currentSpinSpeed = Mathf.Lerp(currentSpinSpeed, targetSpeed, Time.deltaTime * spinSmoothness);

        foreach (Transform prop in propellers)
        {
            prop.Rotate(Vector3.up, currentSpinSpeed * Time.deltaTime, Space.Self);
        }
    }

    // 💨 Partículas solo al ascender
    void UpdateThrusters()
    {
        if (thrusters == null || thrusters.Length == 0) return;

        bool isAscending = verticalInput > 0.1f;

        if (isAscending)
            thrusterTimer = 0.3f;
        else if (thrusterTimer > 0f)
            thrusterTimer -= Time.deltaTime;

        bool shouldEmit = thrusterTimer > 0f;

        foreach (ParticleSystem p in thrusters)
        {
            var emission = p.emission;
            emission.enabled = shouldEmit;
        }
    }

    // ⚖️ Mantiene el dron vertical con corrección suave
    void KeepUpright()
    {
        Vector3 desiredUp = Vector3.up;
        Vector3 currentUp = transform.up;

        Vector3 correctionAxis = Vector3.Cross(currentUp, desiredUp);
        float tiltAngle = correctionAxis.magnitude;

        if (tiltAngle < 0.01f)
            return;

        correctionAxis.Normalize();

        float correctiveStrength = Mathf.Clamp01(tiltAngle * 10f);

        Vector3 torque = correctionAxis * (uprightForce * correctiveStrength)
                         - rb.angularVelocity * uprightTorqueDamping;

        rb.AddTorque(torque, ForceMode.Acceleration);
    }

    // ===================== SISTEMA DE DISPARO =====================

    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletForce = 500f;
    private float fireCooldown = 0.25f;
    private float nextFireTime = 0f;

    void UpdateFire()
    {
        if (controls.Player.Fire.triggered && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void Fire()
    {
        Debug.Log("🔥 Fire() ejecutado");
        if (!bulletPrefab || !shootPoint) return;

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rbBullet = bullet.GetComponent<Rigidbody>();
        rbBullet.AddForce(shootPoint.forward * bulletForce);

        // Destruye la bala tras unos segundos para limpiar la escena
        Destroy(bullet, 5f);
    }

}
