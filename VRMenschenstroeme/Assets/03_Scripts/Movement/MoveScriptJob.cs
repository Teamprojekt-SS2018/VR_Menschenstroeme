using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public class MoveScriptJob : MonoBehaviour {
    public int id;
    public Material material;
    public Color color;
    void Start()
    {
        this.gameObject.AddComponent<MeshRenderer>().material = ManagerData.Instance.capsuleMaterial;
        this.material = this.gameObject.GetComponent<MeshRenderer>().material;
        this.color = this.material.color;
    }
}

class MoveSystem : ComponentSystem
{
    struct Components
    {
        public MoveScriptJob moveScriptJob;
        public Transform transform;
    }

    protected override void OnUpdate()
    {
        float time = ManagerData.Instance.time;
        int startIndex = ManagerData.Instance.startIndex;
        int endIndex = ManagerData.Instance.endIndex;
        float positionProbability = ManagerData.Instance.positionProbability;
        NativeArray<byte> active = ManagerData.Instance.active;
        NativeArray<float3> position = ManagerData.Instance.position;
        NativeArray<float> density = ManagerData.Instance.density;
        int stepCount = ManagerData.Instance.stepCount;
        Color highDensity = ManagerData.Instance.highDensity;
        Color lowDensity = ManagerData.Instance.lowDensity;
        NativeArray<float3> color = ManagerData.Instance.color;
        int colorCount = ManagerData.Instance.colorCount;

        foreach (var e in GetEntities<Components>())
        {
            if (active[e.moveScriptJob.id * stepCount + endIndex] > 0)
            {
                float3 new_pos = (position[e.moveScriptJob.id * stepCount + endIndex] - position[e.moveScriptJob.id * stepCount + startIndex]) * positionProbability + position[e.moveScriptJob.id * stepCount + startIndex];
                e.transform.localPosition = new Vector3(new_pos.x, new_pos.y, new_pos.z);
                float3 cur_color = color[(int)(density[e.moveScriptJob.id * stepCount + startIndex] * (colorCount - 1))];
                e.moveScriptJob.color.r = cur_color.x;
                e.moveScriptJob.color.g = cur_color.y;
                e.moveScriptJob.color.b = cur_color.z;
                e.moveScriptJob.material.color = e.moveScriptJob.color;
            } else
            {
                e.transform.position = new Vector3(9000, 9000, 9000);
            }
        }
    }
}
