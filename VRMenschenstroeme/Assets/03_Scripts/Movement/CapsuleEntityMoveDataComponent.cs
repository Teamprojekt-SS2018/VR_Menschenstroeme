using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct CapsuleEntityMoveData : IComponentData { }

public class CapsuleEntityMoveDataComponent : ComponentDataWrapper<CapsuleEntityMoveData> { }
