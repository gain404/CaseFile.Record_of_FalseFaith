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

    private SpriteRenderer _bossSrpite;

    protected override Status OnStart()
    {
        GameObject bossGameObject = Self.Value;
        GameObject playerGameObject = Target.Value;
        
        Transform bossTransform = bossGameObject.transform;
        Transform playerTransform = playerGameObject.transform;

        if (_bossSrpite == null)
        {
            _bossSrpite = bossGameObject.GetComponent<SpriteRenderer>();
        }

        float playerX = playerTransform.position.x;
        float bossX = bossTransform.position.x;

        if (playerX < bossX)
        {
            //여기에 flip추가
        }
        else
        {
            
        }

        return Status.Success;
    }
}

