using System;
using System.Linq;
using DroidsCode.DroidCore;
using UnityEditor;
using UnityEngine;

/// <summary>Importação inicial dos novos assets; não modifica cena, estoque nem originais.</summary>
public static class OrganizacaoVisualDeItens
{
    public const string Catalogo="Assets/Resources/Catalogos/VisuaisDeItens.asset";
    static readonly string[] Grupos={"Consumo","Tecnologia","Especiais"};
    static readonly string[][] Ids={
        new[]{"pocao_pequena","pocao_media","pocao_grande","reparo_completo","escudo_de_emergencia","oleo_de_precisao","sobrecarga_temp","granada_de_acido","corrosivo"},
        new[]{"disruptor_emp","nucleo_de_energia","servo_motor","sensor_optico","placa_de_blindagem","giroscopio","sinalizador_de_retorno","kit_de_acampamento","modulo_de_treinamento"},
        new[]{"chave_de_acesso","nucleo_sobrecarga","modulo_ressonancia","fio_de_cobre","filtro_de_agua","sucata_limpa","emblema_da_guilda","bilhete_do_ivo","peca_perna_apollo"}
    };

    [MenuItem("DroidsCode/Itens/Abrir catálogo visual")]
    public static void Abrir()
    {
        var alvo=AssetDatabase.LoadAssetAtPath<CatalogoVisualDeItens>(Catalogo);
        if(alvo==null)throw new InvalidOperationException("Catálogo visual ainda não importado.");
        Selection.activeObject=alvo;EditorGUIUtility.PingObject(alvo);
    }

    public static string Aplicar()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Importar fora de Play.");
        if(AssetDatabase.LoadMainAssetAtPath(Catalogo)!=null)throw new InvalidOperationException("Catálogo já existe: não sobrescrever escolhas do Inspector.");
        if(Ids.SelectMany(x=>x).Distinct().Count()!=CatalogoDeItens.Todos.Count||Ids.SelectMany(x=>x).Any(x=>CatalogoDeItens.Obter(x)==null))
            throw new InvalidOperationException("Mapeamento não corresponde ao catálogo conhecido.");
        var texturas=new Texture2D[3];
        var recortes=new Rect[3][];
        for(int g=0;g<3;g++)
        {
            string path=$"Assets/sprites/Itens/{Grupos[g]}/{Grupos[g].ToLowerInvariant()}.png";
            var importer=AssetImporter.GetAtPath(path) as TextureImporter;
            if(importer==null)throw new InvalidOperationException("Folha não importada: "+path);
            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
            importer.maxTextureSize=2048;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;
            importer.textureCompression=TextureImporterCompression.Uncompressed;importer.filterMode=FilterMode.Point;
            importer.SaveAndReimport();texturas[g]=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if(texturas[g].width!=texturas[g].height)throw new InvalidOperationException("Folha deve ser quadrada: "+path);
            recortes[g]=MedirRecortes(path);
            if(g==0)
            {
                // A família de cura usa a mesma janela: o frasco pequeno continua menor na UI.
                float lado=recortes[g].Take(3).Max(r=>Mathf.Max(r.width,r.height));
                for(int i=0;i<3;i++)
                {
                    var centro=recortes[g][i].center;
                    recortes[g][i]=new Rect(Mathf.Clamp(Mathf.Round(centro.x-lado/2),0,texturas[g].width-lado),
                        Mathf.Clamp(Mathf.Round(centro.y-lado/2),0,texturas[g].height-lado),lado,lado);
                }
            }
            foreach(var id in Ids[g])if(AssetDatabase.LoadMainAssetAtPath($"Assets/sprites/Itens/Recortes/{id}.asset")!=null)
                throw new InvalidOperationException("Recorte já existe: "+id);
        }
        var catalogo=ScriptableObject.CreateInstance<CatalogoVisualDeItens>();
        for(int g=0;g<3;g++)for(int i=0;i<9;i++)
        {
            var id=Ids[g][i];var rect=recortes[g][i];
            var sprite=Sprite.Create(texturas[g],rect,Vector2.one*.5f,100,0,SpriteMeshType.FullRect);sprite.name=id;
            AssetDatabase.CreateAsset(sprite,$"Assets/sprites/Itens/Recortes/{id}.asset");
            catalogo.itens.Add(new CatalogoVisualDeItens.Entrada{id=id,nomeParaConsulta=CatalogoDeItens.Obter(id).Nome,sprite=sprite,
                origem=$"Folha {Grupos[g]}, linha {i/3+1}, coluna {i%3+1}, recorte {rect}; geração baseada nas referências 3/17 de Atualizados/novos."});
        }
        AssetDatabase.CreateAsset(catalogo,Catalogo);AssetDatabase.SaveAssetIfDirty(catalogo);
        return Validar();
    }

    // O gerador pode mudar dimensões e ultrapassar a célula nominal. Medir o alpha real evita cortar bicos/antenas.
    static Rect[] MedirRecortes(string path)
    {
        var leitura=new Texture2D(2,2,TextureFormat.RGBA32,false);
        try
        {
            ImageConversion.LoadImage(leitura,System.IO.File.ReadAllBytes(path));
            int w=leitura.width,h=leitura.height;var pixels=leitura.GetPixels32();
            var visitados=new bool[pixels.Length];var fila=new int[pixels.Length];
            var minimo=Enumerable.Repeat(new Vector2Int(w,h),9).ToArray();
            var maximo=Enumerable.Repeat(new Vector2Int(-1,-1),9).ToArray();
            for(int inicio=0;inicio<pixels.Length;inicio++)
            {
                if(visitados[inicio])continue;visitados[inicio]=true;if(pixels[inicio].a<=8)continue;
                int cabeca=0,cauda=1;fila[0]=inicio;int xmin=w,ymin=h,xmax=-1,ymax=-1,contagem=0;
                void Visitar(int n){if(visitados[n])return;visitados[n]=true;if(pixels[n].a>8)fila[cauda++]=n;}
                while(cabeca<cauda)
                {
                    int n=fila[cabeca++],x=n%w,y=n/w;contagem++;
                    xmin=Math.Min(xmin,x);ymin=Math.Min(ymin,y);xmax=Math.Max(xmax,x);ymax=Math.Max(ymax,y);
                    if(x>0)Visitar(n-1);if(x<w-1)Visitar(n+1);if(y>0)Visitar(n-w);if(y<h-1)Visitar(n+w);
                }
                if(contagem<16)continue;
                int coluna=Mathf.Clamp(((xmin+xmax)/2)*3/w,0,2),linha=Mathf.Clamp((h-1-(ymin+ymax)/2)*3/h,0,2),i=linha*3+coluna;
                minimo[i]=Vector2Int.Min(minimo[i],new Vector2Int(xmin,ymin));maximo[i]=Vector2Int.Max(maximo[i],new Vector2Int(xmax,ymax));
            }
            var resultado=new Rect[9];
            for(int i=0;i<9;i++)
            {
                if(maximo[i].x<0)throw new InvalidOperationException("Ícone não detectado na folha: "+path+" célula "+i);
                int x=Math.Max(0,minimo[i].x-8),y=Math.Max(0,minimo[i].y-8);
                resultado[i]=new Rect(x,y,Math.Min(w,maximo[i].x+9)-x,Math.Min(h,maximo[i].y+9)-y);
            }
            return resultado;
        }
        finally{UnityEngine.Object.DestroyImmediate(leitura);}
    }

    public static string Validar()
    {
        var catalogo=AssetDatabase.LoadAssetAtPath<CatalogoVisualDeItens>(Catalogo);
        if(catalogo==null)throw new InvalidOperationException("Catálogo visual ausente.");
        var entradas=catalogo.itens;
        if(entradas.Count!=CatalogoDeItens.Todos.Count||entradas.Any(x=>x==null||x.sprite==null||CatalogoDeItens.Obter(x.id)==null)
            ||entradas.Select(x=>x.id).Distinct().Count()!=entradas.Count||entradas.Select(x=>x.sprite).Distinct().Count()!=entradas.Count)
            throw new InvalidOperationException("Referência ausente ou ID/sprite repetido.");
        if(entradas.Select(x=>new{Textura=x.sprite.texture,Recorte=x.sprite.rect}).Distinct().Count()!=entradas.Count)
            throw new InvalidOperationException("Recorte visual reutilizado em mais de um item.");
        foreach(var item in CatalogoDeItens.Todos.Values)
            if(CatalogoVisualDeItens.Obter(item.Id)!=catalogo.Encontrar(item.Id)||catalogo.Encontrar(item.Id)==null)
                throw new InvalidOperationException("Resolução runtime inválida: "+item.Id);
        return $"OK: {entradas.Count} IDs com sprites/recortes distintos; {CatalogoDeItens.ItensDaLoja.Count()} produtos na loja, estoque/catálogo lógico preservados.";
    }
}
