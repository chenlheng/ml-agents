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
    float kickPower, timePenalty=-1f/3000f;
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
        float rayDistance = 40f;
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };
        string[] detectableObjects;

        int teamId = 0;
        Transform teamGoal, opponentGoal;

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
    }

    public void MoveAgent(float[] act)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        int action = Mathf.FloorToInt(act[0]);

        // Goalies and Strikers have slightly different action spaces.
        if (agentRole == AgentRole.Striker)
        {
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
        }
        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        agentRb.AddForce(dirToGo * academy.agentRunSpeed,
                         ForceMode.VelocityChange);

    }


    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Existential penalty for strikers.
        if (agentRole == AgentRole.Striker)
        {
            // AddReward(-1f / 3000f);
            AddReward(timePenalty);
        }
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
        }
    }

    public override void AgentReset()
    {
        if (academy.randomizePlayersTeamForTraining)
        {
            ChooseRandomTeam();
        }

        if (team == Team.Red)
        {
            JoinRedTeam(agentRole);
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else
        {
            JoinBlueTeam(agentRole);
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        transform.position = area.GetRandomSpawnPos(agentRole, team);
        agentRb.velocity = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;
        area.ResetBall();
    }
}
