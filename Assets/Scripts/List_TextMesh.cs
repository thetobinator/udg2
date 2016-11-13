using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TextMesh))]
//[ExecuteInEditMode]
public class List_TextMesh : MonoBehaviour
{
    private bool editing = true;
    public bool Activate;
    public int showText = 0;
    public float textTimer = 0.0f;
    private float currentTime = 0.0f;
    private bool hasInit;
   
    [Multiline]
    public string editingText, gameText;
    public List<string> item;

    public void Init()
    {     
        var ls = this.gameObject.GetComponent<ListFromTextAsset>();
        if (ls)
        {
            item = new List<string>();
            item = ls.item;
           // DestroyImmediate(ls);
        }
        Activate = false;
        hasInit = true;
    }


    void incrementShowText(int i)
    {
        if (i > item.Count - 2 || i <= -1)
        {
            showText = 0;
        }
        else
        {
            showText++;
        }
    }

    /*
                // add items to list by spliting string by comma delimiter
                item.AddRange(tableInput[1].Split(","[0]));

    */

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
        currentTime = Time.time;
        editing = false;
        Init();
        TextMeshUpdate();
    }

    void Awake()
    {
        //Init();
    }

 

    void Update() {
        //hasn't initialized while in play mode
        if(Activate && !hasInit && !editing) 
        {
            Init();
            Activate = false;
            hasInit = true;  
        }

        //has initilizaed and is play mode
        if (hasInit && !editing)
        {
            if (textTimer > 0.0f)
            {
                float timedTextPassed = Time.time - currentTime;
                if (timedTextPassed >= textTimer)
                {
                    currentTime = Time.time;
                    incrementShowText(showText);
                }
            }
            if (showText <= item.Count - 1 && showText >=0)
            {
                gameText = item[showText];
                editingText = gameText;
            }
            TextMeshUpdate();         
        }
}
}
