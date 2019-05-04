using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerState
{
    public int playerIndex; 
    public Rigidbody agentRB; 
    public Vector3 startingPos; 
    public AgentSoccer agentScript; 
    public float ballPosReward;
}

public class SoccerFieldArea : MonoBehaviour
{
    public Transform redGoal;
    public Transform blueGoal;
    public AgentSoccer redStriker;
    public AgentSoccer blueStriker;
    public GameObject ball;
    [HideInInspector]
    public Rigidbody ballRB;
    public GameObject ground; 
    public GameObject centerPitch;
    SoccerBallController ballController;
    public List<PlayerState> playerStates = new List<PlayerState>();
    [HideInInspector]
    public Vector3 ballStartingPos;
    public bool drawSpawnAreaGizmo;
    Vector3 spawnAreaSize;
    public float goalScoreByTeamReward;
    public float goalScoreAgainstTeamReward;
    public GameObject goalTextUI;
    public float totalPlayers;
    [HideInInspector]
    public bool canResetBall;
    public bool useSpawnPoint;
    public Transform spawnPoint;
    Material groundMaterial;
    Renderer groundRenderer;
    SoccerAcademy academy;
    public float blueBallPosReward;
    public float redBallPosReward;
    public int playerNumber;

    public IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        groundRenderer.material = mat;
        yield return new WaitForSeconds(time); 
        groundRenderer.material = groundMaterial;
    }


    void Awake()
    {
        playerNumber = GameObject.FindGameObjectsWithTag("redAgent").Length + GameObject.FindGameObjectsWithTag("blueAgent").Length;
        academy = FindObjectOfType<SoccerAcademy>();
        groundRenderer = centerPitch.GetComponent<Renderer>(); 
        groundMaterial = groundRenderer.material;
        canResetBall = true;
        if (goalTextUI) { goalTextUI.SetActive(false); }
        ballRB = ball.GetComponent<Rigidbody>();
        ballController = ball.GetComponent<SoccerBallController>();
        ballController.area = this;
        ballStartingPos = ball.transform.position;
        Mesh mesh = ground.GetComponent<MeshFilter>().mesh;
    }

    IEnumerator ShowGoalUI()
    {
        if (goalTextUI) goalTextUI.SetActive(true);
        yield return new WaitForSeconds(.25f);
        if (goalTextUI) goalTextUI.SetActive(false);
    }

    public void AllPlayersDone(float reward)
    {
        foreach (PlayerState ps in playerStates)
        {
            if (ps.agentScript.gameObject.activeInHierarchy)
            {
                if (reward != 0)
                {
                    ps.agentScript.AddReward(reward);
                }
                ps.agentScript.Done();
            }

        }
    }

    public void GoalTouched(AgentSoccer.Team scoredTeam)
    {
        foreach (PlayerState ps in playerStates)
        {
            if (ps.agentScript.team == scoredTeam)
            {
                RewardOrPunishPlayer(ps, academy.strikerReward);
            }
            else
            {
                RewardOrPunishPlayer(ps, academy.strikerPunish);
            }
            if (academy.randomizePlayersTeamForTraining)
            {
                ps.agentScript.ChooseRandomTeam();
            }

            if (scoredTeam == AgentSoccer.Team.Red)
            {
                StartCoroutine(GoalScoredSwapGroundMaterial(academy.redMaterial, 1));
            }
            else
            {
                StartCoroutine(GoalScoredSwapGroundMaterial(academy.blueMaterial, 1));
            }
            if (goalTextUI)
            {
                StartCoroutine(ShowGoalUI());
            }
        }
    }

    public void RewardOrPunishPlayer(PlayerState ps, float striker)
    {
        if (ps.agentScript.agentRole == AgentSoccer.AgentRole.Striker)
        {
            ps.agentScript.AddReward(striker);
        }
        ps.agentScript.Done();  //all agents need to be reset
    }


    public Vector3 GetRandomSpawnPos(AgentSoccer.AgentRole role, AgentSoccer.Team team, int posOrder)
    {
        float xOffset = 15f;

//        if (role == AgentSoccer.AgentRole.Striker)
//        {
//            xOffset = 15f;
//        }
//        if (team == AgentSoccer.Team.Blue)
//        {
//            xOffset = xOffset * -1f;
//        }
//
        Vector3 pos = new Vector3(xOffset, 0f, 0f);
        int dist = 7;
        switch (posOrder){
            case 0:
                pos += new Vector3(-1f * dist,  0f,     1f * dist);
                break;
            case 1:
                pos += new Vector3(-1f * dist,  0f,     -1f * dist);
                break;
            case 2:
                pos += new Vector3(0f * dist,   0f,     1f * dist);
                break;
            case 3:
                pos += new Vector3(0f * dist,   0f,     -1f * dist);
                break;
            case 4:
                pos += new Vector3(1f * dist,   0f,     0f * dist);
                break;
        }
        
        if (team == AgentSoccer.Team.Blue){
            pos = pos * -1f;
        }

        var randomSpawnPos = ground.transform.position + pos
                               + (Random.insideUnitSphere * 1);
        randomSpawnPos.y = ground.transform.position.y + 2;
        return randomSpawnPos;
    }
    
    public Vector3 GetBallSpawnPosition()
    {
        var randomSpawnPos = ground.transform.position + 
                                new Vector3(0f, 0f, 0f)
                              + (Random.insideUnitSphere * 1);
        randomSpawnPos.y = ground.transform.position.y + 2;
        return randomSpawnPos;
    }

    void SpawnObjAtPos(GameObject obj, Vector3 pos)
    {
        obj.transform.position = pos;
    }

    public void ResetBall()
    {
        ball.transform.position = GetBallSpawnPosition();
        ballRB.velocity = Vector3.zero;
        ballRB.angularVelocity = Vector3.zero;
    }
}
