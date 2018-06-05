using UnityEngine;

public class VRInteractableObject : MonoBehaviour {

    protected Rigidbody rigidBody;
    protected bool originalKinematicState;
    protected Transform originalParent;
    private bool isPickedup = false;
    public bool IsAlreadyPickedup { get { return isPickedup; } }
    private bool scalingMode = false;
    private Vector3 scalingStartPoint;

    private void Awake() {
        rigidBody = GetComponent<Rigidbody>();

        //Capture object's original parent and kinematic state
        originalParent = transform.parent;
        originalKinematicState = rigidBody.isKinematic;
    }

    public void Pickup(VRControllerInput controller) {
        //Make object kinematic
        //(Not effected by physics, but still able to effect other objects with physics)
        rigidBody.isKinematic = true;
        //Parent object to hand
        transform.SetParent(controller.gameObject.transform);
        isPickedup = true;
    }

    public void Release(VRControllerInput controller) {
        //Make sure the hand is still the parent
        //Could have been transfered to another hand
        if (transform.parent == controller.gameObject.transform) {

            //Return previous kinematic state
            rigidBody.isKinematic = originalKinematicState;

            //Set object's parent to its original parent
            if (originalParent != controller.gameObject.transform) {
                //Ensure original parent recorded wasn't somehow the controller (failsafe)
                transform.SetParent(originalParent);
            } else {
                transform.SetParent(null);
            }
            isPickedup = false;
            //Throw Object
            rigidBody.velocity = controller.device.velocity;
            rigidBody.angularVelocity = controller.device.angularVelocity;
        }
    }

    public void startScaling(VRControllerInput controller) {
        if (!scalingMode) {
            scalingMode = true;
            scalingStartPoint = controller.transform.position;
        }

        float dist = Vector3.Distance(scalingStartPoint, controller.transform.position);

        this.transform.localScale = this.transform.localScale + new Vector3(dist, dist, dist);
    }

    public void stopScaling() {
        scalingMode = false;
    }
}
