using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    public Canvas quitMenu;
    public Button startText;
    public Button exitText;
	// Use this for initialization
	void Start () {
        quitMenu = quitMenu.GetComponent<Canvas>();
        startText = startText.GetComponent<Button>();
        exitText = exitText.GetComponent<Button>();
        quitMenu.enabled = false;
	}

    public void PressedExit() {
        quitMenu.enabled = true;
        startText.enabled = false;
        exitText.enabled = false;
    }

    public void PressedNo()
    {
        quitMenu.enabled = false;
        startText.enabled = true;
        exitText.enabled = true;
    }

    public void StartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Application.LoadLevel(1); // load level 1

    }

    public void ExitGame()
    {
        Application.Quit();

    }
	
}
