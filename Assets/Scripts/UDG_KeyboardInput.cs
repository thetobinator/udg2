using UnityEngine;
using System.Collections;


public class UDG_KeyboardInput : MonoBehaviour {

 public   void zombieKeyboardInput(GameObject obj)
    {
        ZombieBehavior z = obj.GetComponent<ZombieBehavior>();
        MainGameManager instance = (MainGameManager) this.GetComponent<MainGameManager>();
        if (z != null)
        {
            // udg Follow Player
            if (Input.GetKeyDown("f")) {
                z.GoToTag("Player");
                instance.zombieGroupSize =  GameObject.FindGameObjectsWithTag("Zombie").Length;
            }
            // udg Rush
            if (Input.GetKeyDown("r")) {
                z.GoToTag("Human");
               instance.zombieGroupSize = GameObject.FindGameObjectsWithTag("Zombie").Length;
            }
            // udg Barricade
            if (Input.GetKeyDown("b")) {
                z.GoToTag("Barricade");
              instance.zombieGroupSize = GameObject.FindGameObjectsWithTag("Zombie").Length;
            }
            // udg Window
            if (Input.GetKeyDown("g")) {
                z.GoToTag("Window");
                instance.zombieGroupSize = GameObject.FindGameObjectsWithTag("Zombie").Length;
            }
        }
    }

    public int multipleZombieCommand(int zombieGroupSize)
    {
        int i = zombieGroupSize;
        if (Input.GetKeyDown(KeyCode.Alpha0)) { i = 10; }
        if (Input.GetKeyDown(KeyCode.Alpha1)) { i = 1; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { i = 2; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { i = 3; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { i = 4; }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { i = 5; }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { i = 6; }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { i = 7; }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { i = 8; }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { i = 9; }
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        if (i > zombies.Length) { i = zombies.Length; }
        return i;
    }

    
}
