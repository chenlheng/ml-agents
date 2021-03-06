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

        ps.agentScript.AddReward(striker);
        ps.agentScript.Done();  //all agents need to be reset
    }


    public Vector3 GetRandomSpawnPos(AgentSoccer.AgentRole role, AgentSoccer.Team team, int posOrder)
    {
        // {-9f~13f, 0f, -6~6f}
        float xOffset = 5f;
        Vector3 pos = new Vector3(xOffset, 0f, 0f);
        int tier_interval = 5;
        int flag = 1;
        if (team == AgentSoccer.Team.Blue){
            flag = -1;
        }
        switch (posOrder){
            case 0:
            pos += new Vector3(0f * tier_interval,   0f,     1f * tier_interval);
            break;
            case 1:
            pos += new Vector3(0f * tier_interval,   0f,     -1f * tier_interval);
            break;
        }
        
        pos = pos * flag;
        var randomSpawnPos = ground.transform.position +
//         new Vector3(Random.Range(-9f, 13f) , 0f, Random.Range(-6f, 6f)); //random
        pos + (Random.insideUnitSphere * 1); // fixed
        randomSpawnPos.y = ground.transform.position.y + 2;
        return randomSpawnPos;
    }

    public Vector3 GetBallSpawnPosition()
    {
        var randomSpawnPos = ground.transform.position +
//         new Vector3(Random.Range(-9f, 9f), 0f, Random.Range(-6f, 6f));  // random pos
        new Vector3(0f, 0f, 0f) + (Random.insideUnitSphere * 1); // fixed pos for view
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
