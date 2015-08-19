using UnityEngine;
using UnityEditor;
using System.Collections;

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
        TextMeshUpdate(); 
    }


}
