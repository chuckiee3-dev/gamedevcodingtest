using System;
using UnityEngine;

public static class PredictionSystem
{
    private static float eligibleThresholdBasicDot = .98f;
    private static float eligibleThresholdLine = .95f;
    private static float contactDistSq = .01f;
    private static readonly int ColorId = Shader.PropertyToID("_Color");


    public static void Run(GameObject ball, GameObject[] players, GameObject predictionVisual, 
        ref Vector2[] playerMovementVel,
        Vector3 ballMovementDir)
    {
        var ballPosition = ball.transform.position;
        
        Vector2 ballVel = new Vector2(ballMovementDir.x, ballMovementDir.z).normalized;
        Vector2 ballPos2D = new Vector2(ballPosition.x, ballPosition.z);
       
        int intersectionPlayerIndex = -1;
        
        Vector2 intersectionPos = Vector2.one * 100000f;
        int calculatedIter = 0;
        float distPlayerFinal = float.MaxValue;
        for (int i = 0; i < players.Length; i++)
        {
            Vector2 playerPos2D = new Vector2(players[i].transform.position.x, players[i].transform.position.z);
            

            int maxIter = 300;
            int iter=0;

            Vector2 ballFuturePos = ballPos2D;
            Vector2 playerFuturePos = playerPos2D;
            float distPlayer = float.MaxValue;
            while (iter < maxIter && distPlayer > contactDistSq)
            {
                if (playerMovementVel[i].sqrMagnitude > 0.0001f)
                {
                    playerFuturePos += playerMovementVel[i];
                }

                if (ballFuturePos.sqrMagnitude > 0.00001f)
                {
                    ballFuturePos += ballVel;
                }
                float newDist = (ballFuturePos - playerFuturePos).sqrMagnitude;
                if (distPlayer >= newDist)
                {
                    distPlayer = newDist;
                    if(distPlayer < distPlayerFinal)
                    {
                        distPlayerFinal = distPlayer;
                        calculatedIter = iter + 1;
                        intersectionPlayerIndex = i;
                        intersectionPos = ballFuturePos;
                    }
                }
                iter++;
            }
            
        }

        if (intersectionPlayerIndex != -1 && !float.IsNaN(intersectionPos.x) && !float.IsNaN(intersectionPos.y))
        {
            intersectionPos = ballPos2D + ballVel * calculatedIter;
            Vector3 targetPos = new Vector3(intersectionPos.x, predictionVisual.transform.position.y,
                intersectionPos.y);
            predictionVisual.transform.position = targetPos;
        }
    }
 
}
