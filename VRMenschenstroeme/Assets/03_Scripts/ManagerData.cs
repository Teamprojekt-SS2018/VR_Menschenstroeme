using System;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;
using System.Linq;
using Unity.Rendering;

public class ManagerData : MonoSingleton<ManagerData>
{
    [HideInInspector]
    public NativeArray<byte> active;
    [HideInInspector]
    public NativeArray<float3> position;
    [HideInInspector]
    public NativeArray<float> density;
    [HideInInspector]
    public NativeArray<float3> color;
    [HideInInspector]
    public int stepCount;
    [HideInInspector]
    public int startIndex;
    [HideInInspector]
    public int endIndex;
    [HideInInspector]
    public float positionProbability;

    [Header("Files")]
    public string movementData = "VRData.dat";
    public string configData = "ConfigMuenster.dat";

    [Header("Capsule settings")]
    public bool entityJob;
    public GameObject capsule;
    public GameObject entityCapsule;
    public Material capsuleMaterial;
    public Color highDensity;
    public Color lowDensity;
    public int colorCount;

    [Header("Read configuration")]
    [SerializeField]
    private ReadConfig readConfig;
    [SerializeField]
    private ReadMovement readMovement;

    [Header("Creator settings")]
    [SerializeField]
    public Material floorMaterial;
    public Material fenceMaterial;

    [Header("Movement")]
    public float time = 0;
    public float timeScale = 1;
    public float timeStep = 1;

    [Header("Gamestate")]
    public GameState currentGameState;

    [Header("Map")]
    public GameObject map;
    public GameObject tableplate;
    public String saveGameName;

    public ReadConfig ReadConfig
    {
        get
        {
            return this.readConfig;
        }
        set
        {
            this.readConfig = value;
            this.init();
        }
    }

    public ReadMovement ReadMovement
    {
        get
        {
            return this.readMovement;
        }
        set
        {
            this.readMovement = value;
            this.init();
        }
    }

    private List<Action> triggers = new List<Action>();

    private bool checkInit
    {
        get
        {
            return readConfig != null && readMovement != null;
        }
    }

    private void init()
    {
        if (this.checkInit)
        {
            this.triggers.ForEach(a => a.Invoke());
        }
    }

    public void AddTrigger(Action trigger)
    {
        if (this.checkInit)
        {
            trigger.Invoke();
            this.triggers.Add(trigger);
        } else
        {
            this.triggers.Add(trigger);
        }
    }

    public void ChangeState(GameState gS)
    {
        currentGameState = gS;
        switch(currentGameState)
        {
            case GameState.Play:
                this.timeScale = 1;
                break;
            case GameState.Pause:
                this.timeScale = 0;
                break;
            case GameState.Forward:
                this.timeScale = 3;
                break;
            case GameState.Revert:
                this.timeScale = -1;
                break;
            case GameState.Reset:
                this.timeScale = 0;
                this.time = 0;
                break;
        }
    }

    private void StartJob()
    {
        this.timeStep = this.readConfig.Time;
        this.stepCount = readMovement.persons[readMovement.persons.Keys.GetEnumerator().Current].Count;
        this.active = new NativeArray<Byte>(readMovement.persons.Count * this.stepCount, Allocator.Persistent);
        this.position = new NativeArray<float3>(readMovement.persons.Count * this.stepCount, Allocator.Persistent);
        this.density = new NativeArray<float>(readMovement.persons.Count * this.stepCount, Allocator.Persistent);


        foreach (KeyValuePair<int, List<Structs.PersonPosition>> element in this.readMovement.persons.OrderBy(el => el.Key))
        {
            if (!this.entityJob)
            {
                GameObject instance = Instantiate(this.capsule, new Vector3(0f, 0f, 0f), this.transform.rotation);
                instance.GetComponent<MoveScriptJob>().id = element.Key;
                instance.transform.parent = this.map.transform;
                instance.transform.localScale = new Vector3(0.5f, 0.9f, 0.5f);
            }
            foreach (var item in element.Value.Select((value, i) => new { i, value }))
            {
                this.active[element.Key * this.stepCount + item.i] = (byte)(item.value.activated ? 1 : 0);
                this.position[element.Key * this.stepCount + item.i] = new float3(item.value.position) * this.readConfig.Length + new float3(0, 1, 0);
                this.density[element.Key * this.stepCount + item.i] = item.value.density;
            }
        }

        if (this.entityJob)
        {
            // Create capsules
            EntityManager entityManager = World.Active.GetOrCreateManager<EntityManager>();
            NativeArray<Entity> persons = new NativeArray<Entity>(readMovement.persons.Count, Allocator.Temp);
            entityManager.Instantiate(this.entityCapsule, persons);
            persons.Dispose();
        }
        else
        {

            this.color = new NativeArray<float3>(this.colorCount, Allocator.Persistent);
            for (int i = 0; i < colorCount; i++)
            {
                Color c = Color.Lerp(this.lowDensity, this.highDensity, (float)i / colorCount);
                this.color[i] = new float3(c.r, c.g, c.b);
            }
        }
    }

    void Start()
    {
        this.AddTrigger(StartJob);
    }

    void Update()
    {
        this.time += UnityEngine.Time.deltaTime * this.timeScale;
        if (this.time < 0)
        {
            this.startIndex = 0;
            this.endIndex = 0;
            this.positionProbability = 0;
        } else
        {
            this.startIndex = (int)(this.time / this.timeStep);
            this.startIndex = startIndex >= this.stepCount - 1 ? this.stepCount - 1 : startIndex;
            this.endIndex = startIndex == this.stepCount - 1 ? startIndex : startIndex + 1;
            this.positionProbability = (this.time % this.timeStep) / this.timeStep;
        }
    }

    void OnDestroy()
    {
        if (this.color.IsCreated)
            this.color.Dispose();
        this.active.Dispose();
        this.position.Dispose();
        this.density.Dispose();
    }
}
