using UnityEngine;

public class GUIHelper : MonoBehaviour 
{
   // public Texture icon;

    private void OnGUI()
    {
		GUI.Box(new Rect(10, 10, 430, 220), "Click here to start Camera");
       // GUI.DrawTexture(new Rect(20, 20, 32, 32), icon);
        GUI.Label(new Rect(20, 52, 500, 600), "Click on map to send characters to point.\n" +
                                              "Use the mouse to turn\n" +
                                              "Use W,A,S,D to move\n" +
		         							  "Use Escape to toggle the cursor (maximize on play in editor)\n" +
		          							  "F key makes zombies follow\n" +
		          							  "R key cause zombies to attack random target\n" +
		         							  "Humans choose random barricades and furniture to go to"
		         
		          																							
		          );//end GUI.Label
    }
}
