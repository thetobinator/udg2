using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//[ExecuteInEditMode]
public class UMA_ApplyBehaviorsToChildren : MonoBehaviour
{
    public List<GameObject> sourceChildren;
    private GameObject sourceChild;
    // [ExecuteInEditMode]
    void Start()
    {
      
        //  sourceChildren.Clear();
        sourceChildren = new List<GameObject>(new GameObject[this.transform.childCount]);
        int i = 0;
        foreach (Transform child in this.GetComponent<Transform>())
        {
            sourceChildren[i] = (GameObject)child.gameObject;
            i++;
        }
        foreach (GameObject sourceChild in sourceChildren)
        {
            if (!sourceChild.GetComponent<NavMeshAgent>()) { sourceChild.AddComponent(typeof(NavMeshAgent)); }


            if (sourceChild.tag == "SpawnPoint_Human")
            {
                if (!sourceChild.GetComponent<HumanBehavior>())
                {
                    sourceChild.gameObject.tag = "Human";
                    sourceChild.gameObject.AddComponent<HealthComponent>();
                    sourceChild.gameObject.AddComponent<HumanBehavior>();

                     Animator animator = sourceChild.gameObject.GetComponent<Animator>();
                    animator.runtimeAnimatorController = Resources.Load("Animation Controllers/Human_AnimationController") as RuntimeAnimatorController;

                    //spawnsWeapons
                    //  SpawnStaff();
                    //  SpawnPistolUO();
                }
            }
            else
            {

                sourceChild.gameObject.tag = "Zombie";
                sourceChild.gameObject.AddComponent<HealthComponent>();
                sourceChild.gameObject.AddComponent<ZombieBehavior>();
                Animator animator = sourceChild.gameObject.GetComponent<Animator>();
                animator.runtimeAnimatorController = Resources.Load("Animation Controllers/ZombieAnimationController") as RuntimeAnimatorController;
                }
            }
        }
 
    
}



