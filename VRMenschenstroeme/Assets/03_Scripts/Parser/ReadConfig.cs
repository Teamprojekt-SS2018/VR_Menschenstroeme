using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;


public partial class ReadConfig : MonoBehaviour {

    /* Config Variablen */
    private string triangulation;                 // XXX.am_fmt
    private string config; // Config.Dat

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

    private List<Structs.Entrance> entrances = new List<Structs.Entrance>();
    private List<Structs.Exit> exits         = new List<Structs.Exit>();
    private List<Vector4> stations           = new List<Vector4>();

    /* Aus XXX.am_fmt */
    private int vertCount;
    private int pointCount;
    private Structs.Point[] points;
    private int[] vertices;

    // Use this for initialization
    /*
     * ReadConfig muss vor ReadTrianulation laufen, da die xxx.am_fmt in der Config.dat definiert ist.
     */
    void Awake () {
        config = ManagerData.Instance.configData;

        ReadConfigDat();
        if (triangulation.Contains("am_fmt"))
        {
            ReadTriangulation_fmt();
        }
        else if  (triangulation.Contains("msh"))
        {
            ReadTriangulation_msh();
        }

        
        ManagerData.Instance.ReadConfig = this;
    }

    List<Structs.Entrance> getEntrances()
    {
        return entrances;
    }

    List<Structs.Exit> getExits()
    {
        return exits;
    }

    List<Vector4> getStations()
    {
        return stations;
    }

    public float Length
    {
        get
        {
            return this.length;
        }
    }

    public Structs.Point[] Points
    {
        get
        {
            return this.points;
        }
    }

    public int[] Vertices
    {
        get
        {
            return this.vertices;
        }
    }

    public float Time
    {
        get
        {
            return this.time;
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
                    globalRefinement = Single.Parse(Helper.SplitWhitespace(configData[i + 2])[0].Replace('.', ','));
                    localRefinement = Single.Parse(Helper.SplitWhitespace(configData[i + 3])[0].Replace('.', ','));
                    localRefinementMode = int.Parse(Helper.SplitWhitespace(configData[i + 4])[0]);
                    i = i + 4;
                }
                else if (line.Contains("Charakteristische Laengen"))
                {

                    length = Single.Parse(Helper.SplitWhitespace(configData[i + 1])[0].Replace('.', ','));
                    time = Single.Parse(Helper.SplitWhitespace(configData[i + 2])[0].Replace('.', ','));
                    dencity = Single.Parse(Helper.SplitWhitespace(configData[i + 3])[0].Replace('.', ','));
                    velocity = Single.Parse(Helper.SplitWhitespace(configData[i + 4])[0].Replace('.', ','));
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

                        Structs.Entrance currentEntrance = new Structs.Entrance(Single.Parse(splitted[0].Replace('.', ',')) * length,
                        Single.Parse(splitted[1].Replace('.', ',')) * length,
                        Single.Parse(splitted[2].Replace('.', ',')) * length,
                        Single.Parse(splitted[3].Replace('.', ',')) * length,
                        Single.Parse(splitted[4].Replace('.', ',')),
                        Single.Parse(splitted[5].Replace('.', ',')));
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
                        Structs.Exit currentExit = new Structs.Exit(Single.Parse(splitted[0].Replace('.', ',')) * length,
                        Single.Parse(splitted[1].Replace('.', ',')) * length,
                        Single.Parse(splitted[2].Replace('.', ',')) * length,
                        Single.Parse(splitted[3].Replace('.', ',')) * length);
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
                        Vector4 currentStation = new Vector4(Single.Parse(splitted[0].Replace('.', ',')) * length,
                        Single.Parse(splitted[1].Replace('.', ',')) * length,
                        Single.Parse(splitted[2].Replace('.', ',')) * length,
                        Single.Parse(splitted[3].Split('\t')[0].Replace('.', ',')));
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

    


    void ReadTriangulation_fmt()
    {
        string[] configData = File.ReadAllLines(triangulation);
        pointCount = int.Parse(Helper.SplitWhitespace(configData[0])[1]);
        vertCount  = int.Parse(Helper.SplitWhitespace(configData[0])[2]);
        points = new Structs.Point[pointCount + 1];
        vertices = new int[3 * (vertCount + 1)];
        int tmp = vertCount / 2 + 1;
        int a = 0;

        for (int i = tmp; i <= tmp + (pointCount / 2) - 1; i++)
        {
            string[] splitted = Helper.SplitWhitespace(configData[i]);
            points[a] = new Structs.Point(Single.Parse(splitted[1].Replace('.', ',')), Single.Parse(splitted[2].Replace('.', ',')));
            points[a+1] = new Structs.Point(Single.Parse(splitted[3].Replace('.', ',')), Single.Parse(splitted[4].Replace('.', ',')));
            a = a + 2;
        }
        a = 0;
        for (int i = 1; i < tmp; i++)
        {
            string[] splitted = Helper.SplitWhitespace(configData[i]);
            vertices[a]   = int.Parse(splitted[1]) - 1;
            vertices[a+1] = int.Parse(splitted[2]) - 1;
            vertices[a+2] = int.Parse(splitted[3]) - 1;
            vertices[a+3] = int.Parse(splitted[4]) - 1;
            vertices[a+4] = int.Parse(splitted[5]) - 1;
            vertices[a+5] = int.Parse(splitted[6]) - 1;
            a = a + 6;
        }
    }

    void ReadTriangulation_msh()
    {
        string[] configData = File.ReadAllLines(triangulation);
        string mode = "";
        string[] splitted;
        int v = 0;
        foreach (string line in configData)
        {
             
            splitted = Helper.SplitWhitespace(line);
            if (line.Contains("Nodes"))
            {
                mode = "nodes";
                continue;
            }
            else if (line.Contains("Elements"))
            {
                mode = "elements";
                continue;
            }

            if (mode == "nodes" )
            {
                if (splitted.Length < 3 )
                {
                    pointCount = int.Parse(splitted[0]);
                    points = new Structs.Point[pointCount + 1];
                }
                else
                {
                    Structs.Point p = new Structs.Point(Single.Parse(splitted[1].Replace('.', ',')), Single.Parse(splitted[2].Replace('.', ',')));
                    points[int.Parse(splitted[0]) - 1] = p;
                    Debug.Log(p);
                }
            }
            else if (mode == "elements")
            {
                if (splitted.Length < 3)
                {
                    vertCount = int.Parse(splitted[0]);
                    vertices = new int[3 * (vertCount)];
                    Debug.Log(vertCount);
                }
                else
                {
                    vertices[v] = int.Parse(splitted[5]) - 1 ;
                    vertices[v + 1] = int.Parse(splitted[6]) - 1;
                    vertices[v + 2] = int.Parse(splitted[7]) - 1;
                    v = v + 3;
                }
            }
        }
    }
}
