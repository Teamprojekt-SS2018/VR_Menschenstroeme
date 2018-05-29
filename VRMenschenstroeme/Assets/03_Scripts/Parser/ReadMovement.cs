using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public partial class ReadMovement : MonoBehaviour
{
    private string movementDat = "VRData.dat";

    public List<List<Structs.Person>> MapOfPersons = new List<List<Structs.Person>>();
    
    // Use this for initialization
    /*
     * ReadConfig muss vor ReadTrianulation laufen, da die xxx.am_fmt in der Config.dat definiert ist.
     */
    void Awake()
    {
        ReadMovementDat();
    }

    void ReadMovementDat()
    {
        string[] movementData = File.ReadAllLines(movementDat);
        int prevID = -1;
        List<Structs.Person> current = new List<Structs.Person>();
        for (int i = 0; i < movementDat.Length; i++)
        {
            string line = movementData[i];
            string[] splitted = Helper.SplitWhitespace(movementData[i + 1]);
            int ID = int.Parse(splitted[0]);
            bool is_In = bool.Parse(splitted[1]);
            Structs.Point p = new Structs.Point(float.Parse(splitted[3]), float.Parse(splitted[4]));
            float density = float.Parse(splitted[5]);
            float velocity = float.Parse(splitted[6]);

            if (prevID > ID)
            {
                MapOfPersons.Add(current);
                current = new List<Structs.Person>();
                
            }
            prevID = ID;
            current.Add(new Structs.Person(p, is_In, density, velocity));
            System.Console.Write("test");
        }
        MapOfPersons.Add(current);
    }
}
