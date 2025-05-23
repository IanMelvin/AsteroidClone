using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public struct LeaderboardData
{
    public string initials;
    public int score;

    public LeaderboardData(string initials, int score)
    {
        this.initials = initials;
        this.score = score;
    }
}

public class LeaderboardManager : MonoBehaviour
{
    public static Action<int> OnHighScoreLoaded;
    [SerializeField] TextMeshProUGUI leaderboardText;
    List<LeaderboardData> leaderboard = new List<LeaderboardData>();

    private void OnEnable()
    {
        //ScoreManager.OnTimeToSendOutPlayerScores += UpdateLeaderBoard;
        MenuManager.OnMainMenuOpen += SaveLeaderBoard;
    }

    private void OnDisable()
    {
        //ScoreManager.OnTimeToSendOutPlayerScores -= UpdateLeaderBoard;
        MenuManager.OnMainMenuOpen -= SaveLeaderBoard;
    }

    private void Start()
    {
        LoadLeaderBoard();
    }

    public bool CheckIfCanGoOnLeaderboard(int score)
    {
        if (leaderboard.Count <= 10) return true;
        if (leaderboard[leaderboard.Count - 1].score > score)  return false;
        return true;
    }

    public void DisplayLeaderboard()
    {
        leaderboardText.text = $"High Scores\n\n";
        for(int i = 0; i < leaderboard.Count; i++)
        {
            if (i+1 == 10) leaderboardText.text += $"{i + 1} {leaderboard[i].score} {leaderboard[i].initials}\n";
            else leaderboardText.text += $"{i + 1}  {leaderboard[i].score} {leaderboard[i].initials}\n";
        }
    }

    public void UpdateLeaderBoard(string initials, int score)
    {
        if (leaderboard.Count <= 0)
        {
            leaderboard.Add(new LeaderboardData(initials, score));
            return;
        }
        int score1InsertKey = int.MaxValue;

        int i = 0;
        foreach (var leaderboardLines in leaderboard) 
        { 
            if(leaderboardLines.score < score)
            {
                if(i < score1InsertKey) score1InsertKey = i;
            }
            i++;
        }

        if(score1InsertKey == int.MaxValue && leaderboard.Count < 10)
        {
            leaderboard.Add(new LeaderboardData(initials, score));
            return;
        }

        if (score1InsertKey != int.MaxValue)
        {
            leaderboard.Insert(score1InsertKey, new LeaderboardData(initials, score));
        }
    }

    private void LoadLeaderBoard()
    {
#if UNITY_EDITOR
        string path = "Assets/Resources/LeaderBoardStorage.txt";
#endif
#if !UNITY_EDITOR
        string path = Application.persistentDataPath + "/LeaderBoardStorage.txt";
#endif
        if (File.Exists(path))
        {
            StreamReader reader = new StreamReader(path);
            string fileText = reader.ReadToEnd();
            string[] lines = fileText.Split("\n");
            int i = 0;
            foreach (string line in lines)
            {
                string[] leaderBoardLine = line.Split(',');
                if(leaderBoardLine.Length > 1)
                {
                    leaderboard.Add(new LeaderboardData(leaderBoardLine[0], int.Parse(leaderBoardLine[1])));
                    if(i == 0) OnHighScoreLoaded?.Invoke(int.Parse(leaderBoardLine[1]));
                    i++;
                }
            }
            reader.Close();
        }
    }

    private void SaveLeaderBoard()
    {
#if UNITY_EDITOR
        string path = "Assets/Resources/LeaderBoardStorage.txt";
        StreamWriter writer = new StreamWriter(path);
#endif
#if !UNITY_EDITOR
        string path = Application.persistentDataPath + "/LeaderBoardStorage.txt";
        StreamWriter writer = new StreamWriter(path);
#endif
        string textToWrite = "";
        int i = 0;
        foreach (LeaderboardData leaderboardLine in leaderboard)
        {
            if (i == 9)
            {
                textToWrite += leaderboardLine.initials + "," + leaderboardLine.score.ToString();
                break;
            }
            else textToWrite += leaderboardLine.initials + "," + leaderboardLine.score.ToString() + "\n";
            i++;
        }

        writer.Write(textToWrite);
        writer.Close();
    }
}
