using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public partial class ReadConfig : MonoBehaviour
{
    private string movementDat = "something.dat";
    
    // Use this for initialization
    /*
     * ReadConfig muss vor ReadTrianulation laufen, da die xxx.am_fmt in der Config.dat definiert ist.
     */
    void Awake()
    {
        ReadMovementDat();
        ReadTriangulation();
    }

    void ReadMovement()
    {
        string[] movementData = File.ReadAllLines(movementDat);
        int prevID = -1;
        for (int i = 0; i < movementDat.Length; i++)
        {
            string line = movementData[i];
            Helper.spl
            if (prevID < )

        }
    }


    void ReadConfigDat()
    {
        string[] configData = File.ReadAllLines(config);
        for (int i = 0; i < configData.Length; i++)
        {
            string line = configData[i];
            if (line.StartsWith("#"))
            {
                if (line.Contains("Gebietsdaten"))
                {
                    triangulation = Helper.SplitWhitespace(configData[i + 1])[0];
                    globalRefinement = float.Parse(Helper.SplitWhitespace(configData[i + 2])[0]);
                    localRefinement = float.Parse(Helper.SplitWhitespace(configData[i + 3])[0]);
                    localRefinementMode = int.Parse(Helper.SplitWhitespace(configData[i + 4])[0]);
                    i = i + 4;
                }
                else if (line.Contains("Charakteristische Laengen"))
                {

                    length = float.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                    time = float.Parse(Helper.SplitWhitespace(configData[i + 2])[0]);
                    dencity = float.Parse(Helper.SplitWhitespace(configData[i + 3])[0]);
                    velocity = float.Parse(Helper.SplitWhitespace(configData[i + 4])[0]);
                    i = i + 4;
                }
                else if (line.Contains("Anfangsbedingung"))
                {
                    personCount = int.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                    runtime = int.Parse(Helper.SplitWhitespace(configData[i + 2])[0]);
                    i = i + 2;
                }
                else if (line.Contains("Eingang"))
                {
                    int EntranceCount = int.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                    for (int n = 1; n <= EntranceCount; n++)
                    {
                        string[] splitted = Helper.SplitWhitespace(configData[i + n + 1]);

                        Structs.Entrance currentEntrance = new Structs.Entrance(float.Parse(splitted[0]),
                        float.Parse(splitted[1]),
                        float.Parse(splitted[2]),
                        float.Parse(splitted[3]),
                        float.Parse(splitted[4]),
                        float.Parse(splitted[5]));
                        entrances.Add(currentEntrance);
                    }
                    i = i + EntranceCount;
                }
                else if (line.Contains("Ausgang"))
                {
                    exitCount = int.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                    for (int n = 1; n <= exitCount; n++)
                    {
                        string[] splitted = Helper.SplitWhitespace(configData[i + n + 1]);
                        Structs.Exit currentExit = new Structs.Exit(float.Parse(splitted[0]),
                        float.Parse(splitted[1]),
                        float.Parse(splitted[2]),
                        float.Parse(splitted[3]));
                        exits.Add(currentExit);
                    }
                    i = i + exitCount;
                }
                else if (line.Contains("Messstationen"))
                {
                    stationCount = int.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                    for (int n = 1; n <= stationCount; n++)
                    {
                        string[] splitted = Helper.SplitWhitespace(configData[i + n + 1]);
                        Vector4 currentStation = new Vector4(float.Parse(splitted[0]),
                        float.Parse(splitted[1]),
                        float.Parse(splitted[2]),
                        float.Parse(splitted[3].Split('\t')[0]));
                        stations.Add(currentStation);
                    }
                    i = i + stationCount;
                }
                else if (line.Contains("Tracking"))
                {
                    trackingCount = int.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                }
            }
        }
    }

    void ReadTriangulation()
    {
        string[] configData = File.ReadAllLines(triangulation);
        pointCount = int.Parse(Helper.SplitWhitespace(configData[0])[1]);
        vertCount = int.Parse(Helper.SplitWhitespace(configData[0])[2]);
        points = new Structs.Point[pointCount + 1];
        vertices = new int[3 * (vertCount + 1)];
        int tmp = vertCount / 2 + 1;
        int a = 0;

        for (int i = tmp; i <= tmp + (pointCount / 2) - 1; i++)
        {
            string[] splitted = Helper.SplitWhitespace(configData[i]);
            points[a] = new Structs.Point(float.Parse(splitted[1]), float.Parse(splitted[2]));
            points[a + 1] = new Structs.Point(float.Parse(splitted[3]), float.Parse(splitted[4]));
            a = a + 2;
        }
        a = 0;
        for (int i = 1; i < tmp; i++)
        {
            string[] splitted = Helper.SplitWhitespace(configData[i]);
            vertices[a] = int.Parse(splitted[1]) - 1;
            vertices[a + 1] = int.Parse(splitted[2]) - 1;
            vertices[a + 2] = int.Parse(splitted[3]) - 1;
            vertices[a + 3] = int.Parse(splitted[4]) - 1;
            vertices[a + 4] = int.Parse(splitted[5]) - 1;
            vertices[a + 5] = int.Parse(splitted[6]) - 1;
            a = a + 6;
        }
    }
}
