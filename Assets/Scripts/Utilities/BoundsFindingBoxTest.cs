using UnityEngine;
using System.Collections;


public class BoundsFindBoxTest : MonoBehaviour
{

    Bounds bounds;

    void Update()
    {
        bounds = BoundsFindingBox.GetBounds(GameObject.FindGameObjectsWithTag("Untagged"));
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}

