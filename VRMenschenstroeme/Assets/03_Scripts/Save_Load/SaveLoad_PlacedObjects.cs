using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;



[Serializable]
public class SaveData {
    public string PrefabName;
    public string Id;
    public Vector3 Position;
    public Vector3 Scale;
    public Quaternion Rotation;
    public int Layer;

    public override string ToString() {
        return PrefabName + "_" + Id + "_Position: (" + Position.x + ", " + Position.y + ", " + Position.z + ")" + "_Scale: (" + Scale.x + ", " + Scale.y + ", " + Scale.z + ")" + "Layer: " + Layer;
    }
}

public static class JsonHelper {
    public static T[] FromJson<T>(string json) {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array) {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint) {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T> {
        public T[] Items;
    }
}


public class SaveLoad_PlacedObjects : MonoBehaviour {
    // Declare and initialize a new List of GameObjects called currentCollisions.
    List<GameObject> objectsOnTable = new List<GameObject>();
    public string SaveGameName = "SimulationSave";
    public GameObject Map;
    private string SavePath = "";
    private GameObject[] prefabs;

    // Use this for initialization
    void Start() {
        prefabs = Resources.LoadAll<GameObject>("0_SimulationObjects\\");
        SavePath = Application.dataPath + "/05_SaveData/";
    }

    void OnCollisionEnter(Collision col) {
        // Add the GameObject collided with to the list.
        objectsOnTable.Add(col.gameObject);
    }

    void OnCollisionExit(Collision col) {
        // Remove the GameObject collided with from the list.
        objectsOnTable.Remove(col.gameObject);
    }

    public void save() {
        Debug.Log(SavePath);


        SaveData[] saveDataInstance = new SaveData[objectsOnTable.Count];

        for (int i = 0; i < objectsOnTable.Count; i++) {
            objectsOnTable[i].transform.SetParent(Map.transform);
            string _name = objectsOnTable[i].name;
            saveDataInstance[i] = new SaveData();
            saveDataInstance[i].PrefabName = _name.Substring(0, _name.IndexOf("(Clone)"));
            saveDataInstance[i].Id = _name.Substring(_name.IndexOf("(Clone)_") + 8);
            saveDataInstance[i].Scale = objectsOnTable[i].transform.localScale;
            saveDataInstance[i].Position = objectsOnTable[i].transform.localPosition;
            saveDataInstance[i].Rotation = objectsOnTable[i].transform.localRotation;
            saveDataInstance[i].Rotation = objectsOnTable[i].transform.localRotation;
            saveDataInstance[i].Layer = objectsOnTable[i].layer;
        }

        //Convert to Jason
        string playerToJason = JsonHelper.ToJson(saveDataInstance, true);
        Debug.Log(playerToJason);

        File.WriteAllText(SavePath + SaveGameName + ".json", playerToJason);

    }

    public void load() {
        string jsonString = File.ReadAllText(SavePath + SaveGameName + ".json");
        for (int i = 0; i < objectsOnTable.Count; i++) {
            Destroy(objectsOnTable[i]);
        }
        objectsOnTable = new List<GameObject>();


        if (jsonString.Length > 1) {
            SaveData[] loadedSaveData = JsonHelper.FromJson<SaveData>(jsonString);
            foreach (SaveData item in loadedSaveData) {

                GameObject prefab = prefabs.First(x => x.name == item.PrefabName);

                GameObject newObject = Instantiate(prefab, item.Position, item.Rotation);

                newObject.AddComponent<Rigidbody>();
                newObject.AddComponent<MeshCollider>();
                newObject.GetComponent<MeshCollider>().convex = true;
                newObject.GetComponent<Rigidbody>().isKinematic = false;
                newObject.transform.SetParent(Map.transform);
                newObject.AddComponent<VRInteractableObject>();
                newObject.GetComponent<VRInteractableObject>().clonedObject = true;
                newObject.tag = "SimulationObject";
                newObject.layer = item.Layer;

                newObject.transform.localScale = item.Scale;
                newObject.transform.localRotation = item.Rotation;
                newObject.transform.localPosition = item.Position;
                newObject.name = item.PrefabName + "(Clone)_" + item.Id;
                objectsOnTable.Add(newObject);
            }
        }
    }
}

