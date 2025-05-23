using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    LeaderboardManager leaderboard;
    bool canStartGame = true;
    bool isPaused = false;
    bool startGameTimerOn = false;
    bool leaderboardTimerOn = false;
    float startGameTimer = 0.0f;
    float leaderboardTimer = 0.0f;
    int playerScore = 0;
    int characterIndex = 0;
    int characterColumn = 0;
    int[] initialIndex;

    private void OnEnable()
    {
        PlayerMovement_Retro.OnStartButtonPressed += ProcessStartButton;
        ScoreManager.OnTimeToSendOutPlayerScores += CheckScoreForLeaderboard;
        UniversalPauseManager.OnPauseStateChanged += TogglePauseMenu;
    }

    private void OnDisable()
    {
        PlayerMovement_Retro.OnStartButtonPressed -= ProcessStartButton;
        ScoreManager.OnTimeToSendOutPlayerScores -= CheckScoreForLeaderboard;
        UniversalPauseManager.OnPauseStateChanged -= TogglePauseMenu;
    }

    // Start is called before the first frame update
    void Start()
    {
        OnMainMenuOpen?.Invoke();
        leaderboard = GetComponent<LeaderboardManager>();
        initialIndex = new int []{ 0, characterList.Length - 1, characterList.Length - 1 };
        StartCoroutine("ToggleStartText");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W) && initialsEntryText.enabled)
        {
            characterIndex++;
            characterIndex = characterIndex % characterList.Length;
            UpdateInitials();
        }

        if(Input.GetKeyDown(KeyCode.S) && initialsEntryText.enabled)
        {
            characterIndex--;
            characterIndex = characterIndex % characterList.Length;
            UpdateInitials();
        }
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

    private void UpdateInitials()
    {
        initialIndex[characterColumn] = characterIndex;
        initialsEntryText.text = $"{characterList[initialIndex[0]]}{characterList[initialIndex[1]]}{characterList[initialIndex[2]]}";
    }

    private void CheckScoreForLeaderboard(int playerIndex, int score)
    {
        //Check if score is big enough to get on the leaderboard
        if (leaderboard.CheckIfCanGoOnLeaderboard(score))
        {
            leaderboardPromptText.enabled = true;
            initialsEntryText.enabled = true;
            playerScore = score;
        }
        else
        {
            leaderboard.DisplayLeaderboard();
            leaderboardText.enabled = true;
            leaderboardTimer = 10.0f;
            leaderboardTimerOn = true;
        }
    }

    private void ProcessStartButton()
    {
        if (isPaused) return;

        if (canStartGame)
        {
            StopAllCoroutines();
            startText.enabled = false;
            canStartGame = false;
            OnGameStarted?.Invoke();
            currentPlayerText.enabled = true;
            startGameTimer = 2.0f;
            startGameTimerOn = true;
        }

        if (leaderboardText.enabled)
        {
            leaderboardText.enabled = false;
            canStartGame = true;
            StartCoroutine("ToggleStartText");
            OnMainMenuOpen.Invoke();
            leaderboardTimerOn = false;
        }

        if (initialsEntryText.enabled)
        {
            if (characterColumn < 2)
            {
                characterColumn++;
                characterIndex = 0;
                UpdateInitials();
            }
            else
            {
                leaderboardPromptText.enabled = false;
                leaderboardText.enabled = true;
                //leaderboardText.text = $"{leaderboardText.text}\n\n1  {playerScore} {(initialsEntryText.text.Contains('_') ? initialsEntryText.text.Replace('_', ' ') : initialsEntryText.text)}";
                leaderboard.UpdateLeaderBoard($"{(initialsEntryText.text.Contains('_') ? initialsEntryText.text.Replace('_', ' ') : initialsEntryText.text)}", playerScore);
                ResetInitialsInput();
                initialsEntryText.enabled = false;
                leaderboard.DisplayLeaderboard();
                leaderboardTimer = 10.0f;
                leaderboardTimerOn = true;
            }
        }
    }

    private void ResetInitialsInput()
    {
        characterIndex = 0;
        characterColumn = 0;
        initialIndex[0] = 0;
        initialIndex[1] = characterList.Length - 1;
        initialIndex[2] = characterList.Length - 1;
        UpdateInitials();
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
