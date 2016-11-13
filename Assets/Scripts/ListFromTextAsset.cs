using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using UnityEngine.UI;
[RequireComponent(typeof(TextMesh))]

[ExecuteInEditMode]
public class ListFromTextAsset : MonoBehaviour
{
    public bool Activate;
    public int showText = 0;
    public TextAsset TextFile;
    private string theWholeFileAsOneLongString;
    public List<string> item;
 
    private bool hasInit;

    [Multiline]
    public string editingText, gameText;
    public bool editing = true;
 


    public  void Init()
    {
        if (TextFile != null)
        {
            //get the whole file
            theWholeFileAsOneLongString = TextFile.text;
            //get the table name
            string[] tableInput = theWholeFileAsOneLongString.Split("="[0]);         
            this.gameObject.name = tableInput[0];
            if (tableInput.Length >= 1)
            {                
                // population the data list
                item = new List<string>();
                //replace spaces
                tableInput[1] = tableInput[1].Replace(", ", ",");
                tableInput[1] = tableInput[1].Replace(" ,", ",");
                //replace line feeds
                tableInput[1] = tableInput[1].Replace("\n", "");
                // add items to list by spliting string by comma delimiter
                item.AddRange(tableInput[1].Split(","[0]));

                for (var i = 0; i < item.Count; i++)
                {
                    //strip extra code
                    item[i] = item[i].Replace("{", "");
                    item[i] = item[i].Replace("}", "");
                    // remove extra quotes
                    item[i] = item[i].Replace("\"", "");
                }
                showText = 0;
                gameText = item[showText];
                editingText = gameText;
            
                hasInit = true;
                int kWords = item.Count;
            }
         
            // Debug.Log(items[kWords - 1]);
        }
    }
    // only works in editor
 /*   void ListFolder() {
        string currentFile = AssetDatabase.GetAssetPath(TextFile);           
        string currentFolder = Path.GetDirectoryName(currentFile);
        DirectoryInfo dir = new DirectoryInfo(currentFolder);
       // Debug.Log(dir);
        FileInfo[] info = dir.GetFiles("*.*");
        foreach (FileInfo f in info)
        {
        }
    }*/

    void TextMeshUpdate()
    {
        TextMesh tTextmesh = this.GetComponent<TextMesh>();
        if (!tTextmesh) { this.gameObject.AddComponent<TextMesh>(); }
        tTextmesh.alignment = TextAlignment.Center;
        if (tTextmesh == null){ return; }

        if (Application.isPlaying)
        {
            tTextmesh.text = gameText;
        }
        else
        {
            tTextmesh.text = editingText;
        }
    }
    // Use this for initialization
    void Start()
    {
        editing = false;
        Init();
        TextMeshUpdate();
    }

    void Awake()
    {
        //Init();
    }

  void  incrementShowText(int showText)
    {
        if (showText > item.Count - 1 || showText <=-1)
            {
           showText = 0;
            }
            else
            {
                showText++;
            }     
    }
    void Update() {
        if(Activate && !hasInit && !editing) 
        {
            Init();
            TextMeshUpdate();
            Activate = false;
            hasInit = true;  
        }

       if (hasInit && !editing)
        {
            gameText = item[showText];
            editingText = gameText;
            TextMeshUpdate();         
        }

}

}
