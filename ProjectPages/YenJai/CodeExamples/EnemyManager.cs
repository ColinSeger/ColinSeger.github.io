using System;
using System.Collections.Generic;
using UnityEngine;
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public Action<int> enemyCountChange;
    public Action<int> KilledEnemiesScore;
    [SerializeField] GameObject enemyPrefab;

    List<Enemy> enemiesList = new List<Enemy>(20);
    void Awake()
    {
        if (EnemyManager.Instance) return;
        Instance = this;
    }
    void FixedUpdate()
    {
        for (int i = 0; i < enemiesList.Count; i++)
        {
            Enemy enemy = enemiesList[i];
            Vector3 target = enemy.location + enemy.offset;
            if (enemy.enemyPrefab)
            {
                enemy.enemyPrefab.transform.position = target;
                enemy.MoveTick();
            }
            else
            {
                RemoveInvalid();
            }
        }
    }

    public void SpawnAtLocation(GameObject enemyPrefab, Vector3 location)
    {
        GameObject enemy = Instantiate(enemyPrefab, location, Quaternion.identity);
        AddEnemy(enemy);
    }
    public void SpawnAtLocation(Enemy enemyStats, Vector3 location)
    {
        GameObject enemy = Instantiate(enemyStats.enemyPrefab, new Vector3(location.x,location.y, this.transform.position.z), Quaternion.identity);
        AddEnemy(enemy, enemyStats);
    }

    public void AddEnemy(GameObject enemy)
    {
        Enemy enemyToAdd = new Enemy();

        enemyToAdd.enemyPrefab = enemy;
        enemyToAdd.location = enemyToAdd.enemyPrefab.transform.position;
        enemyToAdd.offset = Vector3.zero;
        AddEnemy(enemyToAdd);
    }
    
    public void AddEnemy(GameObject enemy, Enemy enemyToAdd)
    {
        enemyToAdd.enemyPrefab = enemy;
        enemyToAdd.location = enemyToAdd.enemyPrefab.transform.position;
        enemyToAdd.offset = Vector3.zero;
        
        AddEnemy(enemyToAdd);
    }

    public void AddEnemy(Enemy enemy)
    {
        if (!enemy.enemyPrefab) return;
        var health = enemy.enemyPrefab.GetComponent<BaseHealth>();
        var shootSettings = enemy.enemyPrefab.GetComponent<EnemyBehaviourList>();
        if (health)
        {
            health.onDeath += RemoveEnemy;
        }
        if (shootSettings)
        {
            shootSettings.ShootingStats = enemy.shootingStats;
        }
        enemiesList.Add(enemy);
        if (enemy.GetEnemyMovement())
        {
            GameObject emptyMovement = Instantiate(enemy.GetEnemyMovement().gameObject);
            enemy.SetMovement(emptyMovement.GetComponent<EnemyMovement>());
            enemy.MoveInOut();
        }
        else
        {
            GameObject emptyMovement = new GameObject("Empty Movement");
            EnemyMovement enemyMovement = emptyMovement.AddComponent<EnemyMovement>();
            enemy.SetMovement(enemyMovement);
            enemy.MoveInOut();
            Debug.LogWarning("Missing movement on enemy");
        }
        enemyCountChange?.Invoke(enemiesList.Count);
    }

    public void RemoveEnemy(GameObject target)
    {
        Enemy remove = null;
        foreach (Enemy enemy in enemiesList)
        {
            if (enemy.enemyPrefab != target) continue;
            remove = enemy;
            BaseHealth health;
            
            if (enemy.enemyPrefab.TryGetComponent<BaseHealth>(out health))
            {
                if (health.GetCurrentHP <= 0)
                {
                    KilledEnemiesScore?.Invoke(health.GetScore);                    
                }
            }
            var movement = enemy.GetEnemyMovement();
            if (movement)
            {
                Destroy(movement.gameObject);
            }
            Destroy(enemy.enemyPrefab);
            break;
        }
        if(remove != null)enemiesList.Remove(remove);
        enemyCountChange?.Invoke(enemiesList.Count);
    }

    void RemoveInvalid()
    {
        Enemy remove = null;
        foreach (Enemy enemy in enemiesList)
        {
            if (enemy.enemyPrefab) continue;
            remove = enemy;
            var movement = enemy.GetEnemyMovement();
            if (movement)
            {
                Destroy(movement.gameObject);
            }
            break;
        }
        if(remove != null)enemiesList.Remove(remove);
        enemyCountChange?.Invoke(enemiesList.Count);
    }

    public void KillAllEnemies()
    {
        foreach (Enemy enemy in enemiesList)
        {
            Destroy(enemy.enemyPrefab);
        }
        RemoveInvalid();
        enemiesList.Clear();
        enemyCountChange?.Invoke(enemiesList.Count);
    }
    
    public async void LeaveScreen()
    {
        foreach (Enemy enemy in enemiesList)
        {
            if (!enemy.GetEnemyMovement()) continue;
            enemy.MoveInOut();
        }
        await Awaitable.WaitForSecondsAsync(2f);
        KillAllEnemies();
    }

    public List<Enemy> GetEnemies()
    {
        return enemiesList;
    }

    public int GetEnemyAmount()
    {
        return enemiesList.Count;
    }
    Vector3 RandomVector(Vector3 range)
    {
        float randomX = UnityEngine.Random.Range(range.x, -range.x);
        float randomY = UnityEngine.Random.Range(range.y, -range.y);
        float randomZ = UnityEngine.Random.Range(range.z, -range.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}
