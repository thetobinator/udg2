using UnityEngine;
using System.Collections;

public class RandomizeCategories : MonoBehaviour {
    public float textSpeed = 0.5f;
    private float oldSpeed = 0.5f;
    // Use this for initialization

        public void Init()
    {
        oldSpeed = textSpeed;
        GameObject[] Categories = GameObject.FindGameObjectsWithTag("Category");
        foreach (GameObject obj in Categories)
        {
            if (!obj.GetComponent<RandomTextMovement>())
            {
                obj.AddComponent<RandomTextMovement>();           
            }
            if (!obj.GetComponent<List_TextMesh>())
            {
                obj.AddComponent<List_TextMesh>();
            }
            obj.GetComponent<List_TextMesh>().textTimer = textSpeed;
        }
    }

    
    void Start() {
        Init();
    }

   public void Update()
    {
        if (oldSpeed != textSpeed)
        {
            GameObject[] Categories = GameObject.FindGameObjectsWithTag("Category");
            foreach (GameObject obj in Categories)
            {             
                obj.GetComponent<List_TextMesh>().textTimer = textSpeed;
            }       
            oldSpeed = textSpeed;
        }
    }	
}
