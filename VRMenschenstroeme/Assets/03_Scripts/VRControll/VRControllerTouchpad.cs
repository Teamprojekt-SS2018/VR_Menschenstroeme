using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRControllerTouchpad : VRControllerInput {


    // Update is called once per frame
    void Update() {
        if (preventDoubleClick) {
            if (Device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) { TriggerPressUpEventHandler(); } else
            if (Device.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu)) { ApplicationMenuPressedEventHandler(); } else
            if (Device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) { GripPressedEventHandler(); }
            if (Device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad)) { TouchpadRightPressedEventHandler(); }

        }
    }


    //All Conditions possible on the Touchpad
    public enum TouchPosition {
        Off, Up, Down, Left, Right
    }

    //Returns ToucPosition depending in which quarter of the Touchpad the usesr pressed
    public TouchPosition CurrentTouchPosition() {
        Vector2 pos = Device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
        bool isTop = pos.y >= 0;
        bool isRight = pos.x >= 0;
        if (isTop) {
            if (isRight) {
                if (pos.y > pos.x) {
                    return TouchPosition.Up;
                } else if (pos.y < pos.x) {
                    return TouchPosition.Right;
                }
            } else {
                if (pos.y > -pos.x) {
                    return TouchPosition.Up;
                } else if (pos.y < -pos.x) {
                    return TouchPosition.Left;
                }
            }
        } else {
            if (isRight) {
                if (-pos.y > pos.x) {
                    return TouchPosition.Down;
                } else if (-pos.y < pos.x) {
                    return TouchPosition.Right;
                }
            } else {
                if (-pos.y > -pos.x) {
                    return TouchPosition.Down;
                } else if (-pos.y < -pos.x) {
                    return TouchPosition.Left;
                }
            }
        }
        return TouchPosition.Off;
    }


    private void TouchpadRightPressedEventHandler() {
        switch (CurrentTouchPosition()) {
            case TouchPosition.Up:
            if (ManagerData.Instance.currentGameState == GameState.Play) {
                ManagerData.Instance.ChangeState(GameState.Pause);
            } else {
                ManagerData.Instance.ChangeState(GameState.Play);
            }
            break;
            case TouchPosition.Down:
            ManagerData.Instance.ChangeState(GameState.Reset);
            break;
            case TouchPosition.Left:
            ManagerData.Instance.ChangeState(GameState.Revert);
            break;
            case TouchPosition.Right:
            ManagerData.Instance.ChangeState(GameState.Forward);
            break;
            case TouchPosition.Off:
            break;
        }
    }
}
