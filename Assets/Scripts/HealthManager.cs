using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI player1LifeText;
    [SerializeField] TextMeshProUGUI player2LifeText;
    [SerializeField] TextMeshProUGUI player3LifeText;
    [SerializeField] TextMeshProUGUI player4LifeText;

    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject player3;
    [SerializeField] GameObject player4;

    [SerializeField] int defaultNumLives = 3;
    [SerializeField] float respawnDelay = 2f;
    [SerializeField] float spawnRadiusSafeZone = 2f;

    WaveSpawner spawner;
    int[] playerLives;

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += RemoveALife;
        ScoreManager.OnExtraLifeAwarded += AwardExtraLife;
        playerLives = new int[] { defaultNumLives, defaultNumLives, defaultNumLives, defaultNumLives };
        spawner = GetComponent<WaveSpawner>();

        if (player1) UpdateLifeUI(1);
        if (player2) UpdateLifeUI(2);
        if (player3) UpdateLifeUI(4);
        if (player4) UpdateLifeUI(4);
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= RemoveALife;
        ScoreManager.OnExtraLifeAwarded -= AwardExtraLife;
    }

    void UpdateLifeUI(int playerIndex)
    {
        if (playerLives[playerIndex - 1] < 0) return;

        switch (playerIndex)
        {
            case 1:
                player1LifeText.text = $"x {playerLives[playerIndex-1]}";
                break;
            case 2:
                player2LifeText.text = $"x {playerLives[playerIndex-1]}";
                break;
            case 3:
                player3LifeText.text = $"x {playerLives[playerIndex - 1]}";
                break;
            case 4:
                player4LifeText.text = $"x {playerLives[playerIndex - 1]}";
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
        playerLives[playerIndex - 1]++;
        UpdateLifeUI(playerIndex);
    }

    void RemoveALife(int playerIndex)
    {
        playerLives[playerIndex - 1]--;
        UpdateLifeUI(playerIndex);

        if (playerLives[playerIndex - 1] <= 0) Debug.Log("GameOver");
        else StartCoroutine(RespawnDelay(respawnDelay, 1));
    }
}
