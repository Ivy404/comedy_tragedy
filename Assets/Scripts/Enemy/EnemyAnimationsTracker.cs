using UnityEngine;

public class EnemyAnimationsTracker : MonoBehaviour
{
    public EnemyController enemyRef;

    void SetAttackingStart()
    {
        if(enemyRef != null)
        {
            enemyRef.Attacking = true;
        }
    }

    void SetAttackingEnd()
    {
        if(enemyRef != null)
        {
            enemyRef.Attacking = false;
        }
    }

    void DealDamage()
    {
        if(enemyRef != null)
        {
            enemyRef.TryDamagePlayer();
        }
    }
}
