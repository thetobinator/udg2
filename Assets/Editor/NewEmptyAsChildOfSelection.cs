using UnityEngine;
using UnityEditor;
// from here http://forum.unity3d.com/threads/how-to-create-new-gameobject-as-child-in-hierarchy.77444/
public class NewEmptyAsChildOfSelection : MonoBehaviour
{

    /*   [MenuItem("GameObject/Create Empty Child #&n")]
static void CreateEmptyChildObject()
{
   GameObject go = new GameObject("GameObject");
   if (Selection.activeTransform != null)
       go.transform.parent = Selection.activeTransform;
}
}*/
    [MenuItem("GameObject/Create Empty Child #&c")]
    // empty child object of the selected object
    static void createEmptyChild()
    {
        GameObject go = new GameObject("GameObject");
        if (Selection.activeTransform != null)
        {
            go.transform.parent = Selection.activeTransform;
            go.transform.Translate(Selection.activeTransform.position);
        }
    }
    [MenuItem("GameObject/Create Empty Parent #&e")]
    // parents the selection to an empty gameobject
    static void createEmptyParent()
    {
        GameObject go = new GameObject("GameObject");
        if (Selection.activeTransform != null)
        {
            go.transform.parent = Selection.activeTransform.parent;
            go.transform.Translate(Selection.activeTransform.position);
            Selection.activeTransform.parent = go.transform;
        }
    }
    // empty object within the same hierarchy of objects as the selection
    [MenuItem("GameObject/Create Empty Sibling #&d")]
    static void createEmptySibling()
    {
        GameObject go = new GameObject("GameObject");
        if (Selection.activeTransform != null)
        {
            go.transform.parent = Selection.activeTransform.parent;
            go.transform.Translate(Selection.activeTransform.position);
        }
    }

}