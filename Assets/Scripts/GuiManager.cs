using UnityEngine;
using System.Collections;


public class GuiManager : MainGameInit {
    // the idea here is all the Gui is under control, eventually
    // we'll have some parts of the window mapped out for messages
    //public MainGameInit mainGameInit;
   
    void Awake()
    {
        //initFile = GetComponent(typeof(MainGameInit)) as MainGameInit;
    }

	// Use this for initialization
	void Start () {
        print("GuiManager.cs active. initFile = " + mainGameInit);
    
	}

    // OnGUI is auto updating the top GUI box
    public void OnGUI()
    {
        // string textupdate = screenText;
        //print(screenText.Count);
        GUI.Label(new Rect(10, 10, 700, 200), mainGameInit.screenText[mainGameInit.screenText.Count - 1]);
    }

    // end GUiManager
}


   