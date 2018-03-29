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
    private int entranceCount;
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

    private List<Entrance> entrances = new List<Entrance>();
    private List<Exit> exits         = new List<Exit>();
    private List<Vector4> stations   = new List<Vector4>();

    /* Aus XXX.am_fmt */
    private int vertCount;
    private int pointCount;
    private Point[] points;
    private Triangle[] vertices;

    /* Structs */

    public struct Entrance
    {
        Point left;
        Point right;
        float passageSpeed;
        float passageCount;

        public Entrance(float leftX, float leftY, float rightX, float rightY, float passageSpeed, float passageCount)
        {
            this.left = new Point(leftX, leftY);
            this.right = new Point(rightX, rightY);
            this.passageSpeed = passageSpeed;
            this.passageCount = passageCount;
        }
            
        public Entrance(Point left, Point right, float passageSpeed, float passageCount)
        {
            this.left = left;
            this.right = right;
            this.passageSpeed = passageSpeed;
            this.passageCount = passageCount;
        }

        public override string ToString()
        {
            return "Eingang: " + left + " - " + right + " mit " + passageSpeed + " P/s und " + passageCount;
        }
    }

    public struct Exit
    {
        public Point left, right;

        public Exit(Point left, Point right)
        {
            this.left = left;
            this.right = right;
        }

        public Exit(float leftX, float leftY, float rightX, float rightY)
        {
            this.left = new Point(leftX, leftY);
            this.right = new Point(rightX, rightY);
        }

        public override string ToString()
        {
            return "Ausgang: " + left + " - " + right;
        }
    }

    public struct Point
    {
        public float x,y;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        public Vector3 ToVectorThree()
        {
            return new Vector3(x, 0, y);
        }
    }

    public struct Triangle
    {
        public Point first, second, third;

        public Triangle(Point first, Point second, Point third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public override string ToString()
        {
            return "Dreieck: " + first + " - " + second + " - " + third;
        }

        public float getArea()
        {
            // TODO
            return 0;
        }
    }

    /* Helper */
    string[] SplitWhitespace(string input)
    {
        string pattern = @"\t+| +";
        return System.Text.RegularExpressions.Regex.Split(input, pattern);
    }

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
                    triangulation = SplitWhitespace(configData[i + 1])[0];
                    //Debug.Log ("Triangulation: " + triangulation);
                    globalRefinement = float.Parse(SplitWhitespace(configData[i + 2])[0]);
                    //Debug.Log ("Global Refinement: " + globalRefinement);
                    localRefinement = float.Parse(SplitWhitespace(configData[i + 3])[0]);
                    //Debug.Log ("Local Refinement: " + localRefinement);
                    localRefinementMode = int.Parse(SplitWhitespace(configData[i + 4])[0]);
                    //Debug.Log ("Local Refinement Mode: " + localRefinementMode);


                    i = i + 4;
                }
                else if (line.Contains("Charakteristische Laengen"))
                {

                    length = float.Parse(SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("Length: " + length);
                    time = float.Parse(SplitWhitespace(configData[i + 2])[0]);
                    //Debug.Log ("Time: " + time);
                    dencity = float.Parse(SplitWhitespace(configData[i + 3])[0]);
                    //Debug.Log ("Dencity: " + dencity);
                    velocity = float.Parse(SplitWhitespace(configData[i + 4])[0]);
                    //Debug.Log ("Velocity: " + velocity);

                    i = i + 4;
                }
                else if (line.Contains("Anfangsbedingung"))
                {
                    personCount = int.Parse(SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("personCount: " + personCount);
                    runtime = int.Parse(SplitWhitespace(configData[i + 2])[0]);
                    //Debug.Log ("runtime: " + runtime);
                    i = i + 2;
                }
                else if (line.Contains("Eingang"))
                {
                    entranceCount = int.Parse(SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("Entrance Count: " + entranceCount);
                    for (int n = 1; n <= entranceCount; n++)
                    {
                        string[] splitted = SplitWhitespace(configData[i + n + 1]);

                        Entrance currentEntrance = new Entrance(float.Parse(splitted[0]),
                                                      float.Parse(splitted[1]),
                                                      float.Parse(splitted[2]),
                                                      float.Parse(splitted[3]),
                                                      float.Parse(splitted[4]),
                                                      float.Parse(splitted[5]));
                        entrances.Add(currentEntrance);
                        //Debug.Log ("Eingang: " + currentEntrance.ToString()); 
                    }
                    i = i + entranceCount;
                }
                else if (line.Contains("Ausgang"))
                {
                    exitCount = int.Parse(SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("Exit Count: " + exitCount);
                    for (int n = 1; n <= exitCount; n++)
                    {
                        string[] splitted = SplitWhitespace(configData[i + n + 1]);
                        Exit currentExit = new Exit(float.Parse(splitted[0]),
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
                    stationCount = int.Parse(SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("Station Count: " + stationCount);
                    for (int n = 1; n <= stationCount; n++)
                    {
                        string[] splitted = SplitWhitespace(configData[i + n + 1]);
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
                    trackingCount = int.Parse(SplitWhitespace(configData[i + 1])[0]);
                    //Debug.Log ("tracking Count: " + trackingCount);
                }
            }
        }
    }

    void ReadTriangulation()
    {
        string[] configData = File.ReadAllLines(triangulation);
        pointCount = int.Parse(SplitWhitespace(configData[0])[1]);
        vertCount  = int.Parse(SplitWhitespace(configData[0])[2]);
        points = new Point[pointCount+1];
        vertices = new Triangle[vertCount+1];
        int tmp = vertCount / 2 + 1;
        int a = 0;

        for (int i = tmp; i <= tmp + (pointCount / 2) - 1; i++)
        {
            string[] splitted = SplitWhitespace(configData[i]);
            //Debug.Log(i + " - " + configData[i]);
            points[a] = new Point(float.Parse(splitted[1]), float.Parse(splitted[2]));
            points[a+1] = new Point(float.Parse(splitted[3]), float.Parse(splitted[4]));
            //Debug.Log(points[a] + " & " + points[a + 1]);
            a = a + 2;
        }
        a = 0;
        for (int i = 1; i < tmp; i++)
        {
            string[] splitted = SplitWhitespace(configData[i]);
            vertices[a]   = new Triangle(points[int.Parse(splitted[1])],
                                         points[int.Parse(splitted[2])],
                                         points[int.Parse(splitted[3])]);
            vertices[a+1] = new Triangle(points[int.Parse(splitted[4])],
                                         points[int.Parse(splitted[5])],
                                         points[int.Parse(splitted[6])]);
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
