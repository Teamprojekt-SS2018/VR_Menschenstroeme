using System.Threading;
using UnityEngine;

public class LoadButtonTrigger : MonoBehaviour {
    
    public bool preventDoubleClick = true;
    public int preventDoubleClickWaitTime = 500;
    private void AvoidDoubleClick() {
        new Thread(() => { Thread.Sleep(preventDoubleClickWaitTime); preventDoubleClick = true; }).Start();
    }
    
    void Update() {
        if (Input.GetKey(KeyCode.F6)) {
            CallLoad();
        }
    }

    private void OnTriggerStay(Collider collider) {
        VRControllerInput controller = collider.GetComponent<VRControllerInput>();
        if (ManagerData.Instance.tableplate != null && controller != null && controller.Device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
            CallLoad();
        }
    }

    private void CallLoad() {
        if (preventDoubleClick) {
            preventDoubleClick = false;
            AvoidDoubleClick();
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            Color originColor = meshRenderer.material.color;
            meshRenderer.material.color = Color.red;
            ManagerData.Instance.tableplate.GetComponent<SaveLoad_PlacedObjects>().load();
            meshRenderer.material.color = originColor;
        }
    }
}
