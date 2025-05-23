using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static Action OnGameStarted;
    public static Action OnMainMenuOpen;
    [SerializeField] TextMeshProUGUI startText;
    [SerializeField] TextMeshProUGUI currentPlayerText;
    [SerializeField] TextMeshProUGUI leaderboardPromptText;
    [SerializeField] TextMeshProUGUI initialsEntryText;
    [SerializeField] TextMeshProUGUI leaderboardText;
    [SerializeField] GameObject pauseMenu;

    char[] characterList = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_'};

    bool canStartGame = true;
    bool isPaused = false;
    bool startGameTimerOn = false;
    bool leaderboardTimerOn = false;
    float startGameTimer = 0.0f;
    float leaderboardTimer = 0.0f;
    int playerScore = 0;

    private void OnEnable()
    {
        //HealthManager.OnGameOver += GameEnded;
        ScoreManager.OnTimeToSendOutPlayerScores += CheckScoreForLeaderboard;
        UniversalPauseManager.OnPauseStateChanged += TogglePauseMenu;
    }

    private void OnDisable()
    {
        //HealthManager.OnGameOver -= GameEnded;
        ScoreManager.OnTimeToSendOutPlayerScores -= CheckScoreForLeaderboard;
        UniversalPauseManager.OnPauseStateChanged -= TogglePauseMenu;
    }

    // Start is called before the first frame update
    void Start()
    {
        OnMainMenuOpen?.Invoke();
        StartCoroutine("ToggleStartText");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isPaused)
        {
            if (startGameTimer > 0.0f) startGameTimer -= Time.deltaTime;
            else if (startGameTimerOn)
            {
                startGameTimerOn = false;
                currentPlayerText.enabled = false;
            }

            if (leaderboardTimer > 0.0f) leaderboardTimer -= Time.deltaTime;
            else if (leaderboardTimerOn)
            {
                leaderboardTimerOn = false;
                leaderboardText.enabled = false;
                canStartGame = true;
                StartCoroutine("ToggleStartText");
                OnMainMenuOpen.Invoke();
            }
        }
    }

    public void OnStartButton(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        if (context.phase == InputActionPhase.Started && canStartGame)
        {
            StopAllCoroutines();
            startText.enabled = false;
            canStartGame = false;
            OnGameStarted?.Invoke();
            currentPlayerText.enabled = true;
            startGameTimer = 2.0f;
            startGameTimerOn = true;
        }

        if (context.phase == InputActionPhase.Started && leaderboardText.enabled)
        {
            leaderboardText.enabled = false;
            canStartGame = true;
            StartCoroutine("ToggleStartText");
            OnMainMenuOpen.Invoke();
            leaderboardTimerOn = false;
        }

        if (context.phase == InputActionPhase.Started && initialsEntryText.enabled)
        {
            //Update leaderboard with initials
            leaderboardPromptText.enabled = false;
            initialsEntryText.enabled = false;
            leaderboardText.enabled = true;
            leaderboardText.text = $"{leaderboardText.text}\n\n1  {playerScore} {(initialsEntryText.text.Contains('_') ? initialsEntryText.text.Replace('_', ' ') : initialsEntryText.text)}";
            leaderboardTimer = 10.0f;
            leaderboardTimerOn = true;
        }
    }

    private void CheckScoreForLeaderboard(int playerIndex, int score)
    {
        //Check if score is big enough to get on the leaderboard
        leaderboardPromptText.enabled = true;
        initialsEntryText.enabled = true;
        playerScore = score;
    }

    private void TogglePauseMenu(bool pauseState)
    {
        isPaused = pauseState;
        if(pauseMenu) pauseMenu?.SetActive(pauseState);
    }

    IEnumerator ToggleStartText()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => isPaused == false);
            if (startText.enabled) startText.enabled = false;
            else startText.enabled = true;
        }
    }
}
