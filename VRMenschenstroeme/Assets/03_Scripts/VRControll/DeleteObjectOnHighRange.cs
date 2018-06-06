using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObjectOnHighRange : MonoBehaviour {

    private GameObject _map;
    private Vector3 originPosition;
    [Range(1f, 10f)]
    public float DeleteRange = 3f;

    // Use this for initialization
    void Start() {
        _map = GameObject.Find("SimulationTable_3");
        originPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (Vector3.Distance(this.transform.position, originPosition) > DeleteRange) {
            Destroy(gameObject);
        }
    }
}
