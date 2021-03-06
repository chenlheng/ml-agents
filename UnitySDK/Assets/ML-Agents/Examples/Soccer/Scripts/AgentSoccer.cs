using UnityEngine;
using MLAgents;

public class AgentSoccer : Agent
{

    public enum Team
    {
        Red, 
        Blue
    }
    public enum AgentRole
    {
        Striker
    }
    
    public Team team;
    public AgentRole agentRole;
    float kickPower;
    int playerIndex;
    public SoccerFieldArea area;
    
    [HideInInspector]
    public Rigidbody agentRb;
    SoccerAcademy academy;
    Renderer agentRenderer;
    RayPerception rayPer;

    public void ChooseRandomTeam()
    {
        team = (Team)Random.Range(0, 2);
        if (team == Team.Red)
        {
            JoinRedTeam(agentRole);
        }
        else
        {
            JoinBlueTeam(agentRole);
        }
    }

    public void JoinRedTeam(AgentRole role)
    {
        agentRole = role;
        team = Team.Red;
        agentRenderer.material = academy.redMaterial;
        tag = "redAgent";
    }

    public void JoinBlueTeam(AgentRole role)
    {
        agentRole = role;
        team = Team.Blue;
        agentRenderer.material = academy.blueMaterial;
        tag = "blueAgent";
    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        agentRenderer = GetComponent<Renderer>();
        rayPer = GetComponent<RayPerception>();
        academy = FindObjectOfType<SoccerAcademy>();
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;

        var playerState = new PlayerState
        {
            agentRB = agentRb, 
            startingPos = transform.position, 
            agentScript = this,
        };
        area.playerStates.Add(playerState);
        playerIndex = area.playerStates.IndexOf(playerState);
        playerState.playerIndex = playerIndex;
    }

    public override void CollectObservations()
    {
        // Ray-based observation
        float rayDistance = 100000f;
        //        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };
        //        float[] rayAngles = { 0f, 5f, 10f, 15f, 20f, 25f, 30f, 35f, 40f, 45f, 50f, 55f, 60f, 65f, 70f, 75f, 80f, 85f, 90f, 95f, 100f, 105f, 110f, 115f, 120f, 125f, 130f, 135f, 140f, 145f, 150f, 155f, 160f, 165f, 170f, 175f, 180f, 185f, 190f, 195f, 200f, 205f, 210f, 215f, 220f, 225f, 230f, 235f, 240f, 245f, 250f, 255f, 260f, 265f, 270f, 275f, 280f, 285f, 290f, 295f, 300f, 305f, 310f, 315f, 320f, 325f, 330f, 335f, 340f, 345f, 350f, 355f, 360f};
        float[] rayAngles = { 0f };
        string[] detectableObjects;
        
        int teamId = 0;
        Transform teamGoal, opponentGoal;
        Vector3 forward = transform.forward;
        forward.y = 0;
        float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;
        
        if (team == Team.Red)
        {
            detectableObjects = new[] { "ball", "redGoal", "blueGoal",
                "wall", "redAgent", "blueAgent" };
            teamId = 0;
            teamGoal = area.redGoal;
            opponentGoal = area.blueGoal;
        }
        else
        {
            detectableObjects = new[] { "ball", "blueGoal", "redGoal",
                "wall", "blueAgent", "redAgent" };
            teamId = 1;
            teamGoal = area.blueGoal;
            opponentGoal = area.redGoal;
        }
        
        AddVectorObs(teamId);
        AddVectorObs(playerIndex);
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        // explanation: https://github.com/Unity-Technologies/ml-agents/issues/1483
        // 6-dim onehot vec + [unknown_flag, distance]
        // AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 1f, 0f));
        AddVectorObs(transform.position.x);
        AddVectorObs(transform.position.y);
        AddVectorObs(transform.position.z);
        AddVectorObs(agentRb.velocity.x);
        AddVectorObs(agentRb.velocity.y);
        AddVectorObs(agentRb.velocity.z);
        AddVectorObs(agentRb.angularVelocity.x);
        AddVectorObs(agentRb.angularVelocity.y);
        AddVectorObs(agentRb.angularVelocity.z);
        AddVectorObs(area.ball.transform.position.x);
        AddVectorObs(area.ball.transform.position.y);
        AddVectorObs(area.ball.transform.position.z);
        AddVectorObs(area.ballRB.velocity.x);
        AddVectorObs(area.ballRB.velocity.y);
        AddVectorObs(area.ballRB.velocity.z);
        AddVectorObs(area.ballRB.angularVelocity.x);
        AddVectorObs(area.ballRB.angularVelocity.y);
        AddVectorObs(area.ballRB.angularVelocity.z);
        AddVectorObs(teamGoal.position.x);
        AddVectorObs(teamGoal.position.y);
        AddVectorObs(teamGoal.position.z);
        AddVectorObs(opponentGoal.position.x);
        AddVectorObs(opponentGoal.position.y);
        AddVectorObs(opponentGoal.position.z);
        AddVectorObs(headingAngle);
    }

    public void MoveAgent(float[] act)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        int action = Mathf.FloorToInt(act[0]);

        
        kickPower = 0f;
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                kickPower = 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
                break;
        }
        
        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        agentRb.AddForce(dirToGo * academy.agentRunSpeed,
                         ForceMode.VelocityChange);

    }


    public override void AgentAction(float[] vectorAction, string textAction)
    {

//        AddReward(-1f / 3000f);
        MoveAgent(vectorAction);

    }

    /// <summary>
    /// Used to provide a "kick" to the ball.
    /// </summary>
    void OnCollisionEnter(Collision c)
    {
        float force = 2000f * kickPower;
        if (c.gameObject.CompareTag("ball"))
        {
            Vector3 dir = c.contacts[0].point - transform.position;
            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
            AddReward(0.1f);
        }
    }

    public override void AgentReset()
    {
        int posOrder;
        //        if (team == Team.Red)
        if (playerIndex < area.playerNumber/2)
        {
            posOrder = playerIndex;
            JoinRedTeam(agentRole);
            Vector3 rotationVector = new Vector3(0f, -90f, 0f);
            //            Vector3 rotationVector = new Vector3(0f, Random.Range(-90f, 90f), 0f); # random facing
            transform.rotation = Quaternion.Euler(rotationVector);
            
        }
        else
        {
            posOrder = playerIndex - area.playerNumber/2;
            JoinBlueTeam(agentRole);
            Vector3 rotationVector = new Vector3(0f, 90f, 0f);
            //            Vector3 rotationVector = new Vector3(0f, Random.Range(-90f, 90f), 0f); # random facing
            transform.rotation = Quaternion.Euler(rotationVector);
            
        }
        transform.position = area.GetRandomSpawnPos(agentRole, team, posOrder);
        agentRb.velocity = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;
        area.ResetBall();
        academy.AcademyIgnoreCollision();
    }
}
