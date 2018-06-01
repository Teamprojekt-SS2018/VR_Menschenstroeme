﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class VRControllerInput : MonoBehaviour {

    //Should only ever be one, but just in case
    protected List<VRInteractableObject> heldObjects;
    public bool groß = false;
    public GameObject player;

    //Controller References
    protected SteamVR_TrackedObject trackedObject;
    public SteamVR_Controller.Device device {
        get {
            return SteamVR_Controller.Input((int)trackedObject.index);
        }
    }

    private void Awake() {
        //Instantiate lists
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        heldObjects = new List<VRInteractableObject>();
    }

    private void OnTriggerStay(Collider collider) {
        //If object is an interactable item
        VRInteractableObject interactableObject = collider.GetComponent<VRInteractableObject>();
        if (interactableObject != null) {

            double distanceInteractableObj = Vector3.Distance(interactableObject.transform.position, transform.position);
            double grabDistance = 0.3;
            Debug.Log("grabDistance:" + distanceInteractableObj);

            //if trigger button is down
            if (device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger) && distanceInteractableObj < grabDistance) {
                //Pick up object
                if (heldObjects.Count < 1) {
                    interactableObject.Pickup(this);
                    heldObjects.Add(interactableObject);
                }
            }
        }
    }

    private void Update() {
        if (SceneManager.GetActiveScene().buildIndex == 1 && device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger)) {
            Debug.Log("Trigger Pressed");
            if (groß) {
                groß = false;
                player.transform.localScale = new Vector3(1f, 1f, 1f);
            } else {
                groß = true;
                player.transform.localScale = new Vector3(2f, 2f, 2f);
            }
        } else if (heldObjects.Count > 0) {
            //If trigger is released
            if (device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger)) {
                //Release any held objects
                heldObjects.ForEach(x => x.Release(this));
                heldObjects = new List<VRInteractableObject>();
            }
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
            Debug.Log("Switching scene");
            if (SceneManager.GetActiveScene().buildIndex == 1) {
                SceneManager.LoadSceneAsync(0);
            } else {
                SceneManager.LoadSceneAsync(1);
            }
        }
    }
}