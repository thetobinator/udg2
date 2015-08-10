using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
#region InspectorDrawers
public enum TransformsToPass { Position, Rotation, Size }
//Custom Serializable Class
// creates Drawer in Inpsector
[System.Serializable]
public class Loc
{ 
    public bool x = true;
    public bool y = true;
    public bool z = true;
}
// creates Drawer in Inpsector
[CustomPropertyDrawer(typeof(Loc))]
public class LocDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;


            // Calculate rects
            
            var amountRect = new Rect(position.x, position.y, 20, position.height);
            var unitRect = new Rect(position.x + 32, position.y, 40, position.height);
            var nameRect = new Rect(position.x + 64, position.y, 80, position.height);
            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("x"), GUIContent.none);
          
           
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("y"), GUIContent.none);
           
    
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("z"), GUIContent.none);
           // GUILayout.EndArea();*/
             
            
           

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
}

[System.Serializable]
public class Rot
{
    public bool x = true;
    public bool y = true;
    public bool z = true;
}
// creates Drawer in Inpsector
[CustomPropertyDrawer(typeof(Rot))]
public class RotDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects

        var amountRect = new Rect(position.x, position.y, 20, position.height);
        var unitRect = new Rect(position.x + 32, position.y, 40, position.height);
        var nameRect = new Rect(position.x + 64, position.y, 80, position.height);
        // Draw fields - passs GUIContent.none to each so they are drawn without labels

        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("x"), GUIContent.none);
        EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("y"), GUIContent.none);
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("z"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
[System.Serializable]
public class Siz
{
    public bool x = true;
    public bool y = true;
    public bool z = true;
}
// creates Drawer in Inpsector
[CustomPropertyDrawer(typeof(Siz))]
public class SizDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects

        var amountRect = new Rect(position.x, position.y, 20, position.height);
        var unitRect = new Rect(position.x + 32, position.y, 40, position.height);
        var nameRect = new Rect(position.x + 64, position.y, 80, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels

        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("x"), GUIContent.none);
        EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("y"), GUIContent.none);
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("z"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
#endregion //inspectordrawers

[ExecuteInEditMode]
public class CopyChildTransforms : MonoBehaviour
{
    [Header("Copy:\t              X      Y      Z")]
    public Loc location;
    public Rot rotate;
    public Siz resize;
    public bool maintainTransformation = false;

    public GameObject sourceObject;
    public GameObject destinationObject;
    private Transform destinationOrigin;
    public Vector3 offset = new Vector3(1, 0, 0);
    private Vector3 lastPos = new Vector3(0, 0, 0);
    public bool doCopy = false;
    public bool hasCopied = false;

    public List<GameObject> sourceChildren;
    private List<BoxCollider> sourceBoxes;
    // public Vector3 sourcePos;
    //public Vector3 sourceRot;
    //public Vector3 sourceScale;
    public List<GameObject> destChildren;
    private List<BoxCollider> destBoxes;
 
   //This makes an Inspector Drop Down Arrow Group 
   /* [System.Serializable]
    public class Position
    {
        public bool X, Y, Z = true;
    }
    public Position position;

    [System.Serializable]
    public class Rotation
    {
        public bool X, Y, Z = true;
    }
    public Rotation rotation;

    [System.Serializable]
    public class Size
    {
        public bool X, Y, Z = true;
    }
    public Size size;
    */
  
   
    [ExecuteInEditMode]
    void CopySourceObject()
    {
        sourceChildren.Clear();
        sourceChildren = new List<GameObject>(new GameObject[sourceObject.transform.childCount]);
        sourceBoxes = new List<BoxCollider>(new BoxCollider[sourceObject.transform.childCount]);//room for 1 boxes?
        int i = 0;
        int ii = 0;
        foreach (Transform child in sourceObject.GetComponent<Transform>())
        {
            sourceChildren[i] = (GameObject)child.gameObject;
            sourceBoxes[i] = (BoxCollider)sourceChildren[i].GetComponent<BoxCollider>() as BoxCollider;
            i++;
        }
    }

    void DestinationMatchTransform()
    {
        destChildren.Clear();
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
            #region location
            Vector3 destLoc = destChild.position;
            Vector3 sourceLoc = sourceChild.gameObject.GetComponent<Transform>().position;
            if (location.x == true && location.y == true & location.z == true)
            {
             destChildren[i].transform.position = sourceChild.gameObject.GetComponent<Transform>().position;
            }
            else
            { 

            if (location.x == true) {
                destLoc.x = sourceLoc.x;
            }
            if (location.y == true)
            {
                destLoc.y = sourceLoc.y;
            }
            if (location.z == true)
            {
                destLoc.z = sourceLoc.z;
            }
            destChild.position = new Vector3(destLoc.x, destLoc.y, destLoc.z);
            }
            #endregion //location

            #region rotation
            Vector3 destRot = destChild.localEulerAngles;
            Vector3 sourceRot = sourceChild.gameObject.GetComponent<Transform>().localEulerAngles;

            if (rotate.x == true && rotate.y == true & rotate.z == true)
            {
                destChildren[i].transform.rotation = sourceChild.gameObject.GetComponent<Transform>().rotation;
            }
            else
            {
                if (rotate.x == true)
                {
                    destRot.x = sourceRot.x;
                }
                if (rotate.y == true)
                {
                    destRot.y = sourceRot.y;
                }
                if (rotate.z == true)
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
            if (resize.x == true && resize.y == true & resize.z == true)
            {
                destChildren[i].transform.localScale = sourceChild.gameObject.GetComponent<Transform>().localScale;
            }
            else
            {
                if (resize.x == true)
                {
                    destSize.x = sourceSize.x;
                }
                if (resize.y == true)
                {
                    destSize.y = sourceSize.y;
                }
                if (resize.z == true)
                {
                    destSize.z = sourceSize.z;
                }
                destChild.localScale = new Vector3(destSize.x, destSize.y, destSize.z);
                // destChildren[i].transform.localScale = sourceChild.gameObject.GetComponent<Transform>().localScale;
            }
            #endregion //resize
           
            //copy collision box transforms
            BoxCollider sourceBox = sourceChild.GetComponent<BoxCollider>() as BoxCollider;
            Vector3 sourceTrans = sourceBox.center;
            Vector3 sourceBSize = sourceBox.size;
            BoxCollider destBox = destChildren[i].GetComponent<BoxCollider>() as BoxCollider;
            destBox.center = new Vector3(sourceTrans.x, sourceTrans.y, sourceTrans.z);
            destBox.size = new Vector3(sourceBSize.x, sourceBSize.y, sourceBSize.z);
            i++;
        }
        hasCopied = true;
        doCopy = false;
    }



    // continue maintaining transform
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


