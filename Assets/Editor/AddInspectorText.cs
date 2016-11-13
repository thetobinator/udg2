using UnityEngine;
using System.Collections;
using UnityEditor;

/* AlanMattano}}2014*/

[CustomEditor(typeof(InfoTextNote))]
public class AddInspectorText : Editor
{

    private int c = 0;
    private string buttonText = "Start typing";
    private int MaxSizeInt = 5;
    private int[] IntArray = new int[] { 0, 1, 2, 3, 4 };
    private string[] MaxSizeString = new string[] { "Line Label", "Box Text ", "Box Info", "Box Warning", "Box Error" };


    public override void OnInspectorGUI()
    {
        InfoTextNote inMyScript = (InfoTextNote)target;

        if (inMyScript.TextInfo == "Start writting text here... " +
           "/n Press Lock when finish.")
            ShowLogMessage();// se podria agregar alguna funcin en especial

        if (!inMyScript.isReady)
        {

            //DrawDefaultInspector();// Unity function

            switch (MaxSizeInt)
            {
                case 0:
                    if (EditorGUILayout.Toggle(inMyScript.isReady)) inMyScript.SwitchToggle();                                    // Toggle
                    EditorGUILayout.LabelField(inMyScript.TextInfo);        // A small line text
                    break;
                case 1:
                    if (GUILayout.Button(buttonText)) inMyScript.SwitchToggle();    //
                    EditorGUILayout.HelpBox(inMyScript.TextInfo, MessageType.None);  // This is a small box
                    break;
                case 2:
                    if (GUILayout.Button(buttonText)) inMyScript.SwitchToggle();    //
                    EditorGUILayout.HelpBox(inMyScript.TextInfo, MessageType.Info);  // This is a help box
                    break;
                case 3:
                    if (GUILayout.Button(buttonText)) inMyScript.SwitchToggle();
                    EditorGUILayout.HelpBox(inMyScript.TextInfo, MessageType.Warning);// This is a Warning box
                    break;
                case 4:
                    if (GUILayout.Button(buttonText)) inMyScript.SwitchToggle();     //
                    EditorGUILayout.HelpBox(inMyScript.TextInfo, MessageType.Error);  // This is a Error box
                    break;
                default:
                    if (GUILayout.Button(buttonText)) inMyScript.SwitchToggle();            // Button
                    EditorGUILayout.HelpBox(inMyScript.TextInfo, MessageType.Info);            // This is a help box
                    break;
            }
        }
        else
        {

            //DrawDefaultInspector();// for debug

            buttonText = "Lock !";
            // Button
            if (GUILayout.Button(buttonText)) inMyScript.SwitchToggle();


            // Text
            inMyScript.TextInfo = EditorGUILayout.TextArea(inMyScript.TextInfo);


            // selection
            MaxSizeInt = EditorGUILayout.IntPopup("Text Type :", MaxSizeInt, MaxSizeString, IntArray);

            // warning
            EditorGUILayout.HelpBox(" Press LOCK at the top when finish. ", MessageType.Warning); // A Warning box
        }
    }

    void ShowLogMessage()
    {
        c++;
        if (c == 1)
        {

            Debug.Log(" Need to add text " + "\n");
        }
    }
}