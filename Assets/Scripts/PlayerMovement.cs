using UnityEngine;

/// <summary>
/// Movimenta o player em 8 direções (top-down) usando WASD ou setas,
/// e envia os parâmetros MoveX, MoveY e Speed para o Animator, para
/// que o Blend Tree troque a animação de acordo com a direção.
/// Requer Rigidbody2D, Animator e SpriteRenderer no mesmo GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    [Tooltip("Velocidade de deslocamento do player, em unidades por segundo.")]
    public float velocidade = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 direcaoInput;

    // Guarda a última direção em que o player se moveu, para saber
    // para que lado ele deve "olhar" enquanto está parado (Idle).
    private Vector2 ultimaDirecao = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Lê o input a cada frame (mais responsivo)
        float x = Input.GetAxisRaw("Horizontal"); // A/D ou Setas esquerda/direita
        float y = Input.GetAxisRaw("Vertical");   // W/S ou Setas cima/baixo

        direcaoInput = new Vector2(x, y).normalized; // normalized evita ele andar mais rápido na diagonal

        // Só atualiza a "última direção" quando há movimento real,
        // assim ela fica congelada na direção certa quando o player para.
        if (direcaoInput.sqrMagnitude > 0.01f)
        {
            ultimaDirecao = direcaoInput;
        }

        AtualizarAnimacao();
    }

    void FixedUpdate()
    {
        // Move o Rigidbody2D no FixedUpdate (mais correto fisicamente)
        rb.linearVelocity = direcaoInput * velocidade;
    }

    void AtualizarAnimacao()
    {
        // Envia a última direção conhecida (não a atual, que pode ser zero)
        // para o Blend Tree escolher a animação certa, mesmo parado.
        animator.SetFloat("MoveX", ultimaDirecao.x);
        animator.SetFloat("MoveY", ultimaDirecao.y);
        // Speed indica se ele está se movendo de verdade (usado para trocar entre Idle e Andar)
        animator.SetFloat("Speed", direcaoInput.sqrMagnitude);

        // Vira a sprite horizontalmente quando olhar para a esquerda
        // (assumindo que a animação "Side" original olha para a direita)
        if (ultimaDirecao.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
        else if (ultimaDirecao.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
    }
}