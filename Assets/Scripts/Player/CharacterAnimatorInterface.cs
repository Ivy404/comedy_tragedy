using UnityEngine;

public class CharacterAnimatorInterface : MonoBehaviour
{
    [SerializeField] public PlayerActions player;

    public void dealDmg()
    {
        player.performDmg();
    }
}
