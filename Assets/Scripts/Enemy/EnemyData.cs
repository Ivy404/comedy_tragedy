using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Comedy Tragedy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int health;
    public float damage;
    public float speed;
    public float mass;
    public int xp;
    public GameObject visualPrefab; // The 3D model or Sprite
}
