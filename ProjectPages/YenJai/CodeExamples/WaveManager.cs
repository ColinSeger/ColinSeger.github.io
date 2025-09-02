using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class WaveManager : MonoBehaviour
{
    public UnityEvent winningEvent;
    [SerializeField] List<GameObject> waves = new List<GameObject>();

    [SerializeField]List<WaveSettings> waveSettings = new List<WaveSettings>();
    float endTime = float.PositiveInfinity;
    int currentWave = 0;
    void Start()
    {
        Recreate();
        Trigger();

        if (!Application.isPlaying) return;

        EnemyManager.Instance.enemyCountChange += NextWave;
    }
    void NextWave(int numOfEnemies)
    {
        if (numOfEnemies > 0) return;
        Trigger();
    }
    public void Trigger()
    {
        if (!Application.isPlaying) return;

        if (currentWave > waveSettings.Count - 1)
        {
            winningEvent.Invoke();
            Debug.Log("YOU WIN, YAY!!!!");
            return;
        }

        foreach (var spawnPoint in waveSettings[currentWave].spawnPoints)
        {
            if (spawnPoint.enemies.Count <= 0 || !spawnPoint.enemies[0].enemyPrefab) continue;
            EnemyManager.Instance.SpawnAtLocation(spawnPoint.enemies[0], spawnPoint.location);
        }
        endTime = waveSettings[currentWave].timeForWave + Time.time;
        currentWave++;
    }
    void Update()
    {
        if (Time.time > endTime)
        {
            endTime = float.PositiveInfinity;
            EnemyManager.Instance.LeaveScreen();
        }
#if UNITY_EDITOR
        Recreate();
        foreach (var item in waves)
        {
            if(!item) continue;
            if (!item.transform.hasChanged) continue;

        }
#endif
    }
    void Recreate()
    {
        waves.Clear();
        waveSettings.Clear();
        foreach (Transform child in this.transform)
        {
            waves.Add(child.gameObject);
            WaveSettings newWave = child.GetComponent<SpawnPointVisualizer>().GetWaveSettings();
            newWave.myName = newWave.ToString();
            waveSettings.Add(newWave);
        }
    }

    bool Contains(string nameOf)
    {
        foreach (WaveSettings wave in waveSettings)
        {
            if (wave.myName == nameOf) return true;
        }
        return false;
    }
    void Validate()
    {
        string[] names = new string[waves.Count];
        for (int i = 0; i < waves.Count; i++)
        {
            names[i] = waves[i].name;
        }
        foreach (WaveSettings wave in waveSettings.ToList())
        {
            if (!names.Contains(wave.myName))
            {
                // waveSettings.Remove(wave);
            }
        }
        waveSettings = waveSettings.Distinct().ToList();
    }
}
