using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    [HideInInspector]
    public PlayerMovement PlayerMovement;

    private void Start()
    {
        PlayerMovement = GetComponent<PlayerMovement>();
    }
}
