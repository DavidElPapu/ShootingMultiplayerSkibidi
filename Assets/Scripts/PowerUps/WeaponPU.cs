using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponPU : APickUp
{
    public WeaponObject weaponType;

    [Server]
    public override void TakeEffect(Jugador player)
    {
        base.TakeEffect(player);
        player.currentWeapon = weaponType.ToData();
    }
}
