using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class BasicBullet : ABullet
{
    private void OnCollisionEnter(Collision collision)
    {
        HasHit(collision);
    }

    public override void HasHit(Collision col)
    {
        if (col.collider.gameObject.TryGetComponent(out Jugador player))
        {
            player.TakeDamage(damage, ownerTeam);
        }
        base.HasHit(col);
    }
}
