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

    private GameObject ball;
    private GameObject[] players;
    private GameObject predictionVisual;
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
        if (isFirstFrame)
        {
            isFirstFrame = false;
            ballPrevPosition = ball.transform.position;
            return;
        }
        ballMovementDir = ball.transform.position - ballPrevPosition;
        ballMovementDir.y = 0;
        if(ballMovementDir.sqrMagnitude >= .001f){
            PredictionSystem.Vector3DotRun(ball, players, predictionVisual, ballMovementDir.normalized);
        }

        ballPrevPosition = ball.transform.position;
    }

    private void OnDrawGizmos()
    {
        if(!ball) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(ball.transform.position, ball.transform.position + ballMovementDir.normalized * 5);
    }
}
