using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public class WaveSettings
{
    [HideInInspector] public string myName;
    public float timeForWave;
    [SerializeField] public List<SpawnZone> spawnPoints = new List<SpawnZone>();
    public GameObject wavePrefab { get; set; }

    public WaveSettings(string nameToSet, GameObject prefab)
    {
        myName = nameToSet;
        timeForWave = 10;
        wavePrefab = prefab;
        foreach (Transform point in wavePrefab.transform)
        {
            spawnPoints.Add(new SpawnZone(point.name, point.position));
        }
    }

    public void NewPoints(Transform main)
    {
        int index = 0;
        foreach (Transform location in main)
        {
            spawnPoints[index].SetLocation(location.position);
            index++;
        }
    }
    public override string ToString()
    {
        return myName;
    }
    public void ValidateContent()
    {
        if (!wavePrefab)
        {
            Debug.LogWarning("Missing wavePrefab");
            return;
        }
        foreach (Transform child in wavePrefab.transform)
        {
            SpawnZone spawn = Find(child.name);
            if (spawn == null)
            {
                spawnPoints.Add(new SpawnZone(child.name, child.position));
                continue;
            }
            if (spawn.location != child.position)
            {
                spawn.location = child.position;
            }
        }
        foreach (SpawnZone spawnZone in spawnPoints.ToArray())
        {
            bool contains = false;
            foreach (Transform child in wavePrefab.transform)
            {
                if (child.name == spawnZone.myName)
                {
                    contains = true;
                    break;
                }
            }
            if (!contains)
            {
                spawnPoints.Remove(spawnZone);
            }
        }
    }

    public bool Contains(string settingName)
    {
        foreach (SpawnZone spawn in spawnPoints)
        {
            if (spawn.myName == settingName)
                return true;
        }
        return false;
    }

    SpawnZone Find(string settingName) {
        foreach (SpawnZone spawn in spawnPoints)
        {
            if (spawn.myName == settingName)
                return spawn;
        }
        return null;
    }
}
[System.Serializable]
public class SpawnZone
{
    [HideInInspector] public string myName = "Position";
    [HideInInspector] public Vector3 location;
    [SerializeField] public List<Enemy> enemies = new List<Enemy>();

    public SpawnZone(string zoneName, Vector3 location)
    {
        this.location = location;
        myName = zoneName;
    }

    public void SetLocation(Vector3 location)
    {
        this.location = location;
    }
}