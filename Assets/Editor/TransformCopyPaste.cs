using UnityEditor;
using UnityEngine;

public static class TransformCopyPaste
{
    /*found here: http://feedback.unity3d.com/suggestions/copy-and-paste-transforms-rotation-position-scale-or-everything-at-once
     KRIZKAMAR 26, 2014 13:41 Also you can make custom editor for Transform component with handy copy-paste and reset buttons.
     hmm what? That's an interesting idea, component values as a component.
        For instance everything is at zero by default, and the component puts them in place at game time.
      */
    private static Vector3 position;
    private static Quaternion rotation;
    private static Vector3 localScale;

    [UnityEditor.MenuItem("CONTEXT/Transform/Copy Position")]
    public static void CopyPosition(UnityEditor.MenuCommand command)
    {
        position = (command.context as Transform).position;
    }

    [UnityEditor.MenuItem("CONTEXT/Transform/Paste Position")]
    public static void PastePosition(UnityEditor.MenuCommand command)
    {
        Transform transform = (command.context as Transform);
        Undo.RecordObject(transform, "Paste position");
        (command.context as Transform).position = position;
    }

    [UnityEditor.MenuItem("CONTEXT/Transform/Copy Rotation")]
    public static void CopyRotation(UnityEditor.MenuCommand command)
    {
        rotation = (command.context as Transform).rotation;
    }

    [UnityEditor.MenuItem("CONTEXT/Transform/Paste Rotation")]
    public static void PasteRotation(UnityEditor.MenuCommand command)
    {
        Transform transform = (command.context as Transform);
        Undo.RecordObject(transform, "Paste rotation");
        transform.rotation = rotation;
    }

    [UnityEditor.MenuItem("CONTEXT/Transform/Copy Scale")]
    public static void CopyScale(UnityEditor.MenuCommand command)
    {
        localScale = (command.context as Transform).localScale;
    }

    [UnityEditor.MenuItem("CONTEXT/Transform/Paste Scale")]
    public static void PasteScale(UnityEditor.MenuCommand command)
    {
        Transform transform = (command.context as Transform);
        Undo.RecordObject(transform, "Paste scale");
        transform.localScale = localScale;
    }

}

