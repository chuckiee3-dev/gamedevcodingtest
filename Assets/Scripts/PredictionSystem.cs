using System;
using UnityEngine;

public static class PredictionSystem
{
    private static float eligibleThreshold = .98f;
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    public static void Vector3DotRun(GameObject ball, GameObject[] players, GameObject predictionVisual,
        Vector3 ballMovementDir)
    {
        var ballPosition = ball.transform.position;
        
        Vector2 ballDir = new Vector2(ballMovementDir.x, ballMovementDir.z);
        Vector2 ballPos2D = new Vector2(ballPosition.x, ballPosition.z);

        float minDistSq = float.MaxValue;
        int closestPlayerIndex = -1;
        for (int i = 0; i < players.Length; i++)
        {
            Vector2 playerPos2D = new Vector2(players[i].transform.position.x, players[i].transform.position.z);
            float dot = Vector2.Dot(ballDir.normalized, (playerPos2D - ballPos2D).normalized);
            
            if(dot < eligibleThreshold) continue;
            
            float distSquared = (playerPos2D - ballPos2D).sqrMagnitude;
            if (minDistSq > distSquared )
            {
                minDistSq = distSquared;
                closestPlayerIndex = i;
            }
        }

        if (closestPlayerIndex != -1)
        {
            VisualiseTargetedPlayer(players, closestPlayerIndex);
            Vector2 predictedContactPos = (ball.transform.position + players[closestPlayerIndex].transform.position) / 2;
            Vector3 targetPos = new Vector3(predictedContactPos.x, predictionVisual.transform.position.y,
                predictedContactPos.y);
            predictionVisual.transform.position = targetPos;
        }
    }

    //Used for debugging only
    private static void VisualiseTargetedPlayer(GameObject[] players, int closestPlayerIndex)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (i == closestPlayerIndex)
            {
                players[i].GetComponentInChildren<MeshRenderer>().material.SetColor(ColorId, Color.red);
            }
            else
            {players[i].GetComponentInChildren<MeshRenderer>().material.SetColor(ColorId, Color.white);
            }
        }
    }
}
