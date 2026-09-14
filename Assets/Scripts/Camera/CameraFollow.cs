using UnityEngine;

/// <summary>
/// Faz a Main Camera seguir suavemente um alvo (o personagem/Lip) e,
/// opcionalmente, trava a câmera para nunca mostrar área fora do mapa.
/// Anexar este script na Main Camera da cena City (ou qualquer cena top-down).
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Alvo a seguir")]
    [Tooltip("Arraste aqui o Transform do personagem (Lip).")]
    public Transform target;

    [Header("Suavização")]
    [Tooltip("Quanto maior, mais rápido a câmera alcança o personagem.")]
    public float smoothSpeed = 5f;

    [Tooltip("Deslocamento da câmera em relação ao alvo. Z deve ficar negativo para a câmera 2D enxergar os sprites.")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Limites do mapa (opcional)")]
    [Tooltip("Se marcado, a câmera não mostra área vazia fora do mapa.")]
    public bool usarLimites = false;

    [Tooltip("Canto inferior-esquerdo do mapa, em coordenadas do mundo.")]
    public Vector2 limiteMin;

    [Tooltip("Canto superior-direito do mapa, em coordenadas do mundo.")]
    public Vector2 limiteMax;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 posicaoDesejada = target.position + offset;
        Vector3 posicaoSuavizada = Vector3.Lerp(transform.position, posicaoDesejada, smoothSpeed * Time.deltaTime);

        if (usarLimites && cam != null)
        {
            float metadeAltura = cam.orthographicSize;
            float metadeLargura = metadeAltura * cam.aspect;

            float minX = limiteMin.x + metadeLargura;
            float maxX = limiteMax.x - metadeLargura;
            float minY = limiteMin.y + metadeAltura;
            float maxY = limiteMax.y - metadeAltura;

            // Se o mapa for menor que a tela em algum eixo, trava no centro
            // desse eixo em vez de inverter os limites.
            float x = (minX <= maxX) ? Mathf.Clamp(posicaoSuavizada.x, minX, maxX) : (limiteMin.x + limiteMax.x) / 2f;
            float y = (minY <= maxY) ? Mathf.Clamp(posicaoSuavizada.y, minY, maxY) : (limiteMin.y + limiteMax.y) / 2f;

            posicaoSuavizada = new Vector3(x, y, posicaoDesejada.z);
        }

        transform.position = posicaoSuavizada;
    }
}
