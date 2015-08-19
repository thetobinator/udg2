using UnityEditor;
using UnityEngine;
using System.Collections;

// Floating window utility that lets you randomize the rotation of the selected objects.
// Simple script that randomizes the rotation of the Selected GameObjects
// and lets you see which objects are currently selected
public class RandomizeInSelection : EditorWindow
{
    public float rotationAmount = 0.33F;
    public string selected = "";
    void RandomizeSelected()
    {
        foreach (var transform in Selection.transforms)
        {
            Quaternion rotation = Random.rotation;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, rotation, rotationAmount);
        }
    }
    void OnGUI()
    {
        EditorGUILayout.HelpBox("Randomize the rotation of the selected objects.", MessageType.Info);
        foreach (var t in Selection.transforms)
        {
            selected += t.name + " ";
        }
        EditorGUILayout.LabelField("Selected Object:", selected);
        selected = "";
        if (GUILayout.Button("Randomize!"))
            RandomizeSelected();

        if (GUILayout.Button("Close"))
            Close();

       // Debug.Log(selected);
    }
    void OnInspectorUpdate()
    {
        Repaint();
    }
    [MenuItem("Example/Randomize Children In Selection")]
    static void RandomizeWindow()
    {
        RandomizeInSelection window = new RandomizeInSelection();
        window.ShowUtility();
    }
}