using UnityEditor;
using UnityEngine;
using Prefabs;

namespace ProceduralCity
{
    [CustomEditor(typeof(Building))]
    public class BuildingEditor : Editor
    {
        GUIStyle iconGUIStyle;

        [MenuItem("GameObject/Create Procedural/Procedural Building")]
        static void CreateProceduralBuilding()
        {
            var procBuilding = new GameObject(string.Format("Building_{0:X4}", Random.Range(0, 65536))).AddComponent<Building>();
            procBuilding.seed = Random.Range(0, 65536);
            procBuilding.RandomizeSettings();
            procBuilding.Generate();
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Initialise editor data
        // ---------------------------------------------------------------------------------------------------------------------------

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
            var building = (Building)target;

            /*var pt = PrefabUtility.GetPrefabType(building);
            if (pt != PrefabType.None && pt != PrefabType.DisconnectedPrefabInstance) // Prefabs are not dynamic
            {
                GUI.enabled = false;
                DrawDefaultInspector();
                return;
            }*/

            DrawDefaultInspector();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Rand Building")) // Randomize all building parameters
                {
                    Undo.RecordObject(building, "Random building " + building.name);
                    building.RandomizeSettings();
                    building.Generate();
                }

                if (GUI.changed)
                    building.Generate();

                if (GUILayout.Button("Create Prefab"))
                    PrefabCreator.CreatePrefab(building.gameObject);
                
            }
            GUILayout.EndHorizontal();

        }

    }
}