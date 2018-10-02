using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;

public class MoveJobSystem : JobComponentSystem {
    [BurstCompile]
    struct MoveJob : IJobParallelFor {
        public CapsuleData data;
        public readonly NativeArray<byte> active;
        public readonly NativeArray<float3> position;
        public readonly int stepCount;
        public readonly int startIndex;
        public readonly int endIndex;
        public readonly float positionProbability;
        public readonly float3 positionOffset;
        public readonly float scale;
        public void Execute(int index) {
            if (active[index * stepCount + endIndex] > 0) {
                TransformMatrix matrix = data.transform[index];
                float3 pos = (position[index * stepCount + endIndex] - position[index * stepCount + startIndex]) * positionProbability + position[index * stepCount + startIndex];
                matrix.Value = new float4x4(
                    scale * 0.5f, 0, 0, pos.x * scale + positionOffset.x,
                    0, scale * 0.9f, 0, pos.y * scale + positionOffset.y,
                    0, 0, scale * 0.5f, pos.z * scale + positionOffset.z,
                    0, 0, 0, 1
                );
                data.transform[index] = matrix;
            } else {
                TransformMatrix matrix = data.transform[index];
                matrix.Value = new float4x4(
                    1, 0, 0, 9000,
                    0, 1, 0, 9000,
                    0, 0, 1, 9000,
                    0, 0, 0, 1
                );
                data.transform[index] = matrix;
            }
        }

        public MoveJob(CapsuleData data, NativeArray<byte> active, NativeArray<float3> position, int stepCount,
           int startIndex, int endIndex, float positionProbability, float3 positionOffset, float scale) {
            this.data = data;
            this.active = active;
            this.position = position;
            this.stepCount = stepCount;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.positionProbability = positionProbability;
            this.positionOffset = positionOffset;
            this.scale = scale;
        }
    }

    struct CapsuleData {
        public readonly int Length;
        public ComponentDataArray<TransformMatrix> transform;
        [ReadOnly] public ComponentDataArray<Rotation> rotation;
        [ReadOnly] public ComponentDataArray<CapsuleEntityMoveData> capsuleData;
    }

    [Inject] private CapsuleData capsuleData;

    protected override JobHandle OnUpdate(JobHandle inputDeps) {

        float time = ManagerData.Instance.time;
        int startIndex = ManagerData.Instance.startIndex;
        int endIndex = ManagerData.Instance.endIndex;
        float positionProbability = ManagerData.Instance.positionProbability;
        NativeArray<byte> active = ManagerData.Instance.active;
        NativeArray<float3> position = ManagerData.Instance.position;
        int stepCount = ManagerData.Instance.stepCount;
        float3 positionOffset = new float3(
            ManagerData.Instance.map.transform.position.x,
            ManagerData.Instance.map.transform.position.y,
            ManagerData.Instance.map.transform.position.z
        );
        float scale = ManagerData.Instance.map.transform.lossyScale.x;

        MoveJob job = new MoveJob(this.capsuleData, active, position, stepCount, startIndex, endIndex, positionProbability, positionOffset, scale);
        return job.Schedule(this.capsuleData.Length, 65000, inputDeps);
    }
}
