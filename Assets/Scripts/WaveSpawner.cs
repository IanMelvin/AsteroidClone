using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    //[SerializeField] float distanceFromEdge_Min = -2.0f;
    //[SerializeField] float distanceFromEdge_Max = 0.0f;
    [SerializeField] float timeBetweenWaves = 2.0f;

    List<AsteroidScript> asteroids = new List<AsteroidScript>();

    int currentWave = 0;
    int totalNumberOfAsteroidsInWave = 0;
    int currentNumberOfDestroyedAsteroids = 0;
    float distanceFromEdge;

    float cameraDistance;
    Vector3 bottomLeft;
    Vector3 topLeft;
    //Vector3 topRight;
    Vector3 bottomRight;

    //float width;
    //float height;

    Camera playerCam;

    private void OnEnable()
    {
        AsteroidScript.OnAsteroidBorn += AddNewAsteroid;
        AsteroidScript.OnAsteroidBroken += UpdateWaveStatus;
    }

    private void OnDisable()
    {
        AsteroidScript.OnAsteroidBorn -= AddNewAsteroid;
        AsteroidScript.OnAsteroidBroken -= UpdateWaveStatus;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCam = Camera.main;

        StartWave();
    }

    void StartWave()
    {
        asteroids.Clear();
        int totalAsteroidsToSpawn = 4 + (currentWave * 2);
        totalAsteroidsToSpawn = totalAsteroidsToSpawn < 11 ? totalAsteroidsToSpawn : 11;

        totalNumberOfAsteroidsInWave = totalAsteroidsToSpawn * 7; // total number of asteroid entities needed to be destroyed to end wave

        for (int i = 0; i < totalAsteroidsToSpawn; i++)
        {
            Instantiate(largeAsteroidPrefab, SpawnLocation(SpawnDirections.TOP, SpawnDirections.BOTTOM, SpawnDirections.LEFT, SpawnDirections.RIGHT), Quaternion.identity).GetComponent<AsteroidScript>();
        }

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
        if(currentNumberOfDestroyedAsteroids == totalNumberOfAsteroidsInWave)
        {
            EndWave();
            StartCoroutine(DelayedStartOfWave(timeBetweenWaves));
        }
    }

    IEnumerator DelayedStartOfWave(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartWave();
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
        //width = bottomRight.x - bottomLeft.x;
        //height = topLeft.y - bottomLeft.y;

        bottomLeft = playerCam.ScreenToWorldPoint(new Vector3(0, 0, cameraDistance));
        topLeft = playerCam.ScreenToWorldPoint(new Vector3(0, Screen.height, cameraDistance));
        //topRight = playerCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cameraDistance));
        bottomRight = playerCam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cameraDistance));
    }

    private Vector2 SpawnLocation(params SpawnDirections[] spawnDirections)
    {
        SpawnDirections randomSpawnDirection = spawnDirections[Random.Range(0, spawnDirections.Length)];
        distanceFromEdge = 0; //Random.Range(distanceFromEdge_Min, distanceFromEdge_Max);

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
}
