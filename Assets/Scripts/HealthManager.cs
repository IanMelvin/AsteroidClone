using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] GameObject[] player1LifeSprites;
    [SerializeField] GameObject[] player2LifeSprites;
    [SerializeField] GameObject[] player3LifeSprites;
    [SerializeField] GameObject[] player4LifeSprites;

    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject player3;
    [SerializeField] GameObject player4;

    [SerializeField] int numLives = 3;
    [SerializeField] float respawnDelay = 2f;
    [SerializeField] float spawnRadiusSafeZone = 2f;

    WaveSpawner spawner;
    int[] playerLives;

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += RemoveALife;
        ScoreManager.OnExtraLifeAwarded += AwardExtraLife;
        playerLives = new int[] { numLives, numLives, numLives, numLives };
        spawner = GetComponent<WaveSpawner>();
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= RemoveALife;
        ScoreManager.OnExtraLifeAwarded -= AwardExtraLife;
    }

    void UpdateLifeSprites(int playerIndex, bool status)
    {
        if (playerLives[playerIndex - 1] >= numLives || playerLives[playerIndex - 1] < 0) return;

        switch (playerIndex)
        {
            case 1:
                player1LifeSprites[playerLives[playerIndex - 1]].SetActive(status);
                break;
            case 2:
                player2LifeSprites[playerLives[playerIndex - 1]].SetActive(status);
                break;
            case 3:
                player3LifeSprites[playerLives[playerIndex - 1]].SetActive(status);
                break;
            case 4:
                player4LifeSprites[playerLives[playerIndex - 1]].SetActive(status);
                break;
        }
    }

    IEnumerator RespawnDelay(float delay, int playerIndex)
    {
        float timer = 0;
        yield return new WaitForSeconds(delay);
        timer += delay;

        bool inRadius = true;
        while(inRadius)
        {
            inRadius = false;
            for (int i = 0; i < spawner.GetAsteroids().Count; i++)
            {
                if ((Vector3.zero - spawner.GetAsteroids()[i].transform.position).magnitude < spawnRadiusSafeZone)
                {
                    inRadius = true;
                    break;
                }
            }

            if (inRadius)
            {
                timer += delay / 2.0f;
                yield return new WaitForSeconds(delay / 2.0f);
            }
        }

        Debug.Log($"Respawn: {timer} seconds");
        RespawnPlayer(playerIndex);
    }

    void RespawnPlayer(int playerIndex)
    {
        switch (playerIndex)
        {
            case 1:
                player1.transform.position = Vector3.zero;
                player1.transform.rotation = Quaternion.identity;
                player1.SetActive(true);
                break;
            case 2:
                player2.transform.position = Vector3.zero;
                player2.transform.rotation = Quaternion.identity;
                player2.SetActive(true);
                break;
            case 3:
                player3.transform.position = Vector3.zero;
                player3.transform.rotation = Quaternion.identity;
                player3.SetActive(true);
                break;
            case 4:
                player4.transform.position = Vector3.zero;
                player4.transform.rotation = Quaternion.identity;
                player4.SetActive(true);
                break;
        }
    }

    void AwardExtraLife(int playerIndex)
    {
        UpdateLifeSprites(playerIndex, true);
        playerLives[playerIndex - 1]++;
        //Debug.Log(playerLives[playerIndex - 1]);
    }

    void RemoveALife(int playerIndex)
    {
        playerLives[playerIndex - 1]--;
        //Debug.Log(playerLives[playerIndex - 1]);
        UpdateLifeSprites(playerIndex, false);

        if (playerLives[playerIndex - 1] <= 0) Debug.Log("GameOver");
        else StartCoroutine(RespawnDelay(respawnDelay, 1));
    }
}
