using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Comedy Tragedy/WaveData")]
public class WaveData : ScriptableObject
{
    public string waveName;
    public SpawnerData[] spawnersInWave; // Which types appear in this wave
    public int totalToSpawn;          // Total count before wave ends
    public float spawnRate;           // Time between spawns
    public int nextWaveDelay;
}