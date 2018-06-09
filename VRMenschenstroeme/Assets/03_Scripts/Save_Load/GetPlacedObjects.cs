using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlacedObjects : MonoBehaviour {

    // Declare and initialize a new List of GameObjects called currentCollisions.
    List<GameObject> objectsOnTable = new List<GameObject>();

    void OnCollisionEnter(Collision col) {
        // Add the GameObject collided with to the list.
        objectsOnTable.Add(col.gameObject);
    }

    void OnCollisionExit(Collision col) {
        // Remove the GameObject collided with from the list.
        objectsOnTable.Remove(col.gameObject);
    }

    public void save() {
        Debug.Log("Saving...");
        GameObject map  = transform.Find("Map").gameObject;

        objectsOnTable.ForEach(x=> x.transform.SetParent(map.transform));
        Debug.Log("Saved!");


    }

}
