using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HealthManager : MonoBehaviour
{
    public static Action<int> OnPlayerRespawn;

    [SerializeField] TextMeshProUGUI[] playerLifeText;
    //[SerializeField] TextMeshProUGUI player2LifeText;
    //[SerializeField] TextMeshProUGUI player3LifeText;
    //[SerializeField] TextMeshProUGUI player4LifeText;

    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject player3;
    [SerializeField] GameObject player4;

    [SerializeField] int defaultNumLives = 3;
    [SerializeField] float respawnDelay = 2f;
    [SerializeField] float spawnRadiusSafeZone = 2f;
    [SerializeField] AudioSource extraLifeAudio;

    WaveSpawner spawner;
    int[] playerLives;

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += RemoveALife;
        ScoreManager.OnExtraLifeAwarded += AwardExtraLife;
        playerLives = new int[] { defaultNumLives, defaultNumLives, defaultNumLives, defaultNumLives };
        spawner = GetComponent<WaveSpawner>();
        if (!extraLifeAudio) extraLifeAudio = GetComponent<AudioSource>();

        if (player1) UpdateLifeUI(1);
        if (player2) UpdateLifeUI(2);
        if (player3) UpdateLifeUI(3);
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

        playerLifeText[playerIndex - 1].text = $"x {playerLives[playerIndex - 1]}";
        /*switch (playerIndex)
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
        }*/
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
        OnPlayerRespawn?.Invoke(playerIndex);
    }

    void AwardExtraLife(int playerIndex)
    {
        StartCoroutine("PlayExtraLifeAudio");
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

    IEnumerator PlayExtraLifeAudio()
    {
        extraLifeAudio.Play();
        extraLifeAudio.loop = true;
        yield return new WaitForSeconds(extraLifeAudio.clip.length * 12);
        extraLifeAudio.Stop();
    }
}
