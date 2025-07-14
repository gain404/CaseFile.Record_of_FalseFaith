using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FacePlayer", story: "[Self] Face to [Target]", category: "Action", id: "56e81ce5fba998578c37b3d84e009a10")]
public partial class FacePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        Transform playerTransform = Target.Value.transform;
        Transform bossTransform = Self.Value.transform;

        float bossScale = Math.Abs(bossTransform.localScale.x);
        
        if (playerTransform.position.x < bossTransform.position.x)
        {
            bossTransform.localScale = new Vector3(bossScale, bossTransform.localScale.y, bossTransform.localScale.z);
        }
        else
        {
            bossTransform.localScale = new Vector3(-bossScale, bossTransform.localScale.y, bossTransform.localScale.z);
        }
        return Status.Success;
    }
}

