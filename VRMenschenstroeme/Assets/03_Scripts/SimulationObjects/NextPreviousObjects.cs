using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum State {
    next, previous
}


public class NextPreviousObjects : MonoBehaviour {


    public GameObject ObjectManager;
    public State ButtonType;
    private LoadPrefabsInResources _loadPrefabsInResources;

    private void Start() {
        if (ObjectManager != null) {
            _loadPrefabsInResources = ObjectManager.GetComponent<LoadPrefabsInResources>();
        }
    }


    private void OnTriggerStay(Collider collider) {
        //If object is an interactable item
        VRControllerInput controller = collider.GetComponent<VRControllerInput>();

        if (_loadPrefabsInResources != null && controller != null && controller.Device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            if (ButtonType == State.next) {
                _loadPrefabsInResources.AddNextGameObjectsToShelf();
            } else if (ButtonType == State.previous) {
                _loadPrefabsInResources.AddPreviousGameObjectsToShelf();
            }
        }
    }
}
