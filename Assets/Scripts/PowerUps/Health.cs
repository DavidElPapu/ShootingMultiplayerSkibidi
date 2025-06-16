using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Health : NetworkBehaviour
{
    public int healing = 5;

    [SyncVar(hook = nameof(ActiveChanged))]
    public bool isActive = true;

    private PowerUpSpawner powerUpSpawner;


    private void ActiveChanged(bool old, bool newActive)
    {
        gameObject.SetActive(newActive);
    }

    [Server]
    public void Heal(Jugador player)
    {
        player.IncreaseHealth(healing);
    }

    public void Initialize(PowerUpSpawner spawner)
    {
        powerUpSpawner = spawner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        if (other.gameObject.TryGetComponent<Jugador>(out Jugador player))
        {
            Heal(player);
            powerUpSpawner.StartCoroutine(powerUpSpawner.CollectPowerUp());
        }
    }
}
