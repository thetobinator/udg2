using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
// Custom Editor.

// Automatic handling of multi-object editing, undo, and prefab overrides.
[CustomEditor(typeof(LoadTableText_ObjectNameIsFolderName))]

[CanEditMultipleObjects]
public class LoadTableNote : Editor {

     public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        GUILayout.Label("\nNames a GameObject for Directory in /Resources/");
        GUILayout.Label("Subfolders generate objects tagged 'Directory'");
        GUILayout.Label("TextAssets generate objects tagged 'Category'");
        GUILayout.Label("Comma delimited text is kept as list in object.");
        GUILayout.Label("List_TextMesh  script is added");
        GUILayout.Label("TextAsset file IO is removed after init process.\n");

        LoadTableText_ObjectNameIsFolderName myScript = (LoadTableText_ObjectNameIsFolderName)target;
        if (GUILayout.Button("Load Text Assets\n&\n Build Objects"))
        {   
            string path = EditorUtility.OpenFolderPanel("Choose Resources Folder Whose Subfolders Contains TextAssets Ending in .txt", "", "");
            string appPath = Application.dataPath;
          string folderName = path.Substring(path.LastIndexOf("/") + 1);

        if (folderName != null)
            {
                myScript.Rename(folderName);

                myScript.Init(); ;
            }
        }
    }
    }
#endif