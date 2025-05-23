using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public static Action<int> OnExtraLifeAwarded;
    public static Action<int, int> OnTimeToSendOutPlayerScores;
    public static Action OnSmallSaucerOnlyMilestone;
    public static Action OnAccuracyImprovementMilestone;
    [SerializeField] TextMeshProUGUI[] playerScoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] int numPointsPerExtraLife = 10000;
    [SerializeField] int numPointsBeforeOnlySmallSaucer = 10000;
    [SerializeField] int numPointsSmallSaucerAccuracyImprovement = 35000;

    int[] playerScores = {-1,-1,-1,-1};
    int[] playerExtraLives = {0,0,0,0};

    bool tenThousandPointsAwarded = false;
    bool thirtyFiveThousandPointsAwarded = false;

    private void OnEnable()
    {
        MenuManager.OnGameStarted += StartGame;
        MenuManager.OnMainMenuOpen += Reset;
        HealthManager.OnGameOver += GameOver;
        ScoreHolder.OnScoreSentOut += UpdatePlayerScore;
    }

    private void OnDisable()
    {
        MenuManager.OnGameStarted -= StartGame;
        MenuManager.OnMainMenuOpen -= Reset;
        HealthManager.OnGameOver -= GameOver;
        ScoreHolder.OnScoreSentOut -= UpdatePlayerScore;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerScores[0] = 0;
        playerScoreText[2].enabled = false;
        playerScoreText[3].enabled = false;
    }

    private void StartGame()
    {
        playerScoreText[1].enabled = false;
    }

    private void Reset()
    {
        playerScores[0] = 0;
        playerScoreText[1].enabled = true;
        UpdatePlayerScore(0, 1);
    }

    private void GameOver()
    {
        int biggestScore = 0;
        for(int i = 0; i < playerScores.Length; i++)
        {
            if (playerScores[i] > biggestScore) biggestScore = playerScores[i];
            if (playerScores[i] >= 0) OnTimeToSendOutPlayerScores(i+1, playerScores[i]);
        }
        if(int.Parse(highScoreText.text) < biggestScore) highScoreText.text = biggestScore.ToString("00");
    }

    private void UpdatePlayerScore(int score, int playerIndex)
    {
        if (playerScoreText.Length <= playerIndex || playerIndex < 0 ) return;

        playerScores[playerIndex - 1] += score;
        playerScoreText[playerIndex - 1].text = playerScores[playerIndex - 1].ToString("00");
        CheckForMilestones(playerIndex);
    }

    private void CheckForMilestones(int playerIndex)
    {
        if(playerScores[playerIndex - 1] >= numPointsPerExtraLife * (playerExtraLives[playerIndex - 1] + 1))
        {
            Debug.Log("Extra Life");
            playerExtraLives[playerIndex - 1]++;
            OnExtraLifeAwarded?.Invoke(playerIndex);
        }
        if (playerScores[playerIndex - 1] >= numPointsBeforeOnlySmallSaucer && !tenThousandPointsAwarded)
        {
            Debug.Log("Switch to only small saucer");
            tenThousandPointsAwarded = true;
            OnSmallSaucerOnlyMilestone?.Invoke();
        }
        if (playerScores[playerIndex - 1] > numPointsSmallSaucerAccuracyImprovement && !thirtyFiveThousandPointsAwarded)
        {
            Debug.Log("Improve small saucer accuracy");
            thirtyFiveThousandPointsAwarded = true;
            OnAccuracyImprovementMilestone?.Invoke();
        }
    }
}
