using UnityEditor;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EditorMoveWithArrowKey : MonoBehaviour
{
    //References

    //Public
    public float AmountToMove = 1f;
    public static Transform staticTransform;
    public static float staticAmountToMove;

    //Private
    void Awake()
    {
        Debug.Log("EditorMoveWithArrowKey use Right Control and Arrow Keys to nudge Items in editor");
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
        staticAmountToMove = AmountToMove;
    }

    [MenuItem("Edit/Move/Right %RIGHT")]
    private static void MoveRight()
    {
        staticTransform.Translate(Vector3.right * staticAmountToMove);
    }


    [MenuItem("Edit/Move/Right %LEFT")]
    static void MoveLeft()
    {
        staticTransform.Translate(Vector3.left * staticAmountToMove);
    }

    [MenuItem("Edit/Move/Right %UP")]
    static void MoveUp()
    {
        staticTransform.Translate(Vector3.up * staticAmountToMove);
    }

    [MenuItem("Edit/Move/Right %DOWN")]
    static void MoveDown()
    {
        staticTransform.Translate(Vector3.down * staticAmountToMove);
    }
#endif
}