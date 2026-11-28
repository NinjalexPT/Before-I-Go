using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;
    // public float forcaSalto = 1.5f;
    public float gravidade = -20f;

    [Header("Timer")]
    public TimerJogo timer;          // arrasta aqui o objeto "timer"

    [Header("Camera")]
    public Transform cameraTransform;
    public float sensibilidadeRato = 0.1f;
    public float limiteVertical = 85f;

    private CharacterController controller;
    private float rotacaoX = 0f;
    private Vector3 velocidadeVertical;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Esconde e prende o cursor ao centro do ecrã
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Walk();
        UsarTimer();
    }

    void UsarTimer()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null || timer == null) return;

        // Q = desacelerar o tempo (a função vive no TimerJogo)
        if (teclado.qKey.wasPressedThisFrame)
            timer.Desacelerar();
    }

    void Look()
    {
        if (Mouse.current == null) return;
        if (Cursor.lockState != CursorLockMode.Locked) return; // cursor livre: não rodar a câmara

        Vector2 delta = Mouse.current.delta.ReadValue() * sensibilidadeRato;

        // Rodar o corpo (esquerda/direita)
        transform.Rotate(Vector3.up * delta.x);

        // Rodar a câmara (cima/baixo), com limite
        rotacaoX -= delta.y;
        rotacaoX = Mathf.Clamp(rotacaoX, -limiteVertical, limiteVertical);
        cameraTransform.localRotation = Quaternion.Euler(rotacaoX, 0f, 0f);
    }

    void Walk()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return;

        float x = 0f;
        float z = 0f;
        if (teclado.dKey.isPressed) x += 1f;
        if (teclado.aKey.isPressed) x -= 1f;
        if (teclado.wKey.isPressed) z += 1f;
        if (teclado.sKey.isPressed) z -= 1f;

        Vector3 direcao = (transform.right * x + transform.forward * z).normalized;
        controller.Move(direcao * velocidade * Time.deltaTime);

        // Gravidade
        if (controller.isGrounded && velocidadeVertical.y < 0)
            velocidadeVertical.y = -2f;

        // Salto (desativado)
        // if (teclado.spaceKey.wasPressedThisFrame && controller.isGrounded)
        //     velocidadeVertical.y = Mathf.Sqrt(forcaSalto * -2f * gravidade);

        velocidadeVertical.y += gravidade * Time.deltaTime;
        controller.Move(velocidadeVertical * Time.deltaTime);
    }
}