using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[ExecuteInEditMode]
public class TransformChildren : MonoBehaviour
{
    public bool doTransform = false;
    public bool prefabHasTransform = false;
    public Vector3 Direction = new Vector3(1, 0, 0);
    public Vector3 newPosition;
    public Vector3 sourcePosition;
    public Vector3 oldPosition;
 

#if UNITY_EDITOR
   
#endif
    void Awake()
    {
       sourcePosition =  gameObject.GetComponent<Transform>().position; 
    }
    void TransformEach(){
        doTransform = false;
            prefabHasTransform = true;
        oldPosition = sourcePosition;
     foreach (Transform child in transform)
            {
                     newPosition.x = oldPosition.x + Direction.x;
                     newPosition.y = oldPosition.y + Direction.y;
                     newPosition.z = oldPosition.z + Direction.z;
                    child.gameObject.transform.position = new Vector3( newPosition.x, newPosition.y, newPosition.z);
                    oldPosition = child.GetComponent<Transform>().position;
            }
    }

    public void Update()
    {
        if (doTransform == true)
        {

            TransformEach();    
        }
    }
}