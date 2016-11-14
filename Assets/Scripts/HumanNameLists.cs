using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class HumanNameLists : MonoBehaviour
{
    public List<string> femaleNames;
    public List<string> maleNames;
    public List<string> ghettoNames;
    public List<string> lastNames;

    // Use this for initialization
    public void Transfer () {

      List_TextMesh fem = GameObject.Find("femaleNames").GetComponent<List_TextMesh>();
        List_TextMesh man = GameObject.Find("maleNames").GetComponent<List_TextMesh>();
        List_TextMesh ghe = GameObject.Find("ghettoNames").GetComponent<List_TextMesh>();
        List_TextMesh las = GameObject.Find("lastNames").GetComponent<List_TextMesh>();
        femaleNames = fem.item;
        maleNames = man.item;
        ghettoNames = ghe.item;
        lastNames = las.item;

    }

    
}
