using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
// Custom Editor.

// Automatic handling of multi-object editing, undo, and prefab overrides.
[CustomEditor(typeof(RandomizeCategories))]

[CanEditMultipleObjects]
[ExecuteInEditMode]
public class RandomizeCategoriesGUI : Editor {

     public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
         GUILayout.Label("\nAt runtime : attaches RandomTextMovement\nto Objects tagged Category");
   
        GUILayout.Label("Sets List_TextMesh textTimer speed");
        /*    
           RandomizeCategories myScript = (RandomizeCategories)target;
           if (GUILayout.Button("Start"))
           {
               myScript.Init();
           }
     */
    }
}
#endif