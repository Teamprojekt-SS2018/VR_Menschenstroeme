using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class VRControllerInput : MonoBehaviour {

    public GameObject player;
    public double grabDistance = 0.3;
    public VRControllerInput otherController;

    protected List<VRInteractableObject> heldObjects;
    private bool groß = false;
    private bool scaling = true;
    private VRInteractableObject scalingObject;
    private float startDist = 0f;
    private Vector3 startScale = Vector3.zero;

    private GameObject _tablePlate;
    private GameObject _map;
    private Transform _tablePlateDefaultParent;
    private Vector3 _tablePlateDefaultLocalScale;
    private Vector3 _tablePlateDefaultLocalPosition;
    private Vector3 _playerDefaultLocalPosition;

    //Controller References
    protected SteamVR_TrackedObject trackedObject;
    public SteamVR_Controller.Device Device {
        get {
            return SteamVR_Controller.Input((int)trackedObject.index);
        }
    }

    private void Start() {
        // Speichern der Defaultwerte zum Teleportieren in die Simulation und wieder zurueck
        _map = ManagerData.Instance.map;
        _tablePlate = ManagerData.Instance.tableplate;
        _tablePlateDefaultParent = _tablePlate.transform.parent;
        _tablePlateDefaultLocalScale = new Vector3(_tablePlate.transform.localScale.x, _tablePlate.transform.localScale.y, _tablePlate.transform.localScale.z);
        _tablePlateDefaultLocalPosition = new Vector3(_tablePlate.transform.localPosition.x, _tablePlate.transform.localPosition.y, _tablePlate.transform.localPosition.z);
        _playerDefaultLocalPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y, player.transform.localPosition.z);
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

    //Countdown, preventing doublepress on press
    protected bool preventDoubleClick = true;
    public int preventDoubleClickWaitTime = 100;
    private void AvoidDoubleClick() {
        new Thread(() => { Thread.Sleep(preventDoubleClickWaitTime); preventDoubleClick = true; }).Start();
    }

    private void Update() {
        if (preventDoubleClick) {
            if (Device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) { TriggerPressUpEventHandler(); } else
            if (Device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu)) { ApplicationMenuPressedEventHandler(); } else
            if (Device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) { GripPressedEventHandler(); }
        }
    }

    private void OnTriggerStay(Collider collider) {
        VRInteractableObject interactableObject = collider.GetComponent<VRInteractableObject>();

        //Falls das Kollidierte Objekt bereits von dem anderen Controller gehalten wird und dieser Controller den Trigger betaetigt hat, soll das Objekt Skaliert werden
        if (interactableObject != null
            && interactableObject.transform.parent.name != null
            && interactableObject.transform.parent.name == otherController.name
            && Device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
            scaleSelected(interactableObject.gameObject);
        } else
        if (interactableObject != null && Device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            double distanceInteractableObj = Vector3.Distance(interactableObject.transform.position, transform.position);

            //if trigger button is down
            if (distanceInteractableObj < grabDistance &&
                heldObjects.Count < 1) {
                //Pick up object
                heldObjects.Add(interactableObject.Pickup(this));

            }
        }
    }
    public void TriggerPressUpEventHandler() {
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

    public void ApplicationMenuPressedEventHandler() {
        if (_tablePlate.transform.parent != null) {
            _tablePlate.transform.parent = null;
            _tablePlate.transform.localScale = new Vector3(
                1f / _map.transform.localScale.x,
                1f / _map.transform.localScale.y,
                1f / _map.transform.localScale.z);
            _tablePlate.transform.localPosition = new Vector3(999f, 0f, 0f);
            try {
                Transform humanScale = _tablePlate.transform.Find("Map").transform.Find("HumanScale");
                player.transform.position = new Vector3(
                    humanScale.transform.position.x,
                    humanScale.transform.position.y,
                    humanScale.transform.position.z);
            } catch {
                Debug.Log("HumanScale not found!!!");
            }
        } else {
            try {
                Transform humanScale = _tablePlate.transform.Find("Map").transform.Find("HumanScale");
                humanScale.transform.localPosition = new Vector3(
                    humanScale.transform.localPosition.x,
                    humanScale.transform.localPosition.y + 2f,
                    humanScale.transform.localPosition.z);
            } catch {
                Debug.Log("HumanScale not found!!!");
            }
            _tablePlate.transform.parent = _tablePlateDefaultParent;
            _tablePlate.transform.localPosition = _tablePlateDefaultLocalPosition;
            _tablePlate.transform.localScale = _tablePlateDefaultLocalScale;
            player.transform.localPosition = _playerDefaultLocalPosition;
        }
    }

    public void GripPressedEventHandler() {
        if (groß) {
            groß = false;
            player.transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            groß = true;
            player.transform.localScale = new Vector3(2f, 2f, 2f);
        }
    }
}