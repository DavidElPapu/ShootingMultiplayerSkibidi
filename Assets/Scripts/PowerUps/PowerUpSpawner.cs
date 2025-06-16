using System.Collections;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class PowerUpSpawner : NetworkBehaviour
{
    public float spawnTime = 5;

    public PowerType myType;

    public enum PowerType
    {
        Health = 0,
        Weapon = 1
    }

    public GameObject[] powerUps = new GameObject[2];

    private GameObject spawned;

    public override void OnStartServer()
    {
        base.OnStartServer();
        CmdSpawn();
    }

    [Server]
    private void CmdSpawn()
    {
        spawned = Instantiate(powerUps[(int)myType], transform.position, Quaternion.identity);
        spawned.GetComponent<APickUp>().Initialize(this);
        NetworkServer.Spawn(spawned);
    }

    public IEnumerator CollectPowerUp()
    {
        var foo = spawned.GetComponent<APickUp>();
        foo.isActive = false;

        yield return new WaitForSeconds(spawnTime);

        foo.isActive = true;
    }
}