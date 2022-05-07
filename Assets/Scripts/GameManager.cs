using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private GameObject predictionVisualPrefab;
    [SerializeField]
    private LineRenderer visualizeRenderer;

    private GameObject ball;
    private GameObject[] players;
    private GameObject predictionVisual;
    private Vector3[] playerPrevPositions;
    private Vector2[] playerMovementVel;
    private Vector3 ballPrevPosition;
    private bool isFirstFrame;
    Vector3 ballMovementDir ;
    
    void Start()
    {
        Data.Init();
        var bytesMetaData = Resources.Load<TextAsset>("4239236_highlight_CornerTopRight_4_meta").bytes;
        var metaData = ByteConverter.GetStruct<ReplaySequenceMetaData>(bytesMetaData);
        var bytes = Resources.Load<TextAsset>("4239236_highlight_CornerTopRight_4").bytes;
        Data.SequenceData = ByteConverter.GetSequenceData(bytes, metaData.TotalSteps);
        Data.SequenceMetaData = metaData;
        ball = Instantiate(ballPrefab);
        players = new GameObject[Data.TotalPlayers];
        playerPrevPositions = new Vector3[Data.TotalPlayers];
        playerMovementVel = new Vector2[Data.TotalPlayers];
        predictionVisual = Instantiate(predictionVisualPrefab);
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = Instantiate(playerPrefab);
        }
        
    }

    private void Update()
    {
        Data.HighlightTime += Time.deltaTime * Data.StepsPerSecond;
        BallTransformSystem.Run(ball);
        PlayerTransformSystem.Run(players);
        UpdatePrediction();
    }

    private void UpdatePrediction()
    {
        if (isFirstFrame)
        {
            isFirstFrame = false;
            ballPrevPosition = ball.transform.position;
            for (int i = 0; i < players.Length; i++)
            {
                var pos = players[i].transform.position;
                playerPrevPositions[i] = players[i].transform.position;
            }
            return;
        }
        ballMovementDir = ball.transform.position - ballPrevPosition;
        ballMovementDir.y = 0;
        UseVectorMath();
    }

    private void UseVectorMath()
    {
        for (int i = 0; i < players.Length; i++)
        {
            Vector3 movement3D = players[i].transform.position - playerPrevPositions[i];
            movement3D.y = 0;
            playerMovementVel[i] = new Vector2(movement3D.x, movement3D.z) / Time.deltaTime;
        }

        PredictionSystem.Run(ball, players, predictionVisual, ref playerMovementVel,
            ballMovementDir / Time.deltaTime);
        for (int i = 0; i < players.Length; i++)
        {
            playerPrevPositions[i] = players[i].transform.position;
        }

        if (ballMovementDir.sqrMagnitude > 0.0001f)
        {
            Vector3 start = ball.transform.position;
            start.y = predictionVisual.transform.position.y;
            visualizeRenderer.SetPositions(new []{start, predictionVisual.transform.position});
            ballPrevPosition = ball.transform.position;
        }
        else
        {
            visualizeRenderer.SetPositions(new []{Vector3.zero, Vector3.zero});
        }
    }




#if  UNITY_EDITOR
    private void OnDrawGizmos()
    {
        /*if(!ball) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(ball.transform.position, ball.transform.position + ballMovementDir.normalized * 5);
        Gizmos.color = Color.blue;
        for (int i = 0; i < players.Length; i++)
        {
            Gizmos.DrawLine(players[i].transform.position, 
                players[i].transform.position + new Vector3(playerMovementVel[i].x,0 ,playerMovementVel[i].y) * 5);
        }*/
    }
#endif
}
