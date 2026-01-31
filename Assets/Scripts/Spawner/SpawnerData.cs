using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerData", menuName = "Comedy Tragedy/SpawnerData")]
public class SpawnerData : ScriptableObject
{
    public string spawnerName;
    public EnemyData[] enemiesInWave; // Which types appear in this wave
    public int[] enemyWeights;        // Weight of the enemies
}
