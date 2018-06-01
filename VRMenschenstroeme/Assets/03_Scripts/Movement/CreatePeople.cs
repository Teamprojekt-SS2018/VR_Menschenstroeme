using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePeople : MonoBehaviour
{
    public GameObject capsule;
    private ReadConfig _conf;
    private ReadMovement _move;
    private Creator _creator;
    void Start()
    {
        _move = this.gameObject.GetComponent<ReadMovement>();
        _conf = this.gameObject.GetComponent<ReadConfig>();
        _creator = this.gameObject.GetComponent<Creator>();
        int count = 0;
        List<int> saves = new List<int>();
        foreach (List<Structs.Person> list in _move.MapOfPersons)
        {
            foreach (Structs.Person person in list)
            {
                if (!saves.Contains(person.id))
                {
                    if (person.created == false)
                    {
                        GameObject caps = Instantiate(capsule, (person.p.ToVectorThree() * _conf.Length * _creator.scale) + new Vector3(0, 1f * _creator.scale, 0), this.transform.rotation);
                        caps.name = count.ToString();
                        caps.transform.localScale = caps.transform.localScale * _creator.scale;
                        saves.Add(person.id);
                        ++count;
                    }
                }
            }

        }


    }
}
