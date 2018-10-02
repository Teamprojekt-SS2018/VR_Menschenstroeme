using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMapScale : MonoBehaviour {
    
    float scale;

    // Use this for initialization
    void Start() {
        GameObject _map = ManagerData.Instance.map;
        scale = (1 / this.gameObject.transform.parent.localScale.x) * _map.transform.localScale.x;
        this.gameObject.transform.GetChild(0).localScale = new Vector3(scale, scale, scale);
    }
}
