using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// Make sure to rename the file name to TransformLooper.cs
[ExecuteInEditMode]
public class NewPrefabFromMyChildren : MonoBehaviour
{
 
    public bool doCopy = false;
    public bool hasCopied = false;
    public bool maintainTransformation = false;
    
    public GameObject sourceObject;
    public GameObject destinationObject;
    public Vector3 offset = new Vector3(1, 0, 0);
    private Vector3 lastPos = new Vector3(0, 0, 0);
    public List<GameObject> sourceChildren;
    private List<BoxCollider> sourceBoxes;
   // public Vector3 sourcePos;
    //public Vector3 sourceRot;
    //public Vector3 sourceScale;
    public List<GameObject> destChildren;
    private List<BoxCollider> destBoxes;
      [ExecuteInEditMode]
    void Awake()
    {
     
     
       
    }
    void CopySourceObject(){

        sourceChildren.Clear();
        //initialise lists
        sourceChildren = new List<GameObject>(new GameObject[sourceObject.transform.childCount]);
        sourceBoxes = new List<BoxCollider>(new BoxCollider[sourceObject.transform.childCount]);//room for 1 boxes?
        int i = 0;
        int ii = 0;
        foreach (Transform child in sourceObject.GetComponent<Transform>())
            {
                sourceChildren[i] = (GameObject) child.gameObject;
                sourceBoxes[i] = (BoxCollider)sourceChildren[i].GetComponent<BoxCollider>() as BoxCollider;
                i++;
           
            }
         }
       
     void DestinationMatchTransform(){
        destChildren.Clear();  
        destChildren = new List<GameObject>(new GameObject[destinationObject.transform.childCount]);
        
         int i = 0;
        foreach (Transform child in destinationObject.GetComponent<Transform>())
        {
            Transform sourceChild = sourceChildren[i].GetComponent<Transform>() as Transform;
            destChildren[i] = (GameObject)child.gameObject;
            destChildren[i].gameObject.GetComponent<Transform>().position = sourceChild.gameObject.GetComponent<Transform>().position;
            destChildren[i].transform.rotation = sourceChild.gameObject.GetComponent<Transform>().rotation;
            destChildren[i].transform.localScale = sourceChild.gameObject.GetComponent<Transform>().localScale;
            
            
                //resie
                BoxCollider sourceBox = sourceChild.GetComponent<BoxCollider>() as BoxCollider;
                Vector3 sourceTrans = sourceBox.center;
                Vector3 sourceSize = sourceBox.size;
                BoxCollider destBox = destChildren[i].GetComponent<BoxCollider>() as BoxCollider;
                destBox.center = new Vector3(sourceTrans.x,sourceTrans.y,sourceTrans.z);
                destBox.size = new Vector3(sourceSize.x,sourceSize.y,sourceSize.z);
                    i++;
            }
        hasCopied = true;
        doCopy = false; 
        }
         
     
    

    void MaintainCopiedTransform()
    {
        int ii = 0;
        foreach (Transform child in destinationObject.GetComponent<Transform>())
        {
            Transform sourceChild = sourceChildren[ii].GetComponent<Transform>() as Transform;
            destChildren[ii] = (GameObject)child.gameObject;
            destChildren[ii].transform.position = sourceChild.transform.position;
            destChildren[ii].transform.rotation = sourceChild.transform.rotation;
            destChildren[ii].transform.localScale = sourceChild.transform.localScale;
            BoxCollider sourceBox = sourceChildren[ii].GetComponent<BoxCollider>();
            BoxCollider destBox = destChildren[ii].GetComponent<BoxCollider>();
            destBox.transform.position = sourceBox.transform.position;
            destBox.transform.rotation = sourceBox.transform.rotation;
            destBox.transform.localScale = sourceBox.transform.localScale;
            ii++;

        }
    }
      void Start()
    {
#if UNITY_EDITOR
      //  initCopy();
#endif
        //_lerper = new GameObject().transform;
        //_lerper.name = _lerperName;
       // _lerper.transform.SetParent(transform);
       // sourceChildren = sourceObject.GetComponents<TransformChildren>();
       //   IC = GameObject.Instantiate(_prefab, new Vector3(0, 0, 0), Quaternion.identity) as Transform;
       //  foreach (Transform child in IC){
       //      print(child.name);
       //  }
           // instance.transform.SetParent(_lerper.transform);
           // instance.transform.position = i * Direction * Distance;
    }
      void Update()
     {
  #if UNITY_EDITOR

         if (doCopy == true)
         {
              CopySourceObject();
             DestinationMatchTransform();
             
          }
      
#endif
         if (maintainTransformation == true)
         {
             MaintainCopiedTransform();
         }
     }

}


