using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using Random = UnityEngine.Random;

enum SpawnDirections
{
    TOP,
    BOTTOM,
    LEFT,
    RIGHT
}

public class WaveSpawner : MonoBehaviour
{
    public static Action OnWaveStarted;
    public static Action OnWaveEnded;

    [SerializeField] GameObject largeAsteroidPrefab;
    [SerializeField] GameObject largeSaucerPrefab;
    [SerializeField] GameObject smallSaucerPrefab;
    [SerializeField] Transform[] saucerSpawnPoints;
    [SerializeField] float timeBetweenWaves = 2.0f;

    List<AsteroidScript> asteroids = new List<AsteroidScript>();

    int currentWave = 0;
    int totalNumberOfAsteroidsInWave = 0;
    int currentNumberOfDestroyedAsteroids = 0;
    int activeSaucers = 0;
    float distanceFromEdge;
    float cameraDistance;
    Vector3 bottomLeft;
    Vector3 topLeft;
    Vector3 bottomRight;
    bool isPaused = false;
    bool onlySmallSaucer = false;
    bool startOfWaveTimerOn = false;
    float startOfWaveTimer = 0.0f;
    bool saucerSpawnTimerOn = false;
    float saucerSpawnTimer = 0.0f;

    Camera playerCam;

    private void OnEnable()
    {
        AsteroidScript.OnAsteroidBorn += AddNewAsteroid;
        AsteroidScript.OnAsteroidBroken += UpdateWaveStatus;
        UniversalPauseManager.OnPauseStateChanged += SetPauseState;
        ScoreManager.OnSmallSaucerOnlyMilestone += SmallSaucerOnlyMilestoneReached;
        UFOHealth.OnSaucerDestroyed += StartNewSaucerTimer;
    }

    private void OnDisable()
    {
        AsteroidScript.OnAsteroidBorn -= AddNewAsteroid;
        AsteroidScript.OnAsteroidBroken -= UpdateWaveStatus;
        UniversalPauseManager.OnPauseStateChanged -= SetPauseState;
        ScoreManager.OnSmallSaucerOnlyMilestone -= SmallSaucerOnlyMilestoneReached;
        UFOHealth.OnSaucerDestroyed -= StartNewSaucerTimer;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCam = Camera.main;

        StartWave();
    }

    void FixedUpdate()
    {
        if (!isPaused)
        {
            if (startOfWaveTimer > 0.0f) startOfWaveTimer -= Time.deltaTime;
            else if (startOfWaveTimerOn)
            {
                startOfWaveTimerOn = false;
                StartWave();
            }

            if (saucerSpawnTimer > 0.0f) saucerSpawnTimer -= Time.deltaTime;
            else if (saucerSpawnTimerOn)
            {
                saucerSpawnTimerOn = false;
                Debug.Log("Spawned Saucer");
                Instantiate(GetSaucerPrefab(), GetSaucerSpawnPos(), transform.rotation).GetComponent<UFOMovement>();
                activeSaucers++;
            }
        }
    }

    void StartWave()
    {
        asteroids.Clear();
        int totalAsteroidsToSpawn = 4 + (currentWave * 2);
        totalAsteroidsToSpawn = totalAsteroidsToSpawn < 11 ? totalAsteroidsToSpawn : 11;

        totalNumberOfAsteroidsInWave = totalAsteroidsToSpawn * 7; // total number of asteroid entities needed to be destroyed to end wave

        for (int i = 0; i < totalAsteroidsToSpawn; i++)
        {
            Instantiate(largeAsteroidPrefab, SpawnLocation(SpawnDirections.TOP, SpawnDirections.BOTTOM, SpawnDirections.LEFT, SpawnDirections.RIGHT), Quaternion.identity);
        }

        saucerSpawnTimer = 35;
        saucerSpawnTimerOn = true;
        Debug.Log("Started Spawn timer");
        OnWaveStarted?.Invoke();
    }

    void AddNewAsteroid(AsteroidScript asteroidBorn)
    {
        asteroids.Add(asteroidBorn);
    }

    void UpdateWaveStatus(AsteroidScript asteroidBroken)
    {
        asteroids.Remove(asteroidBroken);

        currentNumberOfDestroyedAsteroids++;
        if(currentNumberOfDestroyedAsteroids == totalNumberOfAsteroidsInWave && activeSaucers == 0)
        {
            EndWave();
            saucerSpawnTimerOn = false;
            saucerSpawnTimer = 0.0f;
            startOfWaveTimer = timeBetweenWaves;
            startOfWaveTimerOn = true;
        }
    }

    void EndWave()
    {
        currentWave++;
        currentNumberOfDestroyedAsteroids = 0;
        OnWaveEnded?.Invoke();
    }
    
    private void DefineScreenValues()
    {
        cameraDistance = playerCam.nearClipPlane;
        bottomLeft = playerCam.ScreenToWorldPoint(new Vector3(0, 0, cameraDistance));
        topLeft = playerCam.ScreenToWorldPoint(new Vector3(0, Screen.height, cameraDistance));
        bottomRight = playerCam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cameraDistance));
    }

    private Vector2 SpawnLocation(params SpawnDirections[] spawnDirections)
    {
        SpawnDirections randomSpawnDirection = spawnDirections[Random.Range(0, spawnDirections.Length)];
        distanceFromEdge = 0;

        DefineScreenValues();

        float x = Random.Range(bottomLeft.x - distanceFromEdge, bottomRight.x + distanceFromEdge);
        float y = Random.Range(bottomLeft.y - distanceFromEdge, topLeft.y + distanceFromEdge);

        switch(randomSpawnDirection)
        {
            case SpawnDirections.TOP:
                y = topLeft.y + distanceFromEdge;
                break;
            case SpawnDirections.BOTTOM:
                y = bottomLeft.y - distanceFromEdge;
                break;
            case SpawnDirections.LEFT:
                x = bottomLeft.x - distanceFromEdge;
                break;
            case SpawnDirections.RIGHT:
                x = bottomRight.x + distanceFromEdge;
                break;
        }

        return new Vector2(x, y);
    }

    public List<AsteroidScript> GetAsteroids()
    {
        return asteroids;
    }

    void SetPauseState(bool pauseState)
    {
        isPaused = pauseState;
    }

    void SmallSaucerOnlyMilestoneReached()
    {
        onlySmallSaucer = true;
    }

    void StartNewSaucerTimer()
    {
        activeSaucers--;

        if (currentNumberOfDestroyedAsteroids == totalNumberOfAsteroidsInWave && activeSaucers == 0)
        {
            EndWave();
            saucerSpawnTimerOn = false;
            saucerSpawnTimer = 0.0f;
            startOfWaveTimer = timeBetweenWaves;
            startOfWaveTimerOn = true;
        }
        else
        {
            saucerSpawnTimer = 11 - currentWave + Random.Range(0, 4);
            saucerSpawnTimerOn = true;
            Debug.Log("Started Spawn timer");
        }
    }

    GameObject GetSaucerPrefab()
    {
        if(!onlySmallSaucer) return Random.Range(0, 5) < 4 ? largeSaucerPrefab : smallSaucerPrefab;
        return smallSaucerPrefab;
    }

    Vector3 GetSaucerSpawnPos()
    {
        return saucerSpawnPoints[Random.Range(0,saucerSpawnPoints.Length)].position;
    }
}
