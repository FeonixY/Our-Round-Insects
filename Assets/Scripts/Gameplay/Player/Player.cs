using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    public PlayerMovement PlayerMovement { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        PlayerMovement = GetComponent<PlayerMovement>();
    }
}
