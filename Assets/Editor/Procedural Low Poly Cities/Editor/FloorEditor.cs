using UnityEditor;
using UnityEngine;

namespace ProceduralCity
{
    [CustomEditor(typeof(Floor))]
    public class FloorEditor : Editor
    {
        private GUIContent cont;
        private static Floor instance;

        void Awake()
        {
            instance = (Floor)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            cont = new GUIContent("Floor Height: ", "changes the height of the current floor");
            instance.floorHeight = EditorGUILayout.FloatField(cont, instance.floorHeight, GUILayout.ExpandWidth(true));
            cont = new GUIContent("Window Box Width: ", "");
            instance.windowBoxWidth = EditorGUILayout.FloatField(cont, instance.windowBoxWidth, GUILayout.ExpandWidth(true));
            cont = new GUIContent("Window Height: ", "changes the height of windows on front and back sides of the current floor");
            instance.windowHeight = EditorGUILayout.FloatField(cont, instance.windowHeight, GUILayout.ExpandWidth(true));
            cont = new GUIContent("Window Width: ", "changes the width of windows on front and back sides of the current floor");
            instance.windowWidth = EditorGUILayout.FloatField(cont, instance.windowWidth, GUILayout.ExpandWidth(true));
            cont = new GUIContent("Side Window Box Width: ", "");
            instance.sideWinBoxWidth = EditorGUILayout.FloatField(cont, instance.sideWinBoxWidth, GUILayout.ExpandWidth(true));
            cont = new GUIContent("Side Window Width: ", "changes the width of windows on left and right sides of the current floor");
            instance.sideWindowWidth = EditorGUILayout.FloatField(cont, instance.sideWindowWidth, GUILayout.ExpandWidth(true));
            cont = new GUIContent("Side Window Height: ", "changes the height of windows on left and right sides of the current floor");
            instance.sideWindowHeight = EditorGUILayout.FloatField(cont, instance.sideWindowHeight, GUILayout.ExpandWidth(true));
            cont = new GUIContent("Separator Height: ", "changes the height of the separator (if there is one) on the current floor");
            instance.separatorHeight = EditorGUILayout.FloatField(cont, instance.separatorHeight, GUILayout.ExpandWidth(true));
            cont = new GUIContent("Floor Changed: ", "if true, the floor won't change if the building change");
            instance.floorChanged = EditorGUILayout.Toggle(cont, instance.floorChanged, GUILayout.ExpandWidth(true));
            /*cont = new GUIContent("Balconies: ", "if toggled, build a balcony in the correspondent position");
            for (int i = 1; i < instance.GetComponentInParent<Building>().balconyDic["Floor" + instance.floorNumber].Count; i++)
            {
                cont = new GUIContent("front" + i + ": ", "");
                instance.GetComponentInParent<Building>().balconyDic["Floor" + instance.floorNumber][i] = EditorGUILayout.Toggle(cont, instance.floorChanged, GUILayout.ExpandWidth(true));
            }*/
            if (GUI.changed)
                instance.Generate();
        }
    }
}