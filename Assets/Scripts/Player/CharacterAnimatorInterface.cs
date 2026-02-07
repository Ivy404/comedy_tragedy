using UnityEngine;

public class CharacterAnimatorInterface : MonoBehaviour
{
    [SerializeField] public PlayerActions player;

    public void dealDmg()
    {
        player.swordPerformDmg();
    }
    public void stopDmg()
    {
        player.swordStopDmg();
    }
}
