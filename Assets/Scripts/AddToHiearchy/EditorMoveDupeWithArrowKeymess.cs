using UnityEditor;
using UnityEngine;
using System.Collections;
 
/* namespace broken
 { 
[ExecuteInEditMode]
public  class EditorMoveDupeWithArrowKey : MonoBehaviour
{
    //References
 
    //Public
    public  bool useBoxCollider = false;
    public   float AmountToMove = 1.0f;
    public  static Transform staticTransform;
    public static float staticAmountToMove;
    private static BoxCollider selectedColl;
    private  static EditorMoveDupeWithArrowKey thisscript;
    //Private
    void Awake()
    {
     string info="EditorMoveDupeWithArrowKey use Right Control and Arrow Keys to nudge Items in editor \n" +
   "Right Shift + Right Control + Arrow will duplicate the object.\n" +
   "Update is checking for a box collider to get size";
        Debug.Log(info);
         thisscript = this.GetComponent<EditorMoveDupeWithArrowKey>();
    }
 
#if UNITY_EDITOR
    void OnRenderObject()
    {
        if (!Application.isPlaying && Selection.activeGameObject != null)
        {
            staticTransform = Selection.activeGameObject.transform;
     
        }
        else
        {
            staticTransform = null;
        }
        //    if (Input.GetKeyDown(KeyCode.RightShift))

        staticAmountToMove = this.GetComponent<EditorMoveDupeWithArrowKey>().AmountToMove;
    }
 
 

    public static  void GetSize()
    {
        Vector3 objectSize;
        SceneView.lastActiveSceneView.Focus();
        staticTransform = Selection.activeGameObject.transform;
       
        BoxCollider selectedColl = staticTransform.GetComponent<BoxCollider>(); ;
        
        if (selectedColl)
        {

            objectSize = selectedColl.size;
            if (this.GetComponent<EditorMoveDupeWithArrowKey>().useBoxCollider == true)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    this.GetComponent<EditorMoveDupeWithArrowKey>().AmountToMove = objectSize.x;
                }
                else
                {
                    this.GetComponent<EditorMoveDupeWithArrowKey>().AmountToMove = objectSize.z; 
                }
            }
           
        }
    }

    public static   void Dupe()
    {
       GetSize();
        // SceneView.lastActiveSceneView.Focus();
        EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));
        staticTransform = Selection.activeGameObject.transform;
    }

    // checking to resize the amountToMove based on boxcollidersize.x
    void Update() { }
 
    [MenuItem("Edit/Move/Right % RIGHT")]
    private static  void MoveRight()
    {
        thisscript.GetComponent<EditorMoveDupeWithArrowKey>().GetSize();
        staticTransform.Translate(Vector3.right * staticAmountToMove);
    }
 
 
    [MenuItem("Edit/Move/Right % LEFT")]
    private static void MoveLeft()
    {
        thisscript.GetComponent<EditorMoveDupeWithArrowKey>().GetSize();
        staticTransform.Translate(Vector3.left * staticAmountToMove);
    }
 
    [MenuItem("Edit/Move/Right % UP")]
    private static void MoveUp()
    {
        thisscript.GetComponent<EditorMoveDupeWithArrowKey>().GetSize();
        staticTransform.Translate(Vector3.forward * staticAmountToMove);
    }
 
    [MenuItem("Edit/Move/Right % DOWN")]
    private static void MoveDown()
    {
        thisscript.GetSize();
        staticTransform.Translate(Vector3.back * staticAmountToMove);
    }
 
    [MenuItem("Edit/Dupe/Right #% RIGHT")]
    private static void DupeRight()
    {
        thisscript.GetComponent<EditorMoveDupeWithArrowKey>().Dupe();
        staticTransform.Translate(Vector3.right * staticAmountToMove);
    }
 
    //shift key should duplicate
    [MenuItem("Edit/Dupe/Right #% LEFT")]
    private static void DupeLeft()
    {
        thisscript.GetComponent<EditorMoveDupeWithArrowKey>().Dupe();
        staticTransform.Translate(Vector3.left * staticAmountToMove);
    }
 
    [MenuItem("Edit/Dupe/Right #% UP")]
    private static void DupeUp()
    {
        thisscript.GetComponent<EditorMoveDupeWithArrowKey>().Dupe();
        staticTransform.Translate(Vector3.forward * staticAmountToMove);
    }
 
    [MenuItem("Edit/Dupe/Right #% DOWN")]
    private static void DupeDown()
    {
        thisscript.GetComponent<EditorMoveDupeWithArrowKey>().Dupe();
        staticTransform.Translate(Vector3.back * staticAmountToMove);
    }
#endif
}
}
*/