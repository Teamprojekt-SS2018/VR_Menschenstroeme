using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreateBlobs : MonoBehaviour
{
    public GameObject capsule;
    public ReadMovement rm;
    public ReadConfig conf;
    public float maxTime = 0f;

    // Use this for initialization
    void Start()
    {
        conf = GetComponent<ReadConfig>();
        this.maxTime = rm.persons[0][rm.persons[0].Count - 1].time;
        foreach (var person in rm.persons)
        {
            MoveScript go = Instantiate(capsule, new Vector3(0f, 0f, 0f), this.transform.rotation).GetComponent<MoveScript>();
            go.transform.parent = gameObject.transform;
            go.transform.localPosition = person.Value[0].position * conf.Length + new Vector3(0, 1f, 0);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            go.posistions = person.Value;
            go.cb = this;
            go.conf = conf;
        }
    }
}