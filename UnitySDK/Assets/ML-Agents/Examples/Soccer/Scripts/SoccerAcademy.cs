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
    
    public GameObject ball;
    public AgentSoccer redStriker1;
    public AgentSoccer redStriker2;
    public AgentSoccer redStriker3;
    public AgentSoccer redStriker4;
    public AgentSoccer redStriker5;
    public AgentSoccer blueStriker1;
    public AgentSoccer blueStriker2;
    public AgentSoccer blueStriker3;
    public AgentSoccer blueStriker4;
    public AgentSoccer blueStriker5;
    
    public float agentRunSpeed;
    public float agentRotationSpeed;

    public float strikerPunish; //if opponents scores, the striker gets this neg reward (-1)
    public float strikerReward; //if team scores a goal they get a reward (+1)

    void Start()
    {
        Physics.gravity *= gravityMultiplier; //for soccer a multiplier of 3 looks good
        foreach (GameObject redBackLineWall in GameObject.FindGameObjectsWithTag("redBackLineWall"))
        {
            Physics.IgnoreCollision(ball.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>());
            Physics.IgnoreCollision(redStriker1.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>());
            Physics.IgnoreCollision(redStriker2.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>());
            Physics.IgnoreCollision(redStriker3.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>());
            Physics.IgnoreCollision(redStriker4.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>());
            Physics.IgnoreCollision(redStriker5.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>());
        }
        foreach (GameObject blueBackLineWall in GameObject.FindGameObjectsWithTag("blueBackLineWall"))
        {
            Physics.IgnoreCollision(ball.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
            Physics.IgnoreCollision(blueStriker1.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
            Physics.IgnoreCollision(blueStriker2.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
            Physics.IgnoreCollision(blueStriker3.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
            Physics.IgnoreCollision(blueStriker4.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
            Physics.IgnoreCollision(blueStriker5.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
        }
        
        
    }
    public override void AcademyReset()
    {

    }

    public override void AcademyStep()
    {

    }

}
