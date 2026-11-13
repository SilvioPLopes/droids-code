// Ferramenta Editor: le a fisica atual; nao cria objetos, nao altera componentes e nao faz IO.
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Grade finita para o circulo integral do Player, com varredura continua nas arestas.</summary>
public static class AnaliseCirculacaoTester
{
    const int LimiteNos = 15000;
    const float ToleranciaGeometria = .0005f;
    static readonly int[] Dx = { 1, 0, -1, 0 };
    static readonly int[] Dy = { 0, 1, 0, -1 };

    public sealed class Resultado
    {
        public bool sucesso;
        public string erro = "";
        public Vector2[] pontos = Array.Empty<Vector2>();
        public int visitados;
        public string[] bloqueadores = Array.Empty<string>();
    }

    public sealed class Mapa
    {
        public float passo, raio;
        public Vector2 offset, min;
        public int largura, altura, livres;

        readonly GameObject player;
        readonly CircleCollider2D corpo;
        readonly Rigidbody2D rigidbody;
        readonly Scene cena;
        readonly Vector2 max;
        readonly bool[] livre;
        readonly byte[] arestas;
        readonly int[] componentes;
        readonly Dictionary<int, string[]> bloqueiosNos = new Dictionary<int, string[]>();
        readonly Dictionary<long, string[]> bloqueiosArestas = new Dictionary<long, string[]>();
        readonly HashSet<Collider2D> portais;
        readonly ContactFilter2D filtroTriggers;
        readonly List<Collider2D> sobrepostos = new List<Collider2D>();
        readonly List<RaycastHit2D> hitsTriggers = new List<RaycastHit2D>();

        internal Mapa(GameObject jogador, CircleCollider2D circulo, Bounds fundo, float tamanhoPasso)
        {
            player = jogador;
            corpo = circulo;
            rigidbody = circulo.attachedRigidbody;
            cena = SceneManager.GetActiveScene();
            passo = tamanhoPasso;
            raio = Mathf.Max(circulo.bounds.extents.x, circulo.bounds.extents.y);
            offset = (Vector2)circulo.bounds.center - (Vector2)jogador.transform.position;
            min = (Vector2)fundo.min + Vector2.one * raio;
            max = (Vector2)fundo.max - Vector2.one * raio;
            Exigir(raio > 0f && Finito(raio), "CORPO_INVALIDO: raio mundial deve ser positivo.");
            Exigir(max.x >= min.x && max.y >= min.y, "FORA_BOUNDS: fundo menor que o corpo integral.");
            double colunas = Math.Floor(((double)max.x - min.x) / passo) + 1d;
            double linhas = Math.Floor(((double)max.y - min.y) / passo) + 1d;
            Exigir(colunas * linhas <= LimiteNos, "TETO_NOS: grade excede " + LimiteNos
                + " celulas (" + colunas + " x " + linhas + ").");
            largura = (int)colunas;
            altura = (int)linhas;
            livre = new bool[largura * altura];
            arestas = new byte[livre.Length];
            componentes = Enumerable.Repeat(-1, livre.Length).ToArray();
            filtroTriggers = new ContactFilter2D { useTriggers = true, useLayerMask = false,
                useDepth = false, useNormalAngle = false };
            portais = new HashSet<Collider2D>(UnityEngine.Object.FindObjectsByType<Collider2D>(
                    FindObjectsInactive.Exclude)
                .Where(c => c.isTrigger && Efetivo(c) && c.GetComponent<TransicaoDeCena>() != null));

            for (int i = 0; i < livre.Length; i++)
            {
                string[] bloqueadores = SolidosSobrepostos(Centro(i));
                livre[i] = bloqueadores.Length == 0;
                if (livre[i]) livres++;
                else bloqueiosNos.Add(i, bloqueadores);
            }
            for (int i = 0; i < livre.Length; i++)
            {
                if (!livre[i]) continue;
                // Cada par cardinal e testado uma unica vez e armazenado nos dois sentidos.
                for (int direcao = 0; direcao < 2; direcao++)
                {
                    int vizinho = Vizinho(i, direcao);
                    if (vizinho < 0 || !livre[vizinho]) continue;
                    string[] bloqueadores = VarreduraSolidos(Centro(i), Centro(vizinho));
                    if (bloqueadores.Length == 0)
                    {
                        arestas[i] |= (byte)(1 << direcao);
                        arestas[vizinho] |= (byte)(1 << ((direcao + 2) % 4));
                    }
                    else bloqueiosArestas.Add(ChaveAresta(i, vizinho), bloqueadores);
                }
            }
            RotularComponentes();
        }

        /// <summary>Uma raiz por setor que possua celula alcancavel sem atravessar transicoes.</summary>
        public Vector2[] Cobertura(Vector2 origem, float setor = .25f)
        {
            ValidarAtual();
            Exigir(Finito(setor) && setor > 0f, "SETOR_INVALIDO: tamanho deve ser positivo.");
            Exigir(Finito(origem), "ORIGEM_INVALIDA: coordenadas nao finitas.");
            Exigir(Dentro(origem + offset), "FORA_BOUNDS_ORIGEM.");
            string[] ocupantes = SolidosSobrepostos(origem + offset);
            Exigir(ocupantes.Length == 0, "ORIGEM_OCUPADA: " + string.Join("; ", ocupantes));
            var bloqueadores = new HashSet<string>(StringComparer.Ordinal);
            int[] inicios = Conectores(origem, null, bloqueadores);
            Exigir(inicios.Length > 0, "ORIGEM_SEM_CONEXAO: " + string.Join("; ", bloqueadores));

            var visitado = new bool[livre.Length];
            var pilha = new Stack<int>(inicios.Reverse());
            var ordem = new List<int>();
            while (pilha.Count > 0)
            {
                int atual = pilha.Pop();
                if (visitado[atual]) continue;
                visitado[atual] = true;
                ordem.Add(atual);
                for (int direcao = 3; direcao >= 0; direcao--)
                {
                    if ((arestas[atual] & (1 << direcao)) == 0) continue;
                    int vizinho = Vizinho(atual, direcao);
                    if (!visitado[vizinho] && VarreduraPortais(Centro(atual), Centro(vizinho), null).Length == 0)
                        pilha.Push(vizinho);
                }
            }

            var representantes = new Dictionary<long, int>();
            var distancias = new Dictionary<long, float>();
            foreach (int indice in ordem)
            {
                Vector2 centro = Centro(indice);
                // Tambem exclui a celula que nasceu dentro de uma regiao de portal.
                if (PortaisSobrepostos(centro, null).Length > 0) continue;
                int x = Mathf.FloorToInt((centro.x - min.x) / setor);
                int y = Mathf.FloorToInt((centro.y - min.y) / setor);
                long chave = ((long)x << 32) | (uint)y;
                Vector2 centroSetor = min + new Vector2((x + .5f) * setor, (y + .5f) * setor);
                float distancia = (centro - centroSetor).sqrMagnitude;
                if (!distancias.TryGetValue(chave, out float anterior) || distancia < anterior)
                {
                    distancias[chave] = distancia;
                    representantes[chave] = indice;
                }
            }
            // Ordem DFS da conectividade; preserva todos os setores, sem truncar em mil pontos.
            var escolhidos = new HashSet<int>(representantes.Values);
            return ordem.Where(escolhidos.Contains).Select(i => Centro(i) - offset).ToArray();
        }

        public Resultado Planejar(Vector2 origem, Vector2 destino, Collider2D contato = null,
            float margemContato = .06f)
        {
            ValidarAtual();
            var bloqueadores = new HashSet<string>(StringComparer.Ordinal);
            if (!Finito(origem) || !Finito(destino) || !Finito(margemContato) || margemContato < 0f)
                return Falha("COORDENADAS_OU_MARGEM_INVALIDAS", 0, bloqueadores);
            if (!Dentro(origem + offset)) return Falha("FORA_BOUNDS_ORIGEM", 0, bloqueadores);
            if (contato == null && !Dentro(destino + offset))
                return Falha("FORA_BOUNDS_DESTINO", 0, bloqueadores);
            if (contato != null && (!contato.enabled || !contato.gameObject.activeInHierarchy
                || !Efetivo(contato))) return Falha("CONTATO_INATIVO_OU_IGNORADO", 0, bloqueadores);

            Adicionar(bloqueadores, SolidosSobrepostos(origem + offset));
            if (bloqueadores.Count > 0) return Falha("ORIGEM_OCUPADA", 0, bloqueadores);
            string[] portaisOrigem = PortaisSobrepostos(origem + offset, contato);
            if (portaisOrigem.Length > 0)
            {
                Adicionar(bloqueadores, portaisOrigem);
                return Falha("ORIGEM_EM_PORTAL_NAO_ESCOLHIDO", 0, bloqueadores);
            }
            if (Objetivo(origem, destino, contato, margemContato))
                return Sucesso(new[] { origem }, 0, bloqueadores);

            int[] inicios = Conectores(origem, contato, bloqueadores);
            if (inicios.Length == 0) return Falha("ORIGEM_SEM_CONEXAO", 0, bloqueadores);
            var pais = Enumerable.Repeat(-2, livre.Length).ToArray();
            var fila = new Queue<int>();
            foreach (int inicio in inicios) { pais[inicio] = -1; fila.Enqueue(inicio); }
            int visitados = 0;
            int contatoSemMargem = -1;
            bool origemSemMargem = contato != null && margemContato > 0f
                && TemContato(origem + offset, contato, 0f);
            while (fila.Count > 0)
            {
                int atual = fila.Dequeue();
                visitados++;
                Vector2 raiz = Centro(atual) - offset;
                if (Objetivo(raiz, destino, contato, margemContato))
                    return Sucesso(Caminho(origem, atual, pais), visitados, bloqueadores);
                // O destino pode cair entre nos a mais de .08m de todos eles. A ultima ligacao
                // tambem e varrida, sem deslocar/snapar o Player nem atravessar geometria.
                if (contato == null && (raiz - destino).sqrMagnitude <= passo * passo * 3.0625f)
                {
                    string[] barreirasFim = VarreduraSolidos(Centro(atual), destino + offset);
                    Adicionar(bloqueadores, barreirasFim);
                    if (barreirasFim.Length == 0)
                    {
                        string[] portaisFim = VarreduraPortais(Centro(atual), destino + offset, null);
                        Adicionar(bloqueadores, portaisFim);
                        if (portaisFim.Length == 0)
                            return Sucesso(Caminho(origem, atual, pais).Concat(new[] { destino }).ToArray(),
                                visitados, bloqueadores);
                    }
                }
                if (contato != null && margemContato > 0f && contatoSemMargem < 0
                    && TemContato(Centro(atual), contato, 0f)) contatoSemMargem = atual;

                for (int direcao = 0; direcao < 4; direcao++)
                {
                    int vizinho = Vizinho(atual, direcao);
                    if (vizinho < 0) continue;
                    if (!livre[vizinho])
                    {
                        Adicionar(bloqueadores, bloqueiosNos[vizinho]);
                        continue;
                    }
                    if ((arestas[atual] & (1 << direcao)) == 0)
                    {
                        if (bloqueiosArestas.TryGetValue(ChaveAresta(atual, vizinho), out string[] barreira))
                            Adicionar(bloqueadores, barreira);
                        continue;
                    }
                    if (pais[vizinho] != -2) continue;
                    string[] portaisBloqueando = VarreduraPortais(Centro(atual), Centro(vizinho), contato);
                    if (portaisBloqueando.Length > 0)
                    {
                        Adicionar(bloqueadores, portaisBloqueando);
                        continue;
                    }
                    pais[vizinho] = atual;
                    fila.Enqueue(vizinho);
                }
            }
            if (origemSemMargem || contatoSemMargem >= 0)
            {
                Resultado falha = Falha("CONTATO_SEM_MARGEM", visitados, bloqueadores);
                falha.pontos = origemSemMargem ? new[] { origem } : Caminho(origem, contatoSemMargem, pais);
                return falha;
            }
            return Falha(contato != null ? "SEM_CONEXAO_CONTATO" : "SEM_CONEXAO_DESTINO",
                visitados, bloqueadores);
        }

        /// <summary>Penetracao atual do corpo real; tocar a superficie nao e penetracao.</summary>
        public string[] Penetracoes(float tolerancia = .005f)
        {
            ValidarAtual();
            Exigir(Finito(tolerancia) && tolerancia >= 0f, "TOLERANCIA_INVALIDA.");
            var bloqueadores = new HashSet<string>(StringComparer.Ordinal);
            foreach (Collider2D colisor in Physics2D.OverlapCircleAll(corpo.bounds.center, raio, Physics2D.AllLayers))
            {
                if (!Solido(colisor)) continue;
                ColliderDistance2D distancia = corpo.Distance(colisor);
                if (distancia.isValid && distancia.distance < -tolerancia)
                    bloqueadores.Add(CaminhoObjeto(colisor.transform));
            }
            return bloqueadores.ToArray();
        }

        public bool Ocupado(Vector2 raiz)
        {
            ValidarAtual();
            return !Finito(raiz) || !Dentro(raiz + offset)
                || SolidosSobrepostos(raiz + offset).Length > 0;
        }

        void ValidarAtual()
        {
            Exigir(player != null && corpo != null && corpo.enabled && corpo.gameObject.activeInHierarchy,
                "MAPA_OBSOLETO: Player/corpo nao esta ativo.");
            Exigir(SceneManager.GetActiveScene() == cena,
                "MAPA_OBSOLETO: cena ativa mudou; crie um mapa novo.");
            Vector2 offsetAtual = (Vector2)corpo.bounds.center - (Vector2)player.transform.position;
            float raioAtual = Mathf.Max(corpo.bounds.extents.x, corpo.bounds.extents.y);
            Exigir(Mathf.Abs(raioAtual - raio) <= ToleranciaGeometria
                && (offsetAtual - offset).sqrMagnitude <= ToleranciaGeometria * ToleranciaGeometria,
                "MAPA_OBSOLETO: raio/offset do corpo mudou.");
        }

        bool Dentro(Vector2 centro)
        {
            return centro.x >= min.x - ToleranciaGeometria && centro.x <= max.x + ToleranciaGeometria
                && centro.y >= min.y - ToleranciaGeometria && centro.y <= max.y + ToleranciaGeometria;
        }

        bool Efetivo(Collider2D colisor)
        {
            if (colisor == null || !colisor.enabled || !colisor.gameObject.activeInHierarchy
                || colisor == corpo || colisor.transform.IsChildOf(player.transform)) return false;
            if (rigidbody != null && colisor.attachedRigidbody == rigidbody) return false;
            if (colisor.attachedRigidbody != null && !colisor.attachedRigidbody.simulated) return false;
            return !Physics2D.GetIgnoreLayerCollision(corpo.gameObject.layer, colisor.gameObject.layer)
                && !Physics2D.GetIgnoreCollision(corpo, colisor);
        }

        bool Solido(Collider2D colisor) { return Efetivo(colisor) && !colisor.isTrigger; }

        string[] SolidosSobrepostos(Vector2 centro)
        {
            var bloqueadores = new HashSet<string>(StringComparer.Ordinal);
            // Consulta usa o raio integral. Tolerancia classifica profundidade; nao encolhe o corpo.
            foreach (Collider2D colisor in Physics2D.OverlapCircleAll(centro, raio, Physics2D.AllLayers))
                if (Solido(colisor) && Profundidade(colisor, centro) > ToleranciaGeometria)
                    Adicionar(bloqueadores, CaminhoObjeto(colisor.transform));
            return bloqueadores.ToArray();
        }

        float Profundidade(Collider2D colisor, Vector2 centro)
        {
            return raio - Vector2.Distance(centro, colisor.ClosestPoint(centro));
        }

        string[] VarreduraSolidos(Vector2 origem, Vector2 destino)
        {
            var bloqueadores = new HashSet<string>(StringComparer.Ordinal);
            Adicionar(bloqueadores, SolidosSobrepostos(origem));
            Adicionar(bloqueadores, SolidosSobrepostos(destino));
            Vector2 delta = destino - origem;
            float distancia = delta.magnitude;
            if (distancia <= .000001f) return bloqueadores.ToArray();
            foreach (RaycastHit2D hit in Physics2D.CircleCastAll(origem, raio, delta / distancia, distancia,
                Physics2D.AllLayers))
            {
                if (!Solido(hit.collider)) continue;
                // Um contato apenas no fim e permitido se o destino nao tem penetracao significativa.
                if (hit.distance >= distancia - ToleranciaGeometria
                    && Profundidade(hit.collider, destino) <= ToleranciaGeometria) continue;
                // Hit inicial/rasante ambiguo e recusado: nunca usa um raio reduzido para liberar a aresta.
                Adicionar(bloqueadores, CaminhoObjeto(hit.collider.transform));
            }
            return bloqueadores.ToArray();
        }

        string[] PortaisSobrepostos(Vector2 centro, Collider2D escolhido)
        {
            var bloqueadores = new HashSet<string>(StringComparer.Ordinal);
            sobrepostos.Clear();
            Physics2D.OverlapCircle(centro, raio, filtroTriggers, sobrepostos);
            foreach (Collider2D colisor in sobrepostos)
                if (colisor != escolhido && portais.Contains(colisor) && Efetivo(colisor))
                    Adicionar(bloqueadores, CaminhoObjeto(colisor.transform));
            return bloqueadores.ToArray();
        }

        string[] VarreduraPortais(Vector2 origem, Vector2 destino, Collider2D escolhido)
        {
            var bloqueadores = new HashSet<string>(StringComparer.Ordinal);
            Adicionar(bloqueadores, PortaisSobrepostos(origem, escolhido));
            Adicionar(bloqueadores, PortaisSobrepostos(destino, escolhido));
            Vector2 delta = destino - origem;
            float distancia = delta.magnitude;
            if (distancia <= .000001f || portais.Count == 0) return bloqueadores.ToArray();
            hitsTriggers.Clear();
            Physics2D.CircleCast(origem, raio, delta / distancia, filtroTriggers, hitsTriggers, distancia);
            foreach (RaycastHit2D hit in hitsTriggers)
                if (hit.collider != escolhido && portais.Contains(hit.collider) && Efetivo(hit.collider))
                    Adicionar(bloqueadores, CaminhoObjeto(hit.collider.transform));
            return bloqueadores.ToArray();
        }

        bool TemContato(Vector2 centro, Collider2D contato, float margem)
        {
            float raioContato = raio - margem;
            if (raioContato <= 0f) return false;
            sobrepostos.Clear();
            // Filtro local inclui triggers mesmo se queriesHitTriggers global estiver desativado.
            Physics2D.OverlapCircle(centro, raioContato, filtroTriggers, sobrepostos);
            return sobrepostos.Contains(contato);
        }

        bool Objetivo(Vector2 raiz, Vector2 destino, Collider2D contato, float margem)
        {
            return contato != null ? TemContato(raiz + offset, contato, margem)
                : (raiz - destino).sqrMagnitude <= .08f * .08f;
        }

        int[] Conectores(Vector2 origem, Collider2D contato, HashSet<string> bloqueadores)
        {
            Vector2 centroOrigem = origem + offset;
            int x = Mathf.RoundToInt((centroOrigem.x - min.x) / passo);
            int y = Mathf.RoundToInt((centroOrigem.y - min.y) / passo);
            var encontrados = new List<int>();
            for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = x + dx, ny = y + dy;
                    if (nx < 0 || nx >= largura || ny < 0 || ny >= altura) continue;
                    int indice = ny * largura + nx;
                    if (!livre[indice]) { Adicionar(bloqueadores, bloqueiosNos[indice]); continue; }
                    string[] solidos = VarreduraSolidos(centroOrigem, Centro(indice));
                    Adicionar(bloqueadores, solidos);
                    if (solidos.Length > 0) continue;
                    string[] transicoes = VarreduraPortais(centroOrigem, Centro(indice), contato);
                    Adicionar(bloqueadores, transicoes);
                    if (transicoes.Length == 0) encontrados.Add(indice);
                }
            return encontrados.OrderBy(i => (Centro(i) - centroOrigem).sqrMagnitude).ToArray();
        }

        Vector2 Centro(int indice)
        {
            return min + new Vector2(indice % largura * passo, indice / largura * passo);
        }

        int Vizinho(int indice, int direcao)
        {
            int x = indice % largura + Dx[direcao], y = indice / largura + Dy[direcao];
            return x >= 0 && x < largura && y >= 0 && y < altura ? y * largura + x : -1;
        }

        void RotularComponentes()
        {
            int componente = 0;
            var fila = new Queue<int>();
            for (int i = 0; i < livre.Length; i++)
            {
                if (!livre[i] || componentes[i] >= 0) continue;
                componentes[i] = componente++;
                fila.Enqueue(i);
                while (fila.Count > 0)
                {
                    int atual = fila.Dequeue();
                    for (int direcao = 0; direcao < 4; direcao++)
                    {
                        if ((arestas[atual] & (1 << direcao)) == 0) continue;
                        int vizinho = Vizinho(atual, direcao);
                        if (componentes[vizinho] >= 0) continue;
                        componentes[vizinho] = componentes[atual];
                        fila.Enqueue(vizinho);
                    }
                }
            }
        }

        Vector2[] Caminho(Vector2 origem, int fim, int[] pais)
        {
            var caminho = new List<Vector2>();
            for (int atual = fim; atual >= 0; atual = pais[atual]) caminho.Add(Centro(atual) - offset);
            caminho.Reverse();
            if (caminho.Count == 0 || (caminho[0] - origem).sqrMagnitude > .0000000001f)
                caminho.Insert(0, origem);
            else caminho[0] = origem;
            return caminho.ToArray();
        }
    }

    public static Mapa Criar(GameObject player, Bounds fundo, float passo = .125f)
    {
        Exigir(player != null, "PLAYER_AUSENTE.");
        Exigir(Finito(passo) && passo > 0f, "PASSO_INVALIDO.");
        Exigir(Finito((Vector2)fundo.min) && Finito((Vector2)fundo.max), "BOUNDS_INVALIDO.");
        Physics2D.SyncTransforms();
        Collider2D[] corpos = player.GetComponentsInChildren<Collider2D>(true)
            .Where(c => c.enabled && c.gameObject.activeInHierarchy && !c.isTrigger).ToArray();
        Exigir(corpos.Length == 1 && corpos[0] is CircleCollider2D,
            "CORPO_NAO_SUPORTADO: exige exatamente um CircleCollider2D solido ativo no Player.");
        Exigir(corpos[0].attachedRigidbody == null || corpos[0].attachedRigidbody.simulated,
            "CORPO_NAO_SIMULADO.");
        return new Mapa(player, (CircleCollider2D)corpos[0], fundo, passo);
    }

    static long ChaveAresta(int a, int b)
    {
        return ((long)Math.Min(a, b) << 32) | (uint)Math.Max(a, b);
    }

    static Resultado Falha(string erro, int visitados, HashSet<string> bloqueadores)
    {
        return new Resultado { erro = erro, visitados = visitados, bloqueadores = bloqueadores.Take(8).ToArray() };
    }

    static Resultado Sucesso(Vector2[] pontos, int visitados, HashSet<string> bloqueadores)
    {
        return new Resultado { sucesso = true, pontos = pontos, visitados = visitados,
            bloqueadores = bloqueadores.Take(8).ToArray() };
    }

    static void Adicionar(HashSet<string> destino, string valor)
    {
        if (destino.Count < 8 && !string.IsNullOrEmpty(valor)) destino.Add(valor);
    }

    static void Adicionar(HashSet<string> destino, IEnumerable<string> valores)
    {
        foreach (string valor in valores) { Adicionar(destino, valor); if (destino.Count >= 8) break; }
    }

    static string CaminhoObjeto(Transform transform)
    {
        var nomes = new List<string>();
        for (Transform atual = transform; atual != null; atual = atual.parent) nomes.Add(atual.name);
        nomes.Reverse();
        return string.Join("/", nomes);
    }

    static bool Finito(float valor) { return !float.IsNaN(valor) && !float.IsInfinity(valor); }
    static bool Finito(Vector2 valor) { return Finito(valor.x) && Finito(valor.y); }
    static void Exigir(bool condicao, string mensagem)
    {
        if (!condicao) throw new InvalidOperationException("[AnaliseCirculacao] " + mensagem);
    }
}
