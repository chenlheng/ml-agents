using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SoccerAcademy : Academy
{

    public Brain brainStriker;
    public Material redMaterial;
    public Material blueMaterial;
    public float spawnAreaMarginMultiplier;
    public float gravityMultiplier = 1;
    public bool randomizePlayersTeamForTraining = true;
    public int maxAgentSteps;

    public float agentRunSpeed;
    public float agentRotationSpeed;

    public float strikerPunish; //if opponents scores, the striker gets this neg reward (-1)
    public float strikerReward; //if team scores a goal they get a reward (+1)

    void Start()
    {
        Physics.gravity *= gravityMultiplier; //for soccer a multiplier of 3 looks good
    }
    public override void AcademyReset()
    {

    }

    public override void AcademyStep()
    {

    }

}
