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
    void Start()
    {
        
        _readMovement = GameObject.Find("Gamefield").GetComponent("ReadMovement") as ReadMovement;
        _conf = GameObject.Find("Gamefield").GetComponent("ReadConfig") as ReadConfig;
        _creator = GameObject.Find("Gamefield").GetComponent("Creator") as Creator;
        foreach (List<Structs.Person> list in _readMovement.MapOfPersons)
        {
            foreach (Structs.Person person in list)
            {
                if (person.id.ToString() == this.name)
                {
                    if(person.p.ToVectorThree() != new Vector3(0f, 0f, 0f))
                    {
                        movesList.Add((person.p.ToVectorThree() * _conf.Length * _creator.scale) + new Vector3(0, 1f * _creator.scale, 0));
                    }
                }
            }
        }
    }
    void Update()
    {
        if(gameObject.name == "11")
        {
            Debug.Log(this.transform.position.ToString() + index.ToString());
        }
        if (index != movesList.Count - 1) {
            this.transform.position = Vector3.MoveTowards(this.transform.position, movesList[index + 1], 1 * _conf.Time * Time.deltaTime);
            if (this.transform.position == movesList[index + 1] && index < movesList.Count)
            {
                if (movesList[index + 1] != new Vector3(0f, 0f, 0f))
                {
                    index++;
                }
            }
        }
    }         
}
        

