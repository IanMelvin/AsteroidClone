using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public static Action<int> OnExtraLifeAwarded;
    public static Action OnSmallSaucerOnlyMilestone;
    public static Action OnAccuracyImprovementMilestone;
    [SerializeField] TextMeshProUGUI player1ScoreText;
    [SerializeField] TextMeshProUGUI player2ScoreText;
    [SerializeField] TextMeshProUGUI player3ScoreText;
    [SerializeField] TextMeshProUGUI player4ScoreText;
    [SerializeField] int numPointsPerExtraLife = 10000;
    [SerializeField] int numPointsBeforeOnlySmallSaucer = 10000;
    [SerializeField] int numPointsSmallSaucerAccuracyImprovement = 35000;

    int player1Score = 0;
    int player2Score = 0;
    int player3Score = 0;
    int player4Score = 0;

    int[] playerExtraLives = {0,0,0,0};

    bool tenThousandPointsAwarded = false;
    bool thirtyFiveThousandPointsAwarded = false;

    private void OnEnable()
    {
        ScoreHolder.OnScoreSentOut += UpdatePlayerScore;
    }

    private void OnDisable()
    {
        ScoreHolder.OnScoreSentOut -= UpdatePlayerScore;
    }

    // Start is called before the first frame update
    void Start()
    {
        player2ScoreText.gameObject.SetActive(false);
        player3ScoreText.gameObject.SetActive(false);
        player4ScoreText.gameObject.SetActive(false);
    }

    private void UpdatePlayerScore(int score, int playerIndex)
    {
        switch (playerIndex)
        {
            case 1:
                player1Score += score;
                CheckForMilestones(player1Score, 1);
                player1ScoreText.text = "P1 Score: " + player1Score;
                break;
            case 2:
                player2Score += score;
                CheckForMilestones(player2Score, 1);
                player2ScoreText.text = "P2 Score: " + player2Score;
                break;
            case 3:
                player3Score += score;
                CheckForMilestones(player3Score, 1);
                player3ScoreText.text = "P3 Score: " + player3Score;
                break;
            case 4:
                player4Score += score;
                CheckForMilestones(player4Score, 1);
                player4ScoreText.text = "P4 Score: " + player4Score;
                break;
            default:
                break;
        }
    }

    private void CheckForMilestones(int playerScore, int playerIndex)
    {
        if(playerScore >= numPointsPerExtraLife * (playerExtraLives[playerIndex - 1] + 1))
        {
            Debug.Log("Extra Life");
            playerExtraLives[playerIndex - 1]++;
            OnExtraLifeAwarded?.Invoke(playerIndex);
        }
        if (playerScore >= numPointsBeforeOnlySmallSaucer && !tenThousandPointsAwarded)
        {
            Debug.Log("Switch to only small saucer");
            tenThousandPointsAwarded = true;
            OnSmallSaucerOnlyMilestone?.Invoke();
        }
        if (playerScore > numPointsSmallSaucerAccuracyImprovement && !thirtyFiveThousandPointsAwarded)
        {
            Debug.Log("Improve small saucer accuracy");
            thirtyFiveThousandPointsAwarded = true;
            OnAccuracyImprovementMilestone?.Invoke();
        }
    }
}
