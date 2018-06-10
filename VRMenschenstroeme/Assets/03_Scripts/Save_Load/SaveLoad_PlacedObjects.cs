using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveLoad_PlacedObjects : MonoBehaviour {

    // Declare and initialize a new List of GameObjects called currentCollisions.
    List<GameObject> objectsOnTable = new List<GameObject>();
    public string SaveGameName = "SimulationSave";

    void OnCollisionEnter(Collision col) {
        // Add the GameObject collided with to the list.
        objectsOnTable.Add(col.gameObject);
    }

    void OnCollisionExit(Collision col) {
        // Remove the GameObject collided with from the list.
        objectsOnTable.Remove(col.gameObject);
    }

    public void save() {
        GameObject map = transform.Find("Map").gameObject;
        foreach (GameObject item in objectsOnTable) {
            item.transform.SetParent(map.transform);
        }
    }

    public void load() {
    }
}
