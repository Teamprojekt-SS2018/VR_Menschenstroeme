using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadButtonTrigger : MonoBehaviour {

    public GameObject table;


    void Update() {
        if (Input.GetKey(KeyCode.F6)) {
            CallLoad();
        }
    }


    private void OnTriggerStay(Collider collider) {
        VRControllerInput controller = collider.GetComponent<VRControllerInput>();

        if (table != null && controller != null && controller.Device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            CallLoad();
        }
    }

    private void CallLoad() {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        Color originColor = meshRenderer.material.color;
        meshRenderer.material.color = Color.red;
        table.GetComponent<SaveLoad_PlacedObjects>().load();
        meshRenderer.material.color = originColor;
    }
}
