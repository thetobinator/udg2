using UnityEditor;
using UnityEngine;
using System.Collections;
 
 
[ExecuteInEditMode]
public class EditorMoveDupeWithArrowKey : MonoBehaviour
{
    //References
 
    //Public
    public float AmountToMove = 1f;
    public static Transform staticTransform;
    public static float staticAmountToMove;
 
    //Private
    void Awake()
    {
		
     string info="EditorMoveDupeWithArrowKey use Right Control and Arrow Keys to nudge Items in editor \n" +
   "Right Shift + Right Control + Arrow will duplicate the object.\n" +
   "Update is checking for a box collider to get size";
        Debug.Log(info);
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
 
        staticAmountToMove = AmountToMove;
    }
 
    private static void Dupe()
    {
        SceneView.lastActiveSceneView.Focus();
        EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));
        staticTransform = Selection.activeGameObject.transform;
    }
    public void GetSize()
    {
		// lets try using this! GameTile.GetComponent<MeshFilter>().mesh.bounds? 
        Vector3 objectSize;
        SceneView.lastActiveSceneView.Focus();
        staticTransform = Selection.activeGameObject.transform;
        BoxCollider selColl = staticTransform.GetComponent<BoxCollider>(); ;
        if (selColl)
        {
            objectSize = selColl.size;
            AmountToMove = objectSize.x;
        }
    }
    // checking to resize the amountToMove based on boxcollidersize.x
    void Update() { GetSize(); }
 
    [MenuItem("Edit/Move/Right % RIGHT")]
    private static void MoveRight()
    {
        staticTransform.Translate(Vector3.right * staticAmountToMove);
    }
 
 
    [MenuItem("Edit/Move/Right % LEFT")]
    static void MoveLeft()
    {
        staticTransform.Translate(Vector3.left * staticAmountToMove);
    }
 
    [MenuItem("Edit/Move/Right % UP")]
    static void MoveUp()
    {
        staticTransform.Translate(Vector3.forward * staticAmountToMove);
    }
 
    [MenuItem("Edit/Move/Right % DOWN")]
    static void MoveDown()
    {
        staticTransform.Translate(Vector3.back * staticAmountToMove);
    }
 
    [MenuItem("Edit/Dupe/Right #% RIGHT")]
    static void DupeRight()
    {
        Dupe();
        staticTransform.Translate(Vector3.right * staticAmountToMove);
    }
 
    //shift key should duplicate
    [MenuItem("Edit/Dupe/Right #% LEFT")]
    static void DupeLeft()
    {
        Dupe();
        staticTransform.Translate(Vector3.left * staticAmountToMove);
    }
 
    [MenuItem("Edit/Dupe/Right #% UP")]
    static void DupeUp()
    {
        Dupe();
        staticTransform.Translate(Vector3.forward * staticAmountToMove);
    }
 
    [MenuItem("Edit/Dupe/Right #% DOWN")]
    static void DupeDown()
    {
        Dupe();
        staticTransform.Translate(Vector3.back * staticAmountToMove);
    }
#endif
}