using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOSpawnPositionManager : MonoBehaviour
{
    [SerializeField] Transform[] leftSideSpawnpoints;
    [SerializeField] Transform[] rightSideSpawnpoints;

    Vector2 topLeft;
    Vector2 bottomRight;
    Camera playerCam;

    private void OnEnable()
    {
        playerCam = Camera.main;
        AdjustSpawnPositions();
    }

    private void OnDisable()
    {
        
    }

    private void AdjustSpawnPositions()
    {
        topLeft = playerCam.ScreenToWorldPoint(new Vector3(0, Screen.height));
        bottomRight = playerCam.ScreenToWorldPoint(new Vector3(Screen.width, 0));
        Debug.Log($"{topLeft} : {bottomRight}");

        foreach (Transform t in leftSideSpawnpoints)
        {
            t.position = new Vector2(topLeft.x - 1.51f, t.position.y);
        }

        foreach (Transform t in rightSideSpawnpoints)
        {
            t.position = new Vector2(bottomRight.x + 1.51f, t.position.y);
        }
    }
}
