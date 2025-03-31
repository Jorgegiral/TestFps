using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //Librería para trabajar con el New Input System

public class FPSController : MonoBehaviour
{
    [Header("Movement & Look Stats")]
    [SerializeField] GameObject camHolder; //Ref al objeto que almacena la cámara (ojos del player)
    public float speed; //Velocidad normal
    public float sprintSpeed; //Velocidad en sprint
    public float crouchSpeed; //Velocidad agachado
    public float maxForce = 1; //Límite de aceleración máxima
    public float sensitivity = 0.1f; //Sensibilidad aplicada al input de observar

    [Header("Jumping Stats")]
    public float jumpForce;
    //Variables del GroundCheck
    [SerializeField] GameObject groundCheck;
    [SerializeField] bool isGrounded;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundLayer;

    [Header("Player State Bools")]
    [SerializeField] bool isSprinting;
    [SerializeField] bool isCrouching;

    //Referencias privadas (GetComponent)
    private Rigidbody playerRb;
    private Animator anim;
    //Referencias privadas del input
    Vector2 moveInput;
    Vector2 lookInput;
    float lookRotation; //Valor de rotación que puede ser utilizado para la dirección de movimiento

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        camHolder = GameObject.Find("CameraHolder");
        groundCheck = GameObject.Find("GroundCheck");
    }

    void Start()
    {
        //Lock del cursor al centro de la cámara y ocultarlo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundCheckRadius, groundLayer);
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    void Movement()
    {
        Vector3 currentVelocity = playerRb.velocity; //Velocidad actual del player
        Vector3 targetVelocity = new Vector3(moveInput.x, 0, moveInput.y); //Velocidad hacia la que queremos que se mueva el player
        targetVelocity *= isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : speed);

        //Alinear la dirección con la orientación correcta (de local a global)
        targetVelocity = transform.TransformDirection(targetVelocity);

        //Calcular las fuerzas que afectan al movimiento
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z); //Hace que la aceleración no afecte en vertical
        Vector3.ClampMagnitude(velocityChange, maxForce);

        //Aplicamos el movimiento
        playerRb.AddForce(velocityChange, ForceMode.VelocityChange);


    }

    void CameraLook()
    {
        //Girar (Gira el personaje en horizontal)
        transform.Rotate(Vector3.up * lookInput.x * sensitivity);
        //Mirar (Gira la cámara en vertical)
        lookRotation += (-lookInput.y * sensitivity);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90); //Restringe el valor de lookRotation entre dos valores mínimo/máximo
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);
    }

    void Jump()
    {
        if (isGrounded)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    #region Input Methods

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Jump();
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)  //Cambia el bool al valor contrario que tenga en ese momento
        {
            isCrouching = !isCrouching;
            anim.SetTrigger("CrouchState"); //Cada vez que pulsamos el input llamamos el Animator
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isCrouching) isSprinting = true;
        }
        if (context.canceled)
        {
            isSprinting = false;
        }
    }

    #endregion
}
