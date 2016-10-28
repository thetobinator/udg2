using UnityEditor;
using UnityEngine;
using Prefabs;

namespace ProceduralCity
{
    [CustomEditor(typeof(ProcTree))]
    public class TreeProcEditor : Editor
    {
        GUIStyle iconGUIStyle;

        [MenuItem("GameObject/Create Procedural/Procedural Tree")]
        static void CreateProceduralTree()
        {
            var procTree = new GameObject(string.Format("Tree_{0:X4}", Random.Range(0, 65536))).AddComponent<ProcTree>();
            procTree.seed = Random.Range(0, 65536);
            procTree.RandomizeSettings();
            procTree.Generate(Random.Range(0f, 1f) <= 0.5f ? true : false);
        }

        void OnEnable()
        {
            if (iconGUIStyle == null)
            {
                iconGUIStyle = new GUIStyle() // Create a labelStyle for the notification icon
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 0,
                    fixedHeight = 16,
                    margin = new RectOffset(3, 3, 5, 0),
                    padding = new RectOffset(0, 1, -2, 0),
                    fontStyle = FontStyle.Bold,
                };
            }
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Draw ProceduralTree inspector GUI
        // ---------------------------------------------------------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            var tree = (ProcTree)target;

            /*var pt = PrefabUtility.GetPrefabType(tree);
            if (pt != PrefabType.None && pt != PrefabType.DisconnectedPrefabInstance) // Prefabs are not dynamic
            {
                GUI.enabled = false;
                DrawDefaultInspector();
                return;
            }*/

            DrawDefaultInspector();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Rand Tree")) // Randomize all tree parameters
                {
                    Undo.RecordObject(tree, "Random Tree " + tree.name);
                    tree.RandomizeSettings();
                    tree.Generate(Random.Range(0f, 1f) <= 0.5 ? true : false);
                    
                }

                if (GUILayout.Button("Rand Seed")) // Generates a tree of the same type
                {
                    Undo.RecordObject(tree, "Random Tree " + tree.name);
                    //tree.RandomizeSettings();
                    tree.seed = Random.Range(0, 64000);
                    tree.Generate(tree.coneTree);
                    
                }

                if (GUI.changed)
                    tree.Generate(tree.coneTree);
                
                if (GUILayout.Button("Create Prefab"))
                    PrefabCreator.CreatePrefab(tree.gameObject);

            }
            GUILayout.EndHorizontal();

        }

    }
}