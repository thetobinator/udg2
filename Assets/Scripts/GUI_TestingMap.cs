using UnityEngine;
using System.Collections;
using System.Collections.Generic; // so we can use List<string> tText= new List<string>();
using UnityEngine.UI;

public class GUI_TestingMap : MonoBehaviour
{
    // public Texture icon;
    private float rr = 0.0f, gg = 0.0f, bb = 0.0f;
    private int rv = 1, gv = 1, bv = 1;
    private float slowColorTime = 0.0f;

    public int showScreenText;
    // screentext array of multiline text
    [Multiline]
    public List<string> screenText = new List<string>();

    public Color slowColor()
    {
        Color GUITextColor = new Color(rr, gg, bb);
        slowColorTime = (Time.deltaTime * 0.05f);
        // this little bit here cycles through colors very very slowly
        if (rr <= 0.0f) { rv = 1; }
        if (gg <= 0.0f) { gv = 1; }
        if (bb <= 0.0f) { bv = 1; }
        if (rr >= 0.99f) { rv = -1; }
        if (gg >= 0.99f) { gv = -1; }
        if (bb >= 0.99f) { bv = -1; }
        if (rr <= 1.0f && gg <= 0.9f) { rr += (slowColorTime * rv); }
        if (gg <= 1.0f && rr >= 0.5f) { gg += (slowColorTime * gv); }
        if (bb <= 1.0f && gg >= 0.5f) { bb += (slowColorTime * bv); }
        return GUITextColor;
    }


    private void OnGUI()
    {

        int screenTextCount = screenText.Count;
        if (showScreenText > screenTextCount-1) { showScreenText = screenTextCount - 1; }
        if (showScreenText < 0) { showScreenText = 0; }
        //GUI.Box(new Rect(10, 10, 430, 220), "Click here to start Camera");
        // GUI.DrawTexture(new Rect(20, 20, 32, 32), icon);
        GUI.contentColor = slowColor();

        GUI.Label(new Rect(20, 60, 500, 600), screenText[showScreenText]);
        #region old instructions to game
        /*string oldcontrolsScreenText = "Controls\n" +
                                 " Mouse to look around\n" +
                                 " Click   To send a zombie to a location, or to attack a target\n" +
                                 " +Additional clicks send more zombies.\n" +
                                 " \n" +
                                 " KEYS\n" +
                                 " W   move Forward\n" +
                                 " S   move Back\n" +
                                 " A   move Left\n" +
                                 " D   move Right\n" +
                                 " \n" +
                                 " H or SPACE: all zombies will hold position until given orders\n" +
                                 " \n" +
                                 " F   All zombies will follow the player\n" +
                                 " R   All zombies rush attack, seeking out any  living to eat\n" +
                                 " I   Activates the help text\n" +
                                 " Z   Toggles 'zombie time' slow motion\n" +
                                 " M   Toggles music\n" +
                                 " L   Toggles HUD\n" +
                                 " \n" +
                                 " CMD+O to access unlocked levels\n" +
                                 " \n" +
                                 " ESCAPE activates the menu"; */


        // previous text
        /*"Click on map to send characters to point.\n" +
                                          "Mouse to turn\n" +
                                          " W,A,S,D to move\n" +
                                          "F key makes zombies follow\n" +
                                          "R key cause zombies to follow random target\n" +
                                         "Escape hide the cursor (maximize on play in editor)\n" +
                                         "Right Click to show cursor\n"+
                                          "Space bar will activate Zoe's pistol\n" +
                                          "Humans choose random barricades,doors,boxes furniture to go to\n"
                                           */

        // Add some text to the file.
        // maybe read this from text files, this C# string stuff is hideous.

        #endregion //old instructionst to game 
    }

   
}
