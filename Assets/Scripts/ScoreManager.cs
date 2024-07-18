using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI player1ScoreText;
    [SerializeField] TextMeshProUGUI player2ScoreText;
    [SerializeField] TextMeshProUGUI player3ScoreText;
    [SerializeField] TextMeshProUGUI player4ScoreText;

    int player1Score = 0;
    int player2Score = 0;
    int player3Score = 0;
    int player4Score = 0;

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
                player1ScoreText.text = "P1 Score: " + player1Score;
                break;
            case 2:
                player2Score += score;
                player2ScoreText.text = "P2 Score: " + player2Score;
                break;
            case 3:
                player3Score += score;
                player3ScoreText.text = "P3 Score: " + player3Score;
                break;
            case 4:
                player4Score += score;
                player4ScoreText.text = "P4 Score: " + player4Score;
                break;
        }
    }
}
