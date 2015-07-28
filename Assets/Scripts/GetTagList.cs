using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
public class GetTagList : MonoBehaviour {
    public List<string> tagList = new List<string>();
	// Use this for initialization


    class MyWindow : EditorWindow
    {
        [MenuItem("Window/My Window")]

        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(MyWindow));
        }

        void OnGUI()
        {
            // The actual window code goes here
        }
    }


	void Start () {
        //string to_search_tag = "Player";
        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
        {
            //print(UnityEditorInternal.InternalEditorUtility.tags[i]);
            tagList.Add(UnityEditorInternal.InternalEditorUtility.tags[i]);
            /*if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(to_search_tag))
            {
                Debug.Log("At Position " + i + " is the Tag " + to_search_tag + " found :) ");
                break;// attention : the first index is 0 !!!
            }
            else
            {
            }
             */
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
