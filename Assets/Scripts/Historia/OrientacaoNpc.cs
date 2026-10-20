using UnityEngine;

/// <summary>
/// Troca a pose de um NPC parado para olhar para o interlocutor.
/// Anexar junto ao GatilhoDeDialogo; configurar quatro sprites direcionais.
/// Um Animator que controle a mesma sprite precisa de integração própria.
/// </summary>
[DisallowMultipleComponent]
public class OrientacaoNpc : MonoBehaviour
{
    public SpriteRenderer renderizador;
    public Sprite cima;
    public Sprite esquerda;
    public Sprite baixo;
    public Sprite direita;

    void Reset()
    {
        renderizador = GetComponent<SpriteRenderer>();
    }

    public bool OlharPara(Transform interlocutor)
    {
        if (interlocutor == null) return false;
        if (renderizador == null) renderizador = GetComponent<SpriteRenderer>();
        if (renderizador == null) return false;

        Vector2 direcao = interlocutor.position - transform.position;
        if (direcao.sqrMagnitude < 0.0001f) return false;

        Sprite pose;
        if (Mathf.Abs(direcao.x) > Mathf.Abs(direcao.y))
            pose = direcao.x > 0f ? direita : esquerda;
        else
            pose = direcao.y > 0f ? cima : baixo;

        // Configuração incompleta não apaga a sprite nem altera a pose atual.
        if (pose == null) return false;

        renderizador.sprite = pose;
        renderizador.flipX = false;
        renderizador.flipY = false;
        return true;
    }
}
