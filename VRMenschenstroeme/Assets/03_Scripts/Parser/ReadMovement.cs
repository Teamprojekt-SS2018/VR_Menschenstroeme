using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public partial class ReadMovement : MonoBehaviour {
    public string movementDat;
    public Dictionary<int, List<Structs.PersonPosition>> persons;


    void Awake() {
        movementDat = ManagerData.Instance.movementData;
        ReadMovementDat();
        ManagerData.Instance.ReadMovement = this;
    }

    void ReadMovementDat() {
        string data = File.ReadAllText(movementDat);
        string[] movementData = data.Split('\n');

        this.persons = new Dictionary<int, List<Structs.PersonPosition>>();
        string[] splitted;

        int id = -1;

        foreach (var line in movementData) {
            if (line.Length > 2) {
                splitted = Helper.SplitWhitespace(line);

                id = int.Parse(splitted[0]);

                if (!persons.ContainsKey(id)) {
                    persons.Add(id, new List<Structs.PersonPosition>());
                }
                if (splitted[1].Equals("1")) {
                    persons[id].Add(new Structs.PersonPosition(new Vector3(Single.Parse(splitted[3].Replace('.', ',')), 0, Single.Parse(splitted[4].Replace('.', ','))), Single.Parse(splitted[2].Replace('.', ',')), Single.Parse(splitted[5].Replace('.', ','))));
                } else {
                    persons[id].Add(new Structs.PersonPosition(Single.Parse(splitted[2].Replace('.', ','))));
                }
            }
        }
    }
}
