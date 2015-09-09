using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace Utilties
{


[ExecuteInEditMode]
    public class CopyChildTransforms : EditorWindow
{
    public GameObject sourceObject;
    public GameObject destinationObject;
   
    public bool relocatex = true;
    public bool relocatey = true;
    public bool relocatez = true;
    public bool rotatex = true;
    public bool rotatey = true;
    public bool rotatez = true;
    public bool resizex = true;
    public bool resizey = true;
    public bool resizez = true;

    public bool viewCopyMatrix = true;

   // public bool maintainTransformation = false;
    public bool doCopy = false;
    public string sourceObjectName;
    public string selected = "";
    private bool aboutState = true;
    private Transform destinationOrigin;
    //public Vector3 offset = new Vector3(1, 0, 0);
    //private Vector3 lastPos = new Vector3(0, 0, 0);


    public List<GameObject> sourceChildren;
    private List<BoxCollider> sourceBoxes;
    // public Vector3 sourcePos;
    //public Vector3 sourceRot;
    //public Vector3 sourceScale;
    public List<GameObject> destChildren;
    private List<BoxCollider> destBoxes;
  
    //because serializable is not working for editor script

    [ExecuteInEditMode]
    void CopySourceObject()
    {
      //  sourceChildren.Clear();
        sourceChildren = new List<GameObject>(new GameObject[sourceObject.transform.childCount]);
        sourceBoxes = new List<BoxCollider>(new BoxCollider[sourceObject.transform.childCount]);//room for 1 boxes?
        int i = 0;

        foreach (Transform child in sourceObject.GetComponent<Transform>())
        {
            sourceChildren[i] = (GameObject)child.gameObject;
            if ((BoxCollider)sourceChildren[i].GetComponent<BoxCollider>() as BoxCollider)
            sourceBoxes[i] = (BoxCollider)sourceChildren[i].GetComponent<BoxCollider>() as BoxCollider;
            i++;
        }
    }

    void DestinationMatchTransform()
    {
        //destChildren.Clear();
        destChildren = new List<GameObject>(new GameObject[destinationObject.transform.childCount]);
        destinationOrigin = destinationObject.GetComponent<Transform>() as Transform;
        int i = 0;
        destinationObject.GetComponent<Transform>().position = sourceObject.GetComponent<Transform>().position;
        destinationObject.GetComponent<Transform>().rotation = sourceObject.GetComponent<Transform>().rotation;
        destinationObject.GetComponent<Transform>().localScale = sourceObject.GetComponent<Transform>().localScale;
        foreach (Transform child in destinationObject.GetComponent<Transform>())
        {
            destChildren[i] = (GameObject)child.gameObject;
            Transform sourceChild = sourceChildren[i].GetComponent<Transform>() as Transform;
            Transform destChild = destChildren[i].gameObject.GetComponent<Transform>() as Transform;
            #region relocate
            Vector3 destLoc = destChild.position;
            Vector3 sourceLoc = sourceChild.gameObject.GetComponent<Transform>().position;
            if (relocatex == true && relocatey == true & relocatez == true)
            {
                destChildren[i].transform.position = sourceChild.gameObject.GetComponent<Transform>().position;
            }
            else
            {

                if (relocatex == true)
                {
                    destLoc.x = sourceLoc.x;
                }
                if (relocatey == true)
                {
                    destLoc.y = sourceLoc.y;
                }
                if (relocatez == true)
                {
                    destLoc.z = sourceLoc.z;
                }
                destChild.position = new Vector3(destLoc.x, destLoc.y, destLoc.z);
            }
            #endregion //relocate

            #region rotation
            Vector3 destRot = destChild.localEulerAngles;
            Vector3 sourceRot = sourceChild.gameObject.GetComponent<Transform>().localEulerAngles;

            if (rotatex == true && rotatey == true & rotatez == true)
            {
                destChildren[i].transform.rotation = sourceChild.gameObject.GetComponent<Transform>().rotation;
            }
            else
            {
                if (rotatex == true)
                {
                    destRot.x = sourceRot.x;
                }
                if (rotatey == true)
                {
                    destRot.y = sourceRot.y;
                }
                if (rotatez == true)
                {
                    destRot.z = sourceRot.z;
                }
                destChild.rotation = Quaternion.Euler(destRot.x, destRot.y, destRot.z);
                //destChildren[i].transform.rotation = sourceChild.gameObject.GetComponent<Transform>().rotation;
            }
            #endregion //rotation

            #region resize
            Vector3 destSize = destChild.localScale;
            Vector3 sourceSize = sourceChild.gameObject.GetComponent<Transform>().localScale;
            if (resizex == true && resizey == true & resizez == true)
            {
                destChildren[i].transform.localScale = sourceChild.gameObject.GetComponent<Transform>().localScale;
            }
            else
            {
                if (resizex == true)
                {
                    destSize.x = sourceSize.x;
                }
                if (resizey == true)
                {
                    destSize.y = sourceSize.y;
                }
                if (resizez == true)
                {
                    destSize.z = sourceSize.z;
                }
                destChild.localScale = new Vector3(destSize.x, destSize.y, destSize.z);
                // destChildren[i].transform.localScale = sourceChild.gameObject.GetComponent<Transform>().localScale;
            }
            #endregion //resize

            //copy collision box transforms
            if (sourceChild.GetComponent<BoxCollider>() != null)
            {
                BoxCollider sourceBox = sourceChild.GetComponent<BoxCollider>() as BoxCollider;
           

            Vector3 sourceTrans = sourceBox.center;
            Vector3 sourceBSize = sourceBox.size;
            
            if (destChildren[i].GetComponent<BoxCollider>() != null){
            BoxCollider destBox = destChildren[i].GetComponent<BoxCollider>() as BoxCollider;
            destBox.center = new Vector3(sourceTrans.x, sourceTrans.y, sourceTrans.z);
            destBox.size = new Vector3(sourceBSize.x, sourceBSize.y, sourceBSize.z);
            }
            }
            i++;
        }
        //hasCopied = true;
        doCopy = false;
    }



    void OnGUI()
    {
        // set up 
        int cCol = 80;
        int cWid = 30;
        int cLine = 20;
        int cHeight = 20;
       
       GUILayout.Label("Copy Child Transforms", EditorStyles.boldLabel);

       aboutState = GUI.Toggle(new Rect(cCol + 150, 4, 100, cHeight), aboutState, "About", EditorStyles.foldout);
        // cLine = cLine +45;
        if (aboutState)
        {
            GUILayout.BeginArea(new Rect(1, cLine, 280, 250));
            EditorGUILayout.HelpBox("Copy transforms of source object children to children of destination object. For Converting generic Unity Cube built objects to Destructible Master Cube Objects.", MessageType.None);
            GUILayout.EndArea();
            cLine = cLine +65;

        }
        else
        {
            cLine = cLine + 2;
            //EditorGUILayout.HelpBox("", MessageType.None);
        }

       
        // Toggle Trasform Matrix
        //GUILayout.Label("Copy:\tX      Y      Z", EditorStyles.foldout);
       
        viewCopyMatrix = GUI.Toggle(new Rect(4, cLine, 100, cHeight), viewCopyMatrix, "Copy:\tX      Y      Z", EditorStyles.foldout);
        if (viewCopyMatrix)
        {
            //GUILayout uses auto loctions (new line for each property)
            //GUI uses defined relocates
            cLine = cLine + cHeight;
            relocatex = GUI.Toggle(new Rect(cCol, cLine, cWid, cHeight), relocatex, "");
            relocatey = GUI.Toggle(new Rect(cCol + cWid, cLine, cWid, cHeight), relocatey, "");
            relocatez = GUI.Toggle(new Rect(cCol + (cWid * 2), cLine, 100, cHeight), relocatez, " Position");
            cLine = cLine + cHeight;
            rotatex = GUI.Toggle(new Rect(cCol, cLine, cWid, cHeight), rotatex, "");
            rotatey = GUI.Toggle(new Rect(cCol + cWid, cLine, cWid, cHeight), rotatey, "");
            rotatez = GUI.Toggle(new Rect(cCol + (cWid * 2), cLine, 100, 30), rotatez, " Rotation");
            cLine = cLine + cHeight;
            resizex = GUI.Toggle(new Rect(cCol, cLine, cWid, cHeight), resizex, "");
            resizey = GUI.Toggle(new Rect(cCol + cWid, cLine, cWid, cHeight), resizey, "");
            resizez = GUI.Toggle(new Rect(cCol + (cWid * 2), cLine, 100, 30), resizez, " Size");
            // end GUI.TOGGLE Matrix
            cLine = cLine + cHeight;
        }
        else
        {
            cLine = cLine + cHeight;
        }
        GUILayout.BeginArea(new Rect(10, cLine, 200, 250));

        GUILayout.Label("Source Parent Object:", EditorStyles.miniBoldLabel);
        sourceObject = (GameObject)EditorGUILayout.ObjectField(sourceObject, typeof(GameObject), true);
        GUILayout.Label("Destination Parent Object:", EditorStyles.miniBoldLabel);
        destinationObject = (GameObject)EditorGUILayout.ObjectField(destinationObject, typeof(GameObject), true);
       //GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        GUILayout.Space(10.0f);
        string srcname = "";
        if (sourceObject != null)
            srcname = sourceObject.name;

        string btnText = "Copy " + srcname + " Child Transforms";
        string dstname = "";
        if (destinationObject != null)
        {
            dstname = destinationObject.name;
            btnText = btnText + "\nTo " + dstname + " Children";          
        }

        if (srcname != "" && dstname != "")
        {
            if (GUILayout.Button(btnText))
            {
                CopySourceObject();
                DestinationMatchTransform();
            }
        }
        else
        {
            GUILayout.Box("choose parent objects", GUILayout.ExpandWidth(true), GUILayout.Height(22));
        }
        GUILayout.EndArea();
    }


    void OnInspectorUpdate()
    {
        Repaint();
    }
    [MenuItem("Utilities/ Copy Child Transforms")]
    static void TransformCopyChildrenWindow()
    {
        //Shows Utility Window
        //CopyChildTransforms window = new CopyChildTransforms();
       // window.ShowUtility();

        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(CopyChildTransforms));
    }
}

} //end namesapce utilities;

