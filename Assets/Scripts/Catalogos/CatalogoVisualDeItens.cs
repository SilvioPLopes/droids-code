using System;
using System.Collections.Generic;
using UnityEngine;

namespace DroidsCode.DroidCore
{
    /// <summary>Somente aparência. IDs e regras continuam em CatalogoDeItens.</summary>
    [CreateAssetMenu(menuName="DroidsCode/Catálogos/Visual de itens",fileName="VisuaisDeItens")]
    public sealed class CatalogoVisualDeItens : ScriptableObject
    {
        public const string Recurso="Catalogos/VisuaisDeItens";

        [Serializable]
        public sealed class Entrada
        {
            [Tooltip("ID exato de CatalogoDeItens. Não usar o nome traduzido.")]
            public string id;
            [Tooltip("Nome de consulta no Inspector; não altera o nome real do item.")]
            public string nomeParaConsulta;
            [Tooltip("Arraste aqui a sprite exclusiva deste item para substituir sua aparência.")]
            public Sprite sprite;
            [TextArea,Tooltip("Referência de procedência da arte inicial; atualizar se trocar a imagem.")]
            public string origem;
        }

        public List<Entrada> itens=new List<Entrada>();

        public Sprite Encontrar(string id)
        {
            if(string.IsNullOrEmpty(id))return null;
            foreach(var entrada in itens)
                if(entrada!=null&&entrada.id==id)return entrada.sprite;
            return null;
        }

        public static Sprite Obter(string id)
        {
            var catalogo=Resources.Load<CatalogoVisualDeItens>(Recurso);
            return catalogo==null?null:catalogo.Encontrar(id);
        }
    }
}
