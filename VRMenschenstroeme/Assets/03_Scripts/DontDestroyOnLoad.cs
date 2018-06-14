using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

    private void Awake() {
        //Finde alle GameObjects mit Tag DontDestroyOnLoadObject (Werden nicht Zerstört beim Laden einer neuen Szene)
        GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestroyOnLoadObject");
        //Beim hin und her wechseln wird das Gameobject nochmal erstellt in der hauptscene. damit keine Redundanz auftritt wird dieser gelöscht
        if (objs.Length > 1) { Destroy(this.gameObject); }
        DontDestroyOnLoad(this.gameObject);
    }
}
