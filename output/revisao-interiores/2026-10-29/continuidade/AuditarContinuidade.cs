using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Somente inspeção de cenas em PreviewScene. Não salva/importa/muta assets.
public static class AuditarContinuidade
{
    const string Out = "output/revisao-interiores/2026-10-29/continuidade/";
    static string Caminho(Transform t) => t.parent == null ? t.name : Caminho(t.parent)+"/"+t.name;
    static object XY(Vector2 p) => new { x=p.x, y=p.y };
    static object Pixel(SpriteRenderer bg, Vector3 pos)
    {
        if(bg==null || bg.sprite==null) return null;
        var v=bg.transform.InverseTransformPoint(pos);var s=bg.sprite;
        return new { x=v.x*s.pixelsPerUnit+s.pivot.x, y=s.rect.height-(v.y*s.pixelsPerUnit+s.pivot.y) };
    }
    public static object Run(int start, int count)
    {
        if(Application.dataPath.Replace('\\','/')!="C:/Users/silve/DroidsCode/Assets" || EditorApplication.isPlaying)
            throw new Exception("Projeto/modo inesperado");
        var config=JObject.Parse(File.ReadAllText("output/montagem-ferrovale/2026-10-29/config-montagem.json"));
        var paths=config["rooms"].Select(r=>(string)r["scene"]).Concat(new[]{"Assets/Scenes/CasaLip.unity"}).ToArray();
        var done=new System.Collections.Generic.List<string>();
        for(int i=start;i<Math.Min(start+count,paths.Length);i++)
        {
            var scene=EditorSceneManager.OpenPreviewScene(paths[i]);
            try
            {
                var roots=scene.GetRootGameObjects();
                var all=roots.SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();
                var bg=all.Select(t=>t.GetComponent<SpriteRenderer>()).FirstOrDefault(s=>s!=null && s.sprite!=null && AssetDatabase.GetAssetPath(s.sprite).Contains("/Mapas/"));
                var player=all.FirstOrDefault(t=>t.CompareTag("Player"));
                var circles=player!=null?player.GetComponents<CircleCollider2D>():new CircleCollider2D[0];
                var portals=all.Select(t=>t.GetComponent<TransicaoDeCena>()).Where(t=>t!=null).ToArray();
                var result=new {
                    scene=scene.name,path=paths[i],
                    background=bg==null?null:new {path=AssetDatabase.GetAssetPath(bg.sprite),width=bg.sprite.rect.width,height=bg.sprite.rect.height,ppu=bg.sprite.pixelsPerUnit,position=XY(bg.transform.position),scale=XY(bg.transform.lossyScale)},
                    player=player==null?null:new {position=XY(player.position),foot=circles.Select(c=>new{position=XY(c.transform.TransformPoint(c.offset)),radius=c.radius*Mathf.Max(Mathf.Abs(c.transform.lossyScale.x),Mathf.Abs(c.transform.lossyScale.y)),c.isTrigger}).ToArray()},
                    portals=portals.Select(t=>new {path=Caminho(t.transform),t.enabled,active=t.gameObject.activeInHierarchy,t.trancada,target=t.nomeCenaDestino,arrival=XY(t.pontoDeChegada),position=XY(t.transform.position),pixel=Pixel(bg,t.transform.position),colliders=t.GetComponents<Collider2D>().Select(c=>new{type=c.GetType().Name,c.enabled,c.isTrigger,center=XY(c.bounds.center),size=XY(c.bounds.size)}).ToArray()}).ToArray(),
                    colliders=all.SelectMany(t=>t.GetComponents<Collider2D>()).Where(c=>!c.isTrigger && (player==null||!c.transform.IsChildOf(player))).Select(c=>new{path=Caminho(c.transform),type=c.GetType().Name,c.enabled,active=c.gameObject.activeInHierarchy,center=XY(c.transform.TransformPoint(c.offset)),rotation=c.transform.eulerAngles.z,scale=XY(c.transform.lossyScale),size=c is BoxCollider2D b?XY(b.size):null,radius=c is CircleCollider2D ci?ci.radius:0,edgeRadius=c is EdgeCollider2D ed?ed.edgeRadius:0,points=c is EdgeCollider2D e?e.points.Select(v=>XY(e.transform.TransformPoint(v+e.offset))).ToArray():null,polygon=c is PolygonCollider2D poly?Enumerable.Range(0,poly.pathCount).Select(k=>poly.GetPath(k).Select(v=>XY(poly.transform.TransformPoint(v+poly.offset))).ToArray()).ToArray():null}).ToArray(),
                    special=all.Select(t=>t.GetComponent<GatilhoDeDialogo>()).Where(g=>g!=null&&(g.name.Contains("Escada")||g.name.Contains("Quarto"))).Select(g=>new{path=Caminho(g.transform),pixel=Pixel(bg,g.transform.position)}).ToArray(),
                    visits=all.Select(t=>t.GetComponent<MoradorInterativo>()).Where(n=>n!=null&&!string.IsNullOrWhiteSpace(n.destinoDeVisita)).Select(n=>new{path=Caminho(n.transform),n.idNpc,target=n.destinoDeVisita,arrival=XY(n.chegadaDaVisita)}).ToArray()
                };
                File.WriteAllText(Out+scene.name+".json",JsonConvert.SerializeObject(result,Formatting.Indented));
                done.Add(scene.name);
            }
            finally { EditorSceneManager.ClosePreviewScene(scene); }
        }
        return new{inspected=done,saveCalls=0,play=false};
    }
}
