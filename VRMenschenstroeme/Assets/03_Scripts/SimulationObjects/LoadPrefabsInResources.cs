using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPrefabsInResources : MonoBehaviour {

    private GameObject[] prefabs;
    [Range(0.001f, 5f)]
    public float emptySize = 0.1f;
    List<GameObject> ShelfGameObjects = new List<GameObject>();
    private int currentStartIndexPrefabs = 0;
    private List<GameObject> currentObjOnShelf = new List<GameObject>();


    // Use this for initialization
    void Start() {
        prefabs = Resources.LoadAll<GameObject>("0_SimulationObjects\\");

        ShelfGameObjects.Add(this.gameObject.transform.GetChild(0).gameObject);
        ShelfGameObjects.Add(this.gameObject.transform.GetChild(1).gameObject);
        ShelfGameObjects.Add(this.gameObject.transform.GetChild(2).gameObject);
        ShelfGameObjects.Add(this.gameObject.transform.GetChild(3).gameObject);
        ShelfGameObjects.Add(this.gameObject.transform.GetChild(4).gameObject);
        ShelfGameObjects.Add(this.gameObject.transform.GetChild(5).gameObject);

        AddNextGameObjectsToShelf();
    }

    public void AddNextGameObjectsToShelf() {
        DeleteCurrentObjOnShelf();
        foreach (GameObject shelfGameObj in ShelfGameObjects) {
            if (prefabs != null && prefabs.Length > 0) {
                currentObjOnShelf.Add(CreateSimulationGameObject(shelfGameObj, prefabs[currentStartIndexPrefabs]));
                ++currentStartIndexPrefabs;
                if (currentStartIndexPrefabs >= prefabs.Length) {
                    currentStartIndexPrefabs = 0;
                }
            }
        }
    }

    public void AddPreviousGameObjectsToShelf() {
        DeleteCurrentObjOnShelf();
        foreach (GameObject shelfGameObj in ShelfGameObjects) {
            if (prefabs != null && prefabs.Length > 0) {
                currentObjOnShelf.Add(CreateSimulationGameObject(shelfGameObj, prefabs[currentStartIndexPrefabs]));
                --currentStartIndexPrefabs;
                if (currentStartIndexPrefabs < 0) {
                    currentStartIndexPrefabs = prefabs.Length - 1;
                }
            }
        }

    }

    private void DeleteCurrentObjOnShelf() {
        foreach (GameObject item in currentObjOnShelf) {
            if (item != null) { Destroy(item); }
        }
    }


    private GameObject CreateSimulationGameObject(GameObject shelfObject, GameObject prefab) {
        MeshFilter shelfObject_mf = shelfObject.GetComponent<MeshFilter>();
        if (shelfObject_mf != null) {
            GameObject newObject = Instantiate(prefab, shelfObject.transform.position, prefab.transform.rotation);

            if (newObject.GetComponent<Rigidbody>() == null) {
                newObject.AddComponent<Rigidbody>();
                newObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
            if (newObject.GetComponent<MeshCollider>() == null) {
                newObject.AddComponent<MeshCollider>();
            }

            newObject.GetComponent<MeshCollider>().convex = true;
            newObject.GetComponent<Rigidbody>().isKinematic = false;
            newObject.transform.SetParent(shelfObject.transform);
            newObject.AddComponent<VRInteractableObject>();
            newObject.tag = "SimulationObject";
            newObject.layer = 8;

            float scale = shelfObject_mf.mesh.bounds.extents.x / req(newObject.transform);
            newObject.transform.localScale = new Vector3(scale, scale, scale);


            return newObject;
        }
        return new GameObject();
    }


    private float req(Transform newObjectTransform) {
        float max = 0f;
        if (newObjectTransform.gameObject.GetComponent<MeshFilter>() != null) {
            Bounds bounds = newObjectTransform.gameObject.GetComponent<MeshFilter>().mesh.bounds;
            max = bounds.extents.x;
            if (max < bounds.extents.y)
                max = bounds.extents.y;
            if (max < bounds.extents.z)
                max = bounds.extents.z;
        }

        int children = newObjectTransform.childCount;
        for (int i = 0; i < children; ++i) {

        float temp = req(newObjectTransform.GetChild(i));
            max = max > temp ? max : temp;
        }

        return max;
    }
}
