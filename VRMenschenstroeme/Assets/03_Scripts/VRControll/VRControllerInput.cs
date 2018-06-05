using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class VRControllerInput : MonoBehaviour {

    //Should only ever be one, but just in case
    protected List<VRInteractableObject> heldObjects;
    public bool groß = false;
    public GameObject player;
    public double grabDistance = 0.3;
    private bool scaling = false;
    private VRInteractableObject scalingObject;

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
        //init collider 
        BoxCollider sc = gameObject.AddComponent<BoxCollider>() as BoxCollider;
        sc.isTrigger = true;
        sc.center = new Vector3(0f, 0f, 0f);
        sc.size = new Vector3(1f, 1f, 1f);
    }

    private void OnTriggerStay(Collider collider) {
        //If object is an interactable item
        VRInteractableObject interactableObject = collider.GetComponent<VRInteractableObject>();
        if (interactableObject != null && device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            Debug.Log("Trigger down");
            if (!interactableObject.IsAlreadyPickedup) {
                double distanceInteractableObj = Vector3.Distance(interactableObject.transform.position, transform.position);

                //if trigger button is down
                if (distanceInteractableObj < grabDistance &&
                    heldObjects.Count < 1) {
                    //Pick up object
                    interactableObject.Pickup(this);
                    heldObjects.Add(interactableObject);
                    Debug.Log("Trigger pressed down");
                }
            } else {
                interactableObject.startScaling(this);
                scaling = true;
            }
        }
    }

    private void Update() {
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) { TriggerPressedEventHandler(); } else
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu)) { ApplicationMenuPressedEventHandler(); } else
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) { GripPressedEventHandler(); }

    }
    private void TriggerPressedEventHandler() {
        //If trigger is released
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
            scaling = false;
            if (scalingObject != null) {
                scalingObject.stopScaling();
            }
            //Release any held objects
            heldObjects.ForEach(x => x.Release(this));
            heldObjects = new List<VRInteractableObject>();
        }
    }

    private void ApplicationMenuPressedEventHandler() {
        int nextScene = (SceneManager.GetActiveScene().buildIndex * -1) + 1;
        SceneManager.LoadSceneAsync(nextScene);
    }

    private void GripPressedEventHandler() {
        if (SceneManager.GetActiveScene().buildIndex == 1) {
            if (groß) {
                groß = false;
                player.transform.localScale = new Vector3(1f, 1f, 1f);
            } else {
                groß = true;
                player.transform.localScale = new Vector3(2f, 2f, 2f);
            }
        }
    }
}