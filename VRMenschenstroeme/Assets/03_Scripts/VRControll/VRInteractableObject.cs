using UnityEngine;

public class VRInteractableObject : MonoBehaviour {

    protected Rigidbody rigidBody;
    protected bool originalKinematicState;
    protected Transform originalParent;
    private int index;
    public bool clonedObject = false;

    private void Awake() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        //Capture object's original parent and kinematic state
        originalParent = transform.parent;
        originalKinematicState = rigidBody.isKinematic;
    }

    public VRInteractableObject Pickup(VRControllerInput controller) {
        if (clonedObject) {
            return PickupAfterClone(controller);
        } else {
            return clonePickup(controller);
        }
    }

    private VRInteractableObject clonePickup(VRControllerInput controller) {
        GameObject newObject = Instantiate(this.gameObject, this.transform.position, this.transform.rotation);
        VRInteractableObject newInteractableObject = newObject.GetComponent(typeof(VRInteractableObject)) as VRInteractableObject;
        newInteractableObject.transform.localScale = gameObject.transform.lossyScale;
        newObject.gameObject.name = this.name + "_" + index++;
        newInteractableObject.GetComponent<Rigidbody>().isKinematic = true;
        newInteractableObject.originalParent = originalParent;
        newInteractableObject.originalKinematicState = originalKinematicState;
        newInteractableObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        newInteractableObject.transform.SetParent(controller.gameObject.transform);
        newInteractableObject.clonedObject = true;
        return newInteractableObject;
    }

    private VRInteractableObject PickupAfterClone(VRControllerInput controller) {
        //Make object kinematic
        //(Not effected by physics, but still able to effect other objects with physics)
        rigidBody.isKinematic = true;
        rigidBody.constraints = RigidbodyConstraints.None;
        //Parent object to hand
        transform.SetParent(controller.gameObject.transform);
        return this;
    }

    public void Release(VRControllerInput controller) {
        //this.rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

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
            //Throw Object
            rigidBody.velocity = controller.Device.velocity;
            rigidBody.angularVelocity = controller.Device.angularVelocity;
        }
    }
}
