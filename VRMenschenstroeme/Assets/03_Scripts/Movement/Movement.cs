using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    ReadMovement _readMovement;
    ReadConfig _conf;
    Creator _creator;
    List<Vector3> movesList = new List<Vector3>();
    int index = 0;
    // Use this for initialization
    void Start() {
        //GameObject _map = GameObject.Find("Map");
        //_readMovement = GameObject.Find("Map").GetComponent("ReadMovement") as ReadMovement;
        //_conf = GameObject.Find("Map").GetComponent("ReadConfig") as ReadConfig;
        //_creator = GameObject.Find("Map").GetComponent("Creator") as Creator;
        //foreach (List<Structs.PersonPosition> list in _readMovement.MapOfPersons) {
        //    foreach (Structs.PersonPosition person in list) {
        //        if (person.id.ToString() == this.name) {
        //            if (person.p.ToVectorThree() != new Vector3(0f, 0f, 0f)) {
        //                movesList.Add((person.p.ToVectorThree() * _conf.Length)
        //                    + new Vector3(0, 1f, 0));
        //            }
        //        }
        //    }
        //}
    }
    void Update() {
        if (index != movesList.Count - 1) {
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, movesList[index + 1], 1 * _conf.Time * Time.deltaTime);
            if (this.transform.localPosition == movesList[index + 1] && index < movesList.Count) {
                index++;
            }
        } else { Destroy(gameObject); }
    }
}


