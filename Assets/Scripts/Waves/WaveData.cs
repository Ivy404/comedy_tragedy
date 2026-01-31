using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "RogueLike/Wave")]
public class WaveData : ScriptableObject
{
    public string waveName;
    public EnemyData[] enemiesInWave; // Which types appear in this wave
    public int totalToSpawn;          // Total count before wave ends
    public float spawnRate;           // Time between spawns
}