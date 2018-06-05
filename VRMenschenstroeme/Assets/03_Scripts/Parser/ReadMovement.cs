using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public partial class ReadMovement : MonoBehaviour
{
    private string movementDat = "VRData.dat";
    Dictionary<int, List<Structs.PersonPosition>> persons;

    public List<List<Structs.PersonPosition>> MapOfPersons = new List<List<Structs.PersonPosition>>();
    
    void Awake()
    {
        ReadMovementDat();
    }

    void ReadMovementDat()
    {
        string data = File.ReadAllText(movementDat);
        string[] movementData = data.Split('\n');

        this.persons = new Dictionary<int, List<Structs.PersonPosition>>();
        string[] splitted;

        int id = -1;

        foreach (var line in movementData) 
        {
            splitted = Helper.SplitWhitespace(line);

            id = int.Parse(splitted[0]);

            if (!persons.ContainsKey(id))
            {
                persons.Add(id, new List<Structs.PersonPosition>());
            }
            if (splitted[1].Equals("1")) 
            {
                persons[id].Add(new Structs.PersonPosition(new Vector3(float.Parse(splitted[3]), 1, float.Parse(splitted[4])), float.Parse(splitted[2])));
            } else
            {
                persons[id].Add(new Structs.PersonPosition(float.Parse(splitted[2])));
            }
        }
    }
}
