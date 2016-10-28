using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;




public class GetTagList : EditorWindow
{
     private Vector2 scroll;
     private float myFloat = 0.0f;
     private bool toggleSwitch = false;
    private bool groupEnabled = false;
    private bool toggleButton = false;

    public List<string> tagList = new List<string>();
 
    [Multiline]
     public string myString;
    

    // class MyWindow : EditorWindow {
    [MenuItem("Window/My Window GetTagList")]
  

    
    // Use this for initialization

    public static void ShowWindow()

    {
        EditorWindow.GetWindow(typeof(GetTagList));
       
    }
    public  void GetTags()
    {
        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
        {
            tagList.Add(UnityEditorInternal.InternalEditorUtility.tags[i]);
        }
    }
    void Awake()
    {
        GetTags();
    }
    void OnGUI()
    {
        // The actual window code goes here
  //
        GUILayout.Label("Example HelpBox Command", EditorStyles.miniBoldLabel);
        EditorGUILayout.HelpBox("EditorGUILayout.HelpBox('text',MessageType.Info);", MessageType.Info);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        GUILayout.Space(20);
        GUILayout.Label("Example GUI miniBoldLabel Script command:", EditorStyles.miniBoldLabel);
        GUILayout.Label("GUILayout.Label('string'),EditorStyles.miniBoldLabel", EditorStyles.boldLabel);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        GUILayout.Space(20);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Example groupEnabled.", groupEnabled);
        toggleSwitch = EditorGUILayout.Toggle("Toggle", toggleSwitch);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(20);

        if (toggleButton == true)
        {
            toggleButton = GUILayout.Toggle(toggleButton, "Toggle me !", "Button");
            GUILayout.Space(20);

            GUILayout.Label("Example MultiLine  TextArea :", EditorStyles.boldLabel);
            scroll = EditorGUILayout.BeginScrollView(scroll);

            myString = EditorGUILayout.TextArea(string.Join("\n", tagList.ToArray()), GUILayout.Height(position.height - 30));
            EditorGUILayout.EndScrollView();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        }
      else
        {
            toggleButton = GUILayout.Toggle(toggleButton, "Toggle me !", "Button");
            GUILayout.Space(20);

            GUILayout.Label("Example MultiLine  TextArea :", EditorStyles.boldLabel);
            scroll = EditorGUILayout.BeginScrollView(scroll);

            myString = EditorGUILayout.TextArea("\n", GUILayout.Height(position.height - 30));

            EditorGUILayout.EndScrollView();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

           

           
        }
   
    }
  //   }
	
	
	
	
}
