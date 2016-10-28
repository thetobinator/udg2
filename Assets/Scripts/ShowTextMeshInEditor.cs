using UnityEngine;
//using UnityEditor;
using System.Collections;

[RequireComponent(typeof(TextMesh))]

[ExecuteInEditMode]
public class ShowTextMeshInEditor : MonoBehaviour {
    [Multiline]
    public string editingText, gameText;
 
    public bool editing = true;

    void TextMeshUpdate()
    {
        TextMesh tTextmesh = this.GetComponent<TextMesh>();
        if (Application.isPlaying)
        { tTextmesh.text = gameText; 
        }
        else
        {
            tTextmesh.text = editingText; 
        }
    }
	// Use this for initialization
    void Start()
    {
        TextMeshUpdate();
    }

   

	// Update is called once per frame
	void Update ()
    {
        if (this.tag == "SpawnPoint_Human")
        {
            this.editingText = "H\nU\nM\nA\nN";
                
        }
        else
        {
            this.editingText = "Z\nO\nM\nB\nI\nE";
        }
        TextMeshUpdate(); 
     
    }


}
