using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(ReadConfig))]
public class Creator : MonoBehaviour
{
    private ReadConfig _conf;
    private GameObject _fence;

    public Material floorMaterial;
    public Material fenceMaterial;
    [Range(0, 127)]
    public int scene = 1;
    public bool noFenceAndInitTeleport = false;
    
    // Use this for initialization
    void Start()
    {
        _conf = this.gameObject.GetComponent<ReadConfig>();
        this.createFloorMesh();
        if (noFenceAndInitTeleport)
        {
            if (this.gameObject.GetComponent<TeleportAreaMeshcreator>() == null)
            {
                this.gameObject.AddComponent<TeleportAreaMeshcreator>();
            }
            this.gameObject.GetComponent<TeleportAreaMeshcreator>().Init();
        }
        else
        {
            this.createFence();
        }
    }

    void createFloorMesh()
    {
        Mesh mesh = new Mesh();
        this.gameObject.AddComponent<MeshRenderer>().material = this.floorMaterial;
        this.gameObject.AddComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = _conf.Points.Select(p => p.ToVectorThree() * _conf.Length).ToArray<Vector3>();
        mesh.vertices = mesh.vertices.Take(mesh.vertices.Length - 1).ToArray();
        mesh.uv = _conf.Points.Select(p => p.ToVectorTwo()).ToArray<Vector2>().Take(mesh.vertices.Length).ToArray();
        mesh.triangles = _conf.Vertices.Take(_conf.Vertices.Length - 3).Reverse().ToArray<int>();
        mesh.RecalculateNormals();
        if (SceneManager.GetActiveScene().buildIndex == this.scene)
            this.gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
    }

    void createFence()
    {
        Mesh mesh = new Mesh();

        _fence = new GameObject("fence");
        _fence.transform.parent = this.gameObject.transform;
        _fence.AddComponent<MeshRenderer>().material = this.fenceMaterial;
        _fence.AddComponent<MeshFilter>().mesh = mesh;


        Dictionary<string, Vector2Int> fences = this.getFences(_conf.Vertices.Take(_conf.Vertices.Length - 3).ToArray<int>());
        fences = this.removeOuterFence(fences);

        Vector3[] fenceVertices = new Vector3[fences.Count * 4];
        Dictionary<int, int> mapping = new Dictionary<int, int>();
        int index = -1;
        foreach (KeyValuePair<string, Vector2Int> el in fences)
        {
            if (!mapping.ContainsKey(el.Value.x))
            {
                mapping.Add(el.Value.x, ++index);
                fenceVertices[index] = this._conf.Points[el.Value.x].ToVectorThree() * this._conf.Length;
                ++index;
                fenceVertices[index] = this._conf.Points[el.Value.x].ToVectorThree() * this._conf.Length + new Vector3(0, this._conf.Length / 10, 0);
            }
            if (!mapping.ContainsKey(el.Value.y))
            {
                mapping.Add(el.Value.y, ++index);
                fenceVertices[index] = this._conf.Points[el.Value.y].ToVectorThree() * this._conf.Length;
                ++index;
                fenceVertices[index] = this._conf.Points[el.Value.y].ToVectorThree() * this._conf.Length + new Vector3(0, this._conf.Length / 10, 0);
            }
        }

        mesh.vertices = fenceVertices;

        int[] triangles = new int[fences.Count * 6];

        index = -1;

        foreach (KeyValuePair<string, Vector2Int> el in fences)
        {
            triangles[++index] = mapping[el.Value.x];
            triangles[++index] = mapping[el.Value.x] + 1;
            triangles[++index] = mapping[el.Value.y];

            triangles[++index] = mapping[el.Value.x] + 1;
            triangles[++index] = mapping[el.Value.y] + 1;
            triangles[++index] = mapping[el.Value.y];
        }

        int[] bothsideTriangles = new int[triangles.Length * 2];

        triangles.Reverse().ToArray<int>().CopyTo(bothsideTriangles, 0);
        triangles.CopyTo(bothsideTriangles, triangles.Length);

        mesh.triangles = bothsideTriangles;

        _fence.transform.localScale = Vector3.one;
        _fence.transform.localPosition = Vector3.zero;
        _fence.transform.localRotation = Quaternion.identity;
    }



    private Dictionary<string, Vector2Int> getFences(int[] triangles)
    {
        Dictionary<string, Vector2Int> fences = new Dictionary<string, Vector2Int>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int t00 = Mathf.Min(triangles[i], triangles[i + 1]);
            int t01 = Mathf.Max(triangles[i], triangles[i + 1]);

            int x0 = Mathf.Min(triangles[i + 2], triangles[i + 1]);
            int x1 = Mathf.Max(triangles[i + 2], triangles[i + 1]);

            int y0 = Mathf.Min(triangles[i], triangles[i + 2]);
            int y1 = Mathf.Max(triangles[i], triangles[i + 2]);

            if (!fences.ContainsKey(t00 + " - " + t01))
            {
                fences.Add(t00 + " - " + t01,
                    new Vector2Int(triangles[i], triangles[i + 1]));
            }
            else
            {
                fences.Remove(t00 + " - " + t01);
            }

            if (!fences.ContainsKey(x0 + " - " + x1))
            {
                fences.Add(x0 + " - " + x1,
                    new Vector2Int(triangles[i + 1], triangles[i + 2]));
            }
            else
            {
                fences.Remove(x0 + " - " + x1);
            }

            if (!fences.ContainsKey(y0 + " - " + y1))
            {
                fences.Add(y0 + " - " + y1,
                    new Vector2Int(triangles[i + 2], triangles[i]));
            }
            else
            {
                fences.Remove(y0 + " - " + y1);
            }
        }
        return fences;
    }

    Dictionary<string, Vector2Int> removeOuterFence(Dictionary<string, Vector2Int> fences)
    {
        string key = "";
        float smallest = float.PositiveInfinity;

        // Get the smalles part of backside
        foreach (var keyValue in fences)
        {
            if (this._conf.Points[keyValue.Value.x].y < smallest)
            {
                key = keyValue.Key;
                smallest = this._conf.Points[keyValue.Value.x].y;
            }
            if (this._conf.Points[keyValue.Value.y].y < smallest)
            {
                key = keyValue.Key;
                smallest = this._conf.Points[keyValue.Value.x].y;
            }

        }

        // Trace and remove the outer fence starting by the smallest part
        while (fences.ContainsKey(key))
        {
            int next = fences[key].y;
            fences.Remove(key);
            foreach (var el in fences)
            {
                if (el.Value.x == next || el.Value.y == next)
                {
                    key = el.Key;
                    break;
                }
            }
        }

        return fences;
    }
}
