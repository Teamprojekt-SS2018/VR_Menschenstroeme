using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public partial class ReadConfig : MonoBehaviour {

    /* Config Variablen */
    private string triangulation;                 // XXX.am_fmt
    private string config = "ConfigMuenster.dat"; // Config.Dat

    /* Aus Config.Dat*/
    private int EntranceCount;
    private int exitCount;
    private int stationCount;
    private int localRefinementMode;
    private int personCount;
    private int runtime;
    private int trackingCount; 

    private float globalRefinement;
    private float localRefinement;
    private float length;
    private float time;
    private float dencity;
    private float velocity;

    private List<Structs.Entrance> Entrances = new List<Structs.Entrance>();
    private List<Structs.Exit> exits         = new List<Structs.Exit>();
    private List<Vector4> stations   = new List<Vector4>();

    /* Aus XXX.am_fmt */
    private int vertCount;
    private int pointCount;
    private Structs.Point[] points;
    private int[] vertices;

    // Use this for initialization
    /*
     * ReadConfig muss vor ReadTrianulation laufen, da die xxx.am_fmt in der Config.dat definiert ist.
     */
    void Start () {
        ReadConfigDat();
        ReadTriangulation();
    }
    
	// Update is called once per frame
	void Update () {
 
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
                    //Debug.Log ("Triangulation: " + triangulation);
                    globalRefinement = float.Parse(Helper.SplitWhitespace(configData[i + 2])[0]);
                    //Debug.Log ("Global Refinement: " + globalRefinement);
                    localRefinement = float.Parse(Helper.SplitWhitespace(configData[i + 3])[0]);
                    //Debug.Log ("Local Refinement: " + localRefinement);
                    localRefinementMode = int.Parse(Helper.SplitWhitespace(configData[i + 4])[0]);
                    //Debug.Log ("Local Refinement Mode: " + localRefinementMode);


                    i = i + 4;
                }
                else if (line.Contains("Charakteristische Laengen"))
                {

                    length = float.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("Length: " + length);
                    time = float.Parse(Helper.SplitWhitespace(configData[i + 2])[0]);
                    //Debug.Log ("Time: " + time);
                    dencity = float.Parse(Helper.SplitWhitespace(configData[i + 3])[0]);
                    //Debug.Log ("Dencity: " + dencity);
                    velocity = float.Parse(Helper.SplitWhitespace(configData[i + 4])[0]);
                    //Debug.Log ("Velocity: " + velocity);

                    i = i + 4;
                }
                else if (line.Contains("Anfangsbedingung"))
                {
                    personCount = int.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("personCount: " + personCount);
                    runtime = int.Parse(Helper.SplitWhitespace(configData[i + 2])[0]);
                    //Debug.Log ("runtime: " + runtime);
                    i = i + 2;
                }
                else if (line.Contains("Eingang"))
                {
                    int EntranceCount = int.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("Structs.Entrance Count: " + Structs.EntranceCount);
                    for (int n = 1; n <= EntranceCount; n++)
                    {
                        string[] splitted = Helper.SplitWhitespace(configData[i + n + 1]);

                        Structs.Entrance currentEntrance = new Structs.Entrance(float.Parse(splitted[0]),
                                                      float.Parse(splitted[1]),
                                                      float.Parse(splitted[2]),
                                                      float.Parse(splitted[3]),
                                                      float.Parse(splitted[4]),
                                                      float.Parse(splitted[5]));
                        Entrances.Add(currentEntrance);
                        //Debug.Log ("Eingang: " + currentStructs.Entrance.ToString()); 
                    }
                    i = i + EntranceCount;
                }
                else if (line.Contains("Ausgang"))
                {
                    exitCount = int.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("Exit Count: " + exitCount);
                    for (int n = 1; n <= exitCount; n++)
                    {
                        string[] splitted = Helper.SplitWhitespace(configData[i + n + 1]);
                        Structs.Exit currentExit = new Structs.Exit(float.Parse(splitted[0]),
                                                  float.Parse(splitted[1]),
                                                  float.Parse(splitted[2]),
                                                  float.Parse(splitted[3]));
                        exits.Add(currentExit);
                        //Debug.Log ("Ausgang: " + currentExit);
                    }
                    i = i + exitCount;
                }
                else if (line.Contains("Messstationen"))
                {
                    stationCount = int.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("Station Count: " + stationCount);
                    for (int n = 1; n <= stationCount; n++)
                    {
                        string[] splitted = Helper.SplitWhitespace(configData[i + n + 1]);
                        Vector4 currentStation = new Vector4(float.Parse(splitted[0]),
                                                     float.Parse(splitted[1]),
                                                     float.Parse(splitted[2]),
                                                     float.Parse(splitted[3].Split('\t')[0]));
                        stations.Add(currentStation);
                        //Debug.Log ("Station: " + currentStation);
                    }
                    i = i + stationCount;
                }
                else if (line.Contains("Tracking"))
                {
                    trackingCount = int.Parse(Helper.SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("tracking Count: " + trackingCount);
                }
            }
        }
    }

    void ReadTriangulation()
    {
        string[] configData = File.ReadAllLines(triangulation);
        pointCount = int.Parse(Helper.SplitWhitespace(configData[0])[1]);
        vertCount  = int.Parse(Helper.SplitWhitespace(configData[0])[2]);
        points = new Structs.Point[pointCount+1];
        vertices = new int[3 * (vertCount+1)];
        int tmp = vertCount / 2 + 1;
        int a = 0;

        for (int i = tmp; i <= tmp + (pointCount / 2) - 1; i++)
        {
            string[] splitted = Helper.SplitWhitespace(configData[i]);
            //Debug.Log(i + " - " + configData[i]);
            points[a] = new Structs.Point(float.Parse(splitted[1]), float.Parse(splitted[2]));
            points[a+1] = new Structs.Point(float.Parse(splitted[3]), float.Parse(splitted[4]));
            //Debug.Log(points[a] + " & " + points[a + 1]);
            a = a + 2;
        }
        a = 0;
        for (int i = 1; i < tmp; i++)
        {
            string[] splitted = Helper.SplitWhitespace(configData[i]);
            vertices[a]   = int.Parse(splitted[1]);
            vertices[a+1] = int.Parse(splitted[2]);
            vertices[a+2] = int.Parse(splitted[3]);
            vertices[a+3] = int.Parse(splitted[4]);
            vertices[a+4] = int.Parse(splitted[5]);
            vertices[a+5] = int.Parse(splitted[6]);
            a = a + 6;   
            //Debug.Log(vertices[a]);
        }
    }


    void OnDrawGizmos()
    {
        /*Gizmos.color = Color.yellow;
        for (int i = 0; i < vertices.Length; i++)
        {
            Debug.Log(vertices[i]);
            Gizmos.DrawLine(vertices[i].first.ToVectorThree(), vertices[i].second.ToVectorThree());
            Gizmos.DrawLine(vertices[i].second.ToVectorThree(), vertices[i].third.ToVectorThree());
            Gizmos.DrawLine(vertices[i].first.ToVectorThree(), vertices[i].third.ToVectorThree());
        }*/
    }
}
