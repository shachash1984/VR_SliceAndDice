using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public Enemy[] enemyPrefabs;
    private List<Enemy> _activeEnemies = new List<Enemy>();
    [SerializeField] private float minSpawnDistance = 5f;
    [SerializeField] private float maxSpawnDistance = 10f;
    [SerializeField] private float minSpawnAltitude = 0.4f;
    [SerializeField] private float maxSpawnAltitude = 3f;
    [SerializeField] private float minSpawnHealth = 10f;
    [SerializeField] private float maxSpawnHealth = 50f;
    [SerializeField] private float minSpawnSpeed = 5f;
    [SerializeField] private float maxSpawnSpeed = 10f;
    [SerializeField] private float minSpawnDamage = 5f;
    [SerializeField] private float maxSpawnDamage = 10f;
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 3f;
    private float _timer;
    private float _currentSpawnTime;

    private void Start()
    {
        ResetTimer();
    }

    private void OnEnable()
    {
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Update()
    {
        if(Time.time - _currentSpawnTime > _timer)
        {
            SpawnEnemy();
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        _timer = Time.time;
        _currentSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void SpawnEnemy()
    {
        int spawnIndex = Random.Range(0, enemyPrefabs.Length);
        float spawnDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 spawnPos = Random.insideUnitSphere * spawnDistance;
        if (!enemyPrefabs[spawnIndex].isAirBorn)
            spawnPos.y = 0f;
        else
            spawnPos.y = Random.Range(minSpawnAltitude, maxSpawnAltitude);
        float spawnHealth = Random.Range(minSpawnHealth, maxSpawnHealth);
        float spawnSpeed = Random.Range(minSpawnSpeed, maxSpawnSpeed);
        float spawnDamage = Random.Range(minSpawnDamage, maxSpawnDamage);
        Quaternion spawnRot = Quaternion.LookRotation(Player.S.transform.position - spawnPos, Vector3.up);
        _activeEnemies.Add(Instantiate(enemyPrefabs[spawnIndex], spawnPos, spawnRot).Init(spawnHealth, spawnSpeed, spawnDamage));
    }

    void HandleEnemyDestroyed(Enemy e)
    {
        _activeEnemies.Remove(e);
    }
}
