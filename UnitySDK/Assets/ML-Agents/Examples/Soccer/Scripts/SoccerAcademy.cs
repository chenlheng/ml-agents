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
            foreach(GameObject redStriker in GameObject.FindGameObjectsWithTag("redAgent"))
            {
                Physics.IgnoreCollision(redStriker.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>());
            }
            foreach(GameObject blueStriker in GameObject.FindGameObjectsWithTag("blueAgent"))
            {
                Physics.IgnoreCollision(blueStriker.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>(), false);
            }
        }
        foreach (GameObject blueBackLineWall in GameObject.FindGameObjectsWithTag("blueBackLineWall"))
        {
            Physics.IgnoreCollision(ball.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
            foreach(GameObject blueStriker in GameObject.FindGameObjectsWithTag("blueAgent"))
            {
                Physics.IgnoreCollision(blueStriker.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
            }
            foreach(GameObject redStriker in GameObject.FindGameObjectsWithTag("redAgent"))
            {
                Physics.IgnoreCollision(redStriker.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>(), false);
            }
        }
    }
    public override void AcademyReset()
    {
        foreach (GameObject redBackLineWall in GameObject.FindGameObjectsWithTag("redBackLineWall"))
        {
//            Physics.IgnoreCollision(ball.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>());
            foreach(GameObject redStriker in GameObject.FindGameObjectsWithTag("redAgent"))
            {
                Physics.IgnoreCollision(redStriker.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>());
            }
            foreach(GameObject blueStriker in GameObject.FindGameObjectsWithTag("blueAgent"))
            {
                Physics.IgnoreCollision(blueStriker.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>(), false);
            }
        }
        foreach (GameObject blueBackLineWall in GameObject.FindGameObjectsWithTag("blueBackLineWall"))
        {
//            Physics.IgnoreCollision(ball.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
            foreach(GameObject blueStriker in GameObject.FindGameObjectsWithTag("blueAgent"))
            {
                Physics.IgnoreCollision(blueStriker.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
            }
            foreach(GameObject redStriker in GameObject.FindGameObjectsWithTag("redAgent"))
            {
                Physics.IgnoreCollision(redStriker.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>(), false);
            }
        }
    }

    public override void AcademyStep()
    {
        foreach (GameObject redBackLineWall in GameObject.FindGameObjectsWithTag("redBackLineWall"))
        {
//            Physics.IgnoreCollision(ball.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>());
            foreach(GameObject redStriker in GameObject.FindGameObjectsWithTag("redAgent"))
            {
                Physics.IgnoreCollision(redStriker.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>());
            }
            foreach(GameObject blueStriker in GameObject.FindGameObjectsWithTag("blueAgent"))
            {
                Physics.IgnoreCollision(blueStriker.GetComponent<Collider>(), redBackLineWall.GetComponent<Collider>(), false);
            }
        }
        foreach (GameObject blueBackLineWall in GameObject.FindGameObjectsWithTag("blueBackLineWall"))
        {
//            Physics.IgnoreCollision(ball.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
            foreach(GameObject blueStriker in GameObject.FindGameObjectsWithTag("blueAgent"))
            {
                Physics.IgnoreCollision(blueStriker.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>());
            }
            foreach(GameObject redStriker in GameObject.FindGameObjectsWithTag("redAgent"))
            {
                Physics.IgnoreCollision(redStriker.GetComponent<Collider>(), blueBackLineWall.GetComponent<Collider>(), false);
            }
        }
    }

}
