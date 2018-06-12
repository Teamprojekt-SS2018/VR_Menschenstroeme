using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButtonTrigger : MonoBehaviour {

    public GameObject table;

    void Update() {
        if (Input.GetKey(KeyCode.F5)) {
            CallSave();
        }
    }
    private void OnTriggerStay(Collider collider) {
        VRControllerInput controller = collider.GetComponent<VRControllerInput>();

        if (table != null && controller != null && controller.Device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            CallSave();
        }
    }

    private void CallSave() {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        Color originColor = meshRenderer.material.color;
        meshRenderer.material.color = Color.red;
        table.GetComponent<SaveLoad_PlacedObjects>().save();
        meshRenderer.material.color = originColor;
    }
}
