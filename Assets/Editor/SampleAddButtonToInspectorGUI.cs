using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This adds a button to a script named to YourScriptName
/*
#if UNITY_EDITOR
using UnityEditor;
// Custom Editor.
[CustomEditor(typeof(YourScriptName))]

// Automatic handling of multi-object editing, undo, and prefab overrides.
[CanEditMultipleObjects]

[ExecuteInEditMode]
public class TransferListItemsGUI : Editor {

     public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
       // GUILayout.Label("\nNames a GameObject for Directory in /Resources/");
   

        if (GUILayout.Button("Transfer\n Child Lists Assets \n to this object"))
        {
            GameObject.Find("HostOfYourScript").GetComponent<YourScriptName>().SomeFunction();
        }

    }
    }
#endif
*/
