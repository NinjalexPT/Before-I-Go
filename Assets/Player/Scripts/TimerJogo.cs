using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TimerJogo : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text textoTimer;      // arrasta aqui o texto do Canvas

    [Header("Tempo")]
    public float tempoInicial = 60f; // segundos de onde a contagem começa
    public float escalaTempo = 1f;   // 1 = normal, 2 = dobro, 0.5 = metade
    public float passo = 0.5f;       // quanto muda a cada toque
    public float escalaMin = 0f;
    public float escalaMax = 10f;
    public bool aCorrer = true;

    [Header("Opções")]
    [Tooltip("Se ativo, acelera/abranda o jogo todo (Time.timeScale). Se desativo, só o timer muda.")]
    public bool afetarJogoTodo = false;

    private float tempoRestante;
    private bool terminou = false;

    void Start()
    {
        tempoRestante = tempoInicial;
    }

    void Update()
    {
        LerTeclas();

        if (afetarJogoTodo)
            Time.timeScale = aCorrer ? escalaTempo : 1f;
        else
            Time.timeScale = 1f;

        if (aCorrer && !terminou)
        {
            // Com timeScale ativo o deltaTime já vem escalado; senão aplicamos a escala à mão
            float dt = afetarJogoTodo ? Time.deltaTime : Time.deltaTime * escalaTempo;
            tempoRestante -= dt;

            if (tempoRestante <= 0f)
            {
                tempoRestante = 0f;
                terminou = true;
                AoTerminar();
            }
        }

        AtualizarUI();
    }

    // --- Chamados pelos botões da UI ---
    public void Acelerar()
    {
        escalaTempo = Mathf.Min(escalaTempo + passo, escalaMax);
    }

    public void Desacelerar()
    {
        escalaTempo = Mathf.Max(escalaTempo - passo, escalaMin);
    }

    public void VelocidadeNormal()
    {
        escalaTempo = 1f;
    }

    void AoTerminar()
    {
        Debug.Log("Tempo esgotado!");
        // Aqui podes meter game over, abrir um menu, etc.
    }

    void LerTeclas()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return;

        // Seta para cima = acelerar (desacelerar é chamado pelo FirstPersonController, tecla Q)
        if (teclado.upArrowKey.wasPressedThisFrame)
            Acelerar();

        // R = repor velocidade normal | T = pausar/continuar | Y = reiniciar o timer
        if (teclado.rKey.wasPressedThisFrame)
            VelocidadeNormal();

        // Esc = libertar/prender o cursor (para poderes clicar nos botões)
        if (teclado.escapeKey.wasPressedThisFrame)
        {
            bool preso = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = preso ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = preso;
        }

        if (teclado.tKey.wasPressedThisFrame)
            aCorrer = !aCorrer;

        if (teclado.yKey.wasPressedThisFrame)
        {
            tempoRestante = tempoInicial;
            terminou = false;
        }
    }

    void AtualizarUI()
    {
        if (textoTimer == null) return;

        int minutos = (int)(tempoRestante / 60f);
        float segundos = tempoRestante % 60f;

        textoTimer.text = $"{minutos:00}:{segundos:00.0}\nVelocidade: x{escalaTempo:0.0}" + (aCorrer ? "" : " (pausado)");
    }

    void OnDisable()
    {
        Time.timeScale = 1f; // não deixar o jogo preso em câmara lenta
    }
}