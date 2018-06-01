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

    //public List<List<Structs.Person>> MapOfPersons = new List<List<Structs.Person>>();
    //private Vector3[] currentPos = new Vector3[MapOfPersons.Length - 1];
    //private Vector3 endPosition = new Vector3(0f, 0f, 0f);
    //private int index = 0;
    //// Use this for initialization
    //void Start()
    //{
    //    for (int i = 0; i < MapOfPersons.Length; i++)
    //    {
    //        foreach (Person p in MapOfPersons[i])
    //        {
    //            if (is_In == true)
    //            {
    //                Create GameOb at Point p.p
    //                currentPos[i] = p.vec3;
    //            }
    //        }
    //    }

    //    // Update is called once per frame
    //    void Update()
    //    {
    //        for (int i = 0; i < MapOfPersons.Length; i++)
    //        {
    //            foreach (Person p in MapOfPersons[i])
    //            {
    //                transform.position = Vector3.MoveTowards(, p.p, 1 * Time.deltaTime * _conf.);
    //                currentPos[i] = p.vec3;
    //            }
    //        }
    //        transform.position = Vector3.MoveTowards(transform.position, endPosition, 5 * Time.deltaTime);
    //        if (transform.position == endPosition && index < way.Length - 1)
    //        {
    //            if (way[index + 1] != new Vector3(0f, 0f, 0f))
    //            {
    //                ++index;
    //                endPosition = way[index];
    //            }
    //        }
    //    }
    //}
}
