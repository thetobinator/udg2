using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class HumanNameLists : MonoBehaviour
{
    [HideInInspector]
    public List<string> femaleNames;
    [HideInInspector]
    public List<string> maleNames;
    [HideInInspector]
    public List<string> culturalNames;
    [HideInInspector]
    public List<string> lastNames;

    // Use this for initialization
    public void Transfer () {

      List_TextMesh fem = GameObject.Find("femaleNames").GetComponent<List_TextMesh>();
        List_TextMesh man = GameObject.Find("maleNames").GetComponent<List_TextMesh>();
        List_TextMesh cul = GameObject.Find("culturalNames").GetComponent<List_TextMesh>();
        List_TextMesh las = GameObject.Find("lastNames").GetComponent<List_TextMesh>();
        femaleNames = fem.item;
        maleNames = man.item;
        culturalNames = cul.item;
        lastNames = las.item;

    }

    
}
