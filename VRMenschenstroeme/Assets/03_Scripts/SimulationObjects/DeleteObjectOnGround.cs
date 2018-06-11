using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObjectOnGround : MonoBehaviour {


    void OnCollisionEnter(Collision col) {
        //If object is an interactable item
        VRInteractableObject interactableObject = col.gameObject.GetComponent<VRInteractableObject>();
        if (interactableObject != null && interactableObject.tag == "SimulationObject") {
            Destroy(col.gameObject);
        }
    }
}
