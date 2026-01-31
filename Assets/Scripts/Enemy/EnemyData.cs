using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int health;
    public float damage;
    public float speed;
    public float weight;
    public GameObject visualPrefab; // The 3D model or Sprite
}
