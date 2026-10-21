using UnityEngine;

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
        if (interlocutor == null) { Debug.Log("[OrientacaoNpc] interlocutor nulo"); return false; }
        if (renderizador == null) renderizador = GetComponent<SpriteRenderer>();
        if (renderizador == null) { Debug.Log("[OrientacaoNpc] sem renderizador"); return false; }

        Vector2 direcao = CentroVisual(interlocutor) - renderizador.bounds.center;
        Debug.Log($"[OrientacaoNpc] interlocutor='{interlocutor.name}' pos={interlocutor.position} npc={transform.position} direcao={direcao}");

        if (direcao.sqrMagnitude < 0.0001f) { Debug.Log("[OrientacaoNpc] direcao zero"); return false; }

        Sprite pose;
        if (Mathf.Abs(direcao.x) > Mathf.Abs(direcao.y))
            pose = direcao.x > 0f ? direita : esquerda;
        else
            pose = direcao.y > 0f ? cima : baixo;

        Debug.Log($"[OrientacaoNpc] pose escolhida: {(pose == null ? "NULA" : pose.name)}");

        if (pose == null) return false;

        renderizador.sprite = pose;
        renderizador.flipX = false;
        renderizador.flipY = false;

        Debug.Log($"[OrientacaoNpc] sprite agora: {renderizador.sprite.name}");
        return true;

        static Vector3 CentroVisual(Transform t)
            {
                var sr = t.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) return sr.bounds.center;
                var col = t.GetComponentInChildren<Collider2D>();
                if (col != null) return col.bounds.center;
                return t.position;
            }
    }
}