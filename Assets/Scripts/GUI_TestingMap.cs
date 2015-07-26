using UnityEngine;

public class GUI_TestingMap : MonoBehaviour 
{
   // public Texture icon;

    private void OnGUI()
    {
		//GUI.Box(new Rect(10, 10, 430, 220), "Click here to start Camera");
       // GUI.DrawTexture(new Rect(20, 20, 32, 32), icon);
        GUI.Label(new Rect(20, 60, 500, 600), "Click on map to send characters to point.\n" +
                                              "Mouse to turn\n" +
                                              " W,A,S,D to move\n" +
		          							  "F key makes zombies follow\n" +
		          							  "R key cause zombies to follow random target\n" +
		                                      "Escape hide the cursor (maximize on play in editor)\n" +
		          							  "Right Click to show cursor\n"+
		          							  "Space bar will activate Zoe's pistol\n" +
		         							  "Humans choose random barricades,doors,boxes furniture to go to\n"
		                                    
		         
		          																							
		          );//end GUI.Label
    }
}
