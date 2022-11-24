using UnityEngine;
using UnityEngine.SceneManagement;
using Landfall.TABS;
using System.Collections.Generic;
using Pathfinding;
using System.Linq;

namespace AnimalKingdom
{
    public class AKSceneManager
    {
        public AKSceneManager()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        public void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name.Contains("AK_"))
            {
                GameObject astar = null;
                GameObject map = null;
                foreach (var obj in scene.GetRootGameObjects())
                {
                    if (obj.name == "AStar_Lvl1_Grid")
                    {
                        astar = obj;
                    }
                    if (obj.name == "Map")
                    {
                        map = obj;
                        var shadersToReplace = new List<MeshRenderer>(obj.GetComponentsInChildren<MeshRenderer>(true)
                            .ToList().FindAll(x => x.name.Contains("_ReplaceMe")));
                        foreach (var rend in shadersToReplace)
                        {
                            foreach (var mat in rend.materials)
                            {
                                mat.shader = Shader.Find(mat.shader.name);
                            }
                            if (rend.GetComponent<PiratePlacementTransparency>())
                            {
                                rend.GetComponent<PiratePlacementTransparency>().Materials[0].m_oldMaterial.shader =
                                    Shader.Find(rend.GetComponent<PiratePlacementTransparency>().Materials[0]
                                        .m_oldMaterial.shader.name);
                            }
                        }
                    }
                    if (obj.name.Contains("_ReplaceMe"))
                    {
                        obj.GetComponent<MeshRenderer>().material.shader =
                            Shader.Find(obj.GetComponent<MeshRenderer>().material.shader.name);
                    }
                    if (obj.name == "WaterManager")
                    {
                        obj.GetComponent<PirateWaterManager>().WaterMaterial = obj.GetComponent<MeshRenderer>().material;
                    }
                }
                if (astar != null && map != null)
                {
                    var path = astar.GetComponentInChildren<AstarPath>(true);
                    astar.SetActive(true);
                    if (path.data.graphs.Length > 0) { path.data.RemoveGraph(path.data.graphs[0]); }
                    path.data.AddGraph(typeof(RecastGraph));
                    path.data.recastGraph.minRegionSize = 0.1f;
                    path.data.recastGraph.characterRadius = 0.3f;
                    path.data.recastGraph.cellSize = 0.2f;
                    path.data.recastGraph.forcedBoundsSize = new Vector3(map.GetComponent<MapSettings>().m_mapRadius * 2f, map.GetComponent<MapSettings>().m_mapRadius * map.GetComponent<MapSettings>().mapRadiusYMultiplier * 2f, map.GetComponent<MapSettings>().m_mapRadius * 2f);
                    path.data.recastGraph.rasterizeMeshes = false;
                    path.data.recastGraph.rasterizeColliders = true;
                    path.data.recastGraph.mask = AKMain.kermate.LoadAsset<GameObject>("AStarDummy").GetComponent<Explosion>().layerMask;
                    path.Scan();

                    //path.data.GetNodes(delegate (GraphNode node)
                    //{
                    //    GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //    gameObject.transform.position = (Vector3)node.position;
                    //    gameObject.GetComponent<Renderer>().material.color = Color.green;
                    //    gameObject.GetComponent<Collider>().enabled = false;
                    //    gameObject.transform.localScale *= 0.5f;
                    //});
                }
            }
            if (scene.name == "05_Lvl1_Medieval_VC")
            {
                var secrets = new GameObject()
                {
                    name = "Secrets"
                };
                Object.Instantiate(AKMain.kermate.LoadAsset<GameObject>("GreaterDragon_Unlock"), secrets.transform, true);
            }
        }
	}
}
