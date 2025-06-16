using UnityEngine;
using Mirror;

public abstract class APickUp : NetworkBehaviour
{
    public PowerUpSpawner spawner;
    [SyncVar(hook = (nameof(ActiveChanged)))]
    public bool isActive = true;


    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        if (other.gameObject.TryGetComponent(out Jugador jugador))
        {
            TakeEffect(jugador);
        }
    }

    public void Initialize(PowerUpSpawner spawner)
    {
        this.spawner = spawner;
    }

    [Server]
    public virtual void TakeEffect(Jugador player)
    {
        spawner.StartCoroutine(nameof(PowerUpSpawner.CollectPowerUp));
    }

    private void ActiveChanged(bool oldActive, bool newActive)
    {
        gameObject.SetActive(newActive);
    }
}
