using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Utilities
{
  
    public class MoveAndDuplicate : EditorWindow
    {
        //References
        //Public
        public static Vector3 AmountToMove = new Vector3(1, 1 ,1);
        public static Transform staticTransform;
        public static float staticAmountToMove;
        public string sourceObjectName;
        public GameObject sourceObject;
        public GameObject destinationObject;
        public bool useBoxCollider;
        public static bool captureTheKeys;
        public BoxCollider selectedBoxColl;
        public string selected = "";
        public Vector3 objectSize;
        public int mSelected = 0;
         public string[] menuoptions = new string[]
                {
                "Box Collider", "Object Scale", "Input", 
                };
        //Private
        private bool aboutState = true;
        void Awake()
        {
            Debug.Log("Alert!\nUtlilties/MoveAndDuplicate Active Control + Arrow and Control + Shift + Arrow move/duplicates selected objects!");
        }
     void OnGUI()
    {
        // set up 
        int cCol = 80;
        //int cWid = 30; checkbox width
        int cLine = 20;
        int cHeight = 20;        
       GUILayout.Label("Move And Duplicate", EditorStyles.boldLabel);
       aboutState = GUI.Toggle(new Rect(cCol + 150, 4, 100, cHeight), aboutState, "About", EditorStyles.foldout);
        // cLine = cLine +45;
        if (aboutState)
        {
            GUILayout.BeginArea(new Rect(4, cLine, 280, 150));
           EditorGUILayout.HelpBox("Move Selected Object  using Control Key + Arrow Key. Include Shift Key to duplicate and move.", MessageType.None);
            GUILayout.EndArea();
            cLine = cLine +(cHeight*2);
        }
    
        sourceObject = Selection.activeGameObject;
        if (Selection.activeGameObject != null)    
        {   
            staticTransform = Selection.activeGameObject.transform;
            if (staticTransform != null)
            {
                // Toggle Trasform Matrix
                //GUILayout.Label("Copy:\tX      Y      Z", EditorStyles.foldout);
                GUILayout.BeginArea(new Rect(10, cLine, 260, (cHeight * 8)));
                EditorGUILayout.LabelField("Selection:");
                sourceObject = Selection.activeGameObject.gameObject;
                sourceObject = (GameObject)EditorGUILayout.ObjectField(sourceObject, typeof(GameObject), true);
                //captureTheKeys = EditorGUILayout.Toggle("Capture Keys", captureTheKeys);
                
                mSelected = EditorGUILayout.Popup("Use:", mSelected, menuoptions);
              //Debug.Log(menuoptions[mSelected]);

                   // useBoxCollider = EditorGUILayout.Toggle("Use Box Collider Size", useBoxCollider);
                    EditorGUILayout.LabelField("Move:", EditorStyles.boldLabel);
                   // cLine = cLine + (cHeight * 4);
                    if (menuoptions[mSelected] == "Box Collider")
                    {
                        selectedBoxColl = staticTransform.GetComponent<BoxCollider>() ;
                        if (selectedBoxColl)
                        {
                            //objectSize = sourceObject.GetComponent<MeshFilter>().mesh.bounds;
                            objectSize = selectedBoxColl.size;
                            AmountToMove = objectSize;
                        }
                    }

                    if (menuoptions[mSelected] == "Object Scale")
                    {
                        
                            //objectSize = sourceObject.GetComponent<MeshFilter>().mesh.bounds;
                        objectSize = staticTransform.lossyScale;
                            AmountToMove = objectSize;
                       
                    }


                    AmountToMove.x = EditorGUILayout.FloatField("X", AmountToMove.x);
                    //AmountToMove.y = EditorGUILayout.FloatField("Y", AmountToMove.y);
                    AmountToMove.z = EditorGUILayout.FloatField("Z", AmountToMove.z);           
                GUILayout.EndArea();
            } // end if staticTransform not null
        } // end if selection not null
        else
        {
            cLine = cLine + (10);
            GUILayout.BeginArea(new Rect(10, cLine, 260, 250));
            sourceObject = (GameObject)EditorGUILayout.ObjectField(sourceObject, typeof(GameObject), true);
            if (sourceObject)
            Selection.activeGameObject = sourceObject.gameObject;
            //GUILayout.Box("Select Scene Object", GUILayout.ExpandWidth(true), GUILayout.Height(22));
             GUILayout.EndArea();
        }
    }// end OnGui;
        
    void OnInspectorUpdate()
    {
        Repaint();       
    }
    [MenuItem("Utilities/ Move And Duplicate Transforms")]
    static void MoveAndDuplicateWindow()
    {
        //Shows Utility Window
        //MoveAndDuplicate window = new MoveAndDuplicate();
       // window.ShowUtility();

        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(MoveAndDuplicate));
    }

   public static void MoveAndOrDupe(string command)
    {
        SceneView.lastActiveSceneView.Focus();
        if (Selection.activeObject != null)
        {
           
             staticTransform = Selection.activeGameObject.transform;
             switch (command)
             {
                 case "moveleft":
                     staticTransform.Translate(Vector3.left * AmountToMove.x);
                     break;
                 case "moveright":
                     staticTransform.Translate(Vector3.right * AmountToMove.x);
                     break;
                 case "moveup":
                     staticTransform.Translate(Vector3.forward * AmountToMove.z);
                     break;
                 case "movedown":
                     staticTransform.Translate(Vector3.back * AmountToMove.z);
                     break;


                 case "dupeleft":
                     EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));
                     staticTransform = Selection.activeGameObject.transform;
                     staticTransform.Translate(Vector3.left * AmountToMove.x);
                     break;
                 case "duperight":
                     EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));
                     staticTransform = Selection.activeGameObject.transform;
                     staticTransform.Translate(Vector3.right * AmountToMove.x);
                     break;
                 case "dupeup":
                     EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));
                     staticTransform = Selection.activeGameObject.transform;
                     staticTransform.Translate(Vector3.forward * AmountToMove.z);
                     break;
                 case "dupedown":
                     EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));
                     staticTransform = Selection.activeGameObject.transform;
                     staticTransform.Translate(Vector3.back * AmountToMove.z);
                     break;


                       
        

             }
        }      
    }

        #region ControlPlusArrowKeyMenuAction
    [MenuItem("Edit/Move/Right % RIGHT")]
    private static void MoveRight()
    {
        MoveAndOrDupe("moveright");
    }
    [MenuItem("Edit/Move/Left % LEFT")]
    private static void MoveLeft()
    {
        MoveAndOrDupe("moveleft");
    }
    [MenuItem("Edit/Move/Up % UP")]
     private  static void MoveUp()
    {
        MoveAndOrDupe("moveup");
    }
    [MenuItem("Edit/Move/Down % DOWN")]
     private static void MoveDown()
    {
        MoveAndOrDupe("movedown");
    }
    [MenuItem("Edit/Dupe/Right #% RIGHT")]
    private static void DupeRight()
    {
        MoveAndOrDupe("duperight");
    }
    //shift key should duplicate
    [MenuItem("Edit/Dupe/Left #% LEFT")]
    private static void DupeLeft()
    {
        MoveAndOrDupe("dupeleft");
    }
    [MenuItem("Edit/Dupe/Up #% UP")]
    private static void DupeUp()
    {
        MoveAndOrDupe("dupeup");
    }
    [MenuItem("Edit/Dupe/Down #% DOWN")]
    private static void DupeDown()
    {
        MoveAndOrDupe("dupedown");
    }
        #endregion //end controlplusarrowkeymenuaction
    }

} // end namespace Utilities