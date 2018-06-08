using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class VRControllerInput : MonoBehaviour {

    //Should only ever be one, but just in case
    protected List<VRInteractableObject> heldObjects;
    private bool groß = false;
    public GameObject player;
    public double grabDistance = 0.3;
    private bool scaling = true;
    private VRInteractableObject scalingObject;
    public VRControllerInput otherController;
    private float startDist = 0f;
    private Vector3 startScale = Vector3.zero;

    //Controller References
    protected SteamVR_TrackedObject trackedObject;
    public SteamVR_Controller.Device Device {
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


        if (interactableObject != null
            && interactableObject.transform.parent.name != null
            && interactableObject.transform.parent.name == otherController.name
            && Device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
            scaleSelected(interactableObject.gameObject);
        } else if (interactableObject != null && Device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            double distanceInteractableObj = Vector3.Distance(interactableObject.transform.position, transform.position);

            //if trigger button is down
            if (distanceInteractableObj < grabDistance &&
                heldObjects.Count < 1) {
                //Pick up object
                heldObjects.Add(interactableObject.Pickup(this));

            }
        }
    }

    private void Update() {
        if (Device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) { TriggerPressUpEventHandler(); } else
        if (Device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu)) { ApplicationMenuPressedEventHandler(); } else
        if (Device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) { GripPressedEventHandler(); }

    }
    private void TriggerPressUpEventHandler() {
        //Release any held objects
        heldObjects.ForEach(x => x.Release(this));
        heldObjects = new List<VRInteractableObject>();
        scaling = true;
        startScale = Vector3.zero;
    }

    void scaleSelected(GameObject selected) {
        if (scaling) {
            scaling = false;
            startDist = Vector3.Distance(otherController.transform.position, transform.position);
            startScale = selected.transform.localScale;
        }

        float scale = Vector3.Distance(otherController.transform.position, transform.position) / startDist;
        selected.transform.localScale = startScale * scale;
    }

    private void ApplicationMenuPressedEventHandler() {
        int nextScene = (SceneManager.GetActiveScene().buildIndex * -1) + 1;
        SceneManager.LoadSceneAsync(nextScene);
    }

    private void GripPressedEventHandler() {
        if (groß) {
            groß = false;
            player.transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            groß = true;
            player.transform.localScale = new Vector3(2f, 2f, 2f);
        }
    }
}