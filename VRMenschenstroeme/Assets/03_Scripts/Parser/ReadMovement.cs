using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public partial class ReadMovement : MonoBehaviour
{
    private string movementDat = "VRData.dat";

    public List<List<Structs.Person>> MapOfPersons = new List<List<Structs.Person>>();
    
    void Awake()
    {
        ReadMovementDat();
    }

    void ReadMovementDat()
    {
        string data = File.ReadAllText(movementDat);
        string[] movementData = data.split("\n");

        this.persons = new Dictionary<int, List<Structs.PersonPostition>>
        string[] splitted;

        int id = -1;

        foreach (var line in movementData) 
        {
            splitted = Helper.SplitWhitespace(line);

            id = int.Parse(splited[0]);

            if (!persons.ContainsKey(id))
            {
                persons.Add(id, new List<PersonPostition>());
            }
            if (splitted[1].Equals("1")) 
            {
                persons[id].Add(new PersonPosition(new Vector3(float.Parse(splitted[3]) * rc.Length * cr.scale, 1, float.Parse(splitted[4]) * rc.Length * cr.scale), float.Parse(splitted[2])));
            } else
            {
                persons[id].Add(new PersonPosition(float.Parse(splitted[2])));
            }
        }
    }
}
