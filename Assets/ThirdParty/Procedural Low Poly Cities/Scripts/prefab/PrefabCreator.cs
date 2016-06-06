using UnityEngine;
using System.Collections;
using UnityEditor;
using ProceduralCity;

namespace Prefabs
{
    public class PrefabCreator : MonoBehaviour
    {
        private static int index = 0;

        public static void CreatePrefab(GameObject obj)
        {
            CreatePrefab(null, obj);
            CreateRootPrefab("Assets/Procedural Low Poly Cities/Temporary/" + obj.name + "/" + obj.name + ".prefab", obj);
        }

        private static void CreatePrefab(string path, GameObject obj)
        {

            if (path == null)
            {
				string guid = AssetDatabase.CreateFolder("Assets/Procedural Low Poly Cities/Temporary", obj.name);
                path = AssetDatabase.GUIDToAssetPath(guid);
            }
            else
            {
                if (obj.GetComponent<MeshFilter>() != null)
                    SaveMesh(obj, path);
            }

            foreach (Transform go in obj.transform)
            {
                CreatePrefab(path, go.gameObject);
            }
        }

        private static void SaveMesh(GameObject obj, string path)
        {
            Mesh m1 = obj.GetComponent<MeshFilter>().sharedMesh;//update line1
            if (!AssetDatabase.Contains(m1))
                AssetDatabase.CreateAsset(m1, path + "/" + obj.name + index + "_M" + ".asset"); // update line2
            index++;
            
        }

        private static void CreateRootPrefab(string localPath, GameObject obj)
        {
            Object prefab = PrefabUtility.CreatePrefab(localPath, obj);
            PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.Refresh();
        }

    }
}