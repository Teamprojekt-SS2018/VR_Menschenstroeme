using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSaveDataName : MonoBehaviour {
    public GameObject GOWithSaveNameComponent;
    // Use this for initialization
    void Start() {
        try {
            gameObject.GetComponent<TextMesh>().text = GOWithSaveNameComponent.GetComponent<SaveLoad_PlacedObjects>().SaveGameName;
        } catch {
            gameObject.GetComponent<TextMesh>().text = "Couldn't find SaveComponent!";
        }
    }
}
