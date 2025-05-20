using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class TeamManager : NetworkBehaviour
{
    public static TeamManager singleton;

    public List<Jugador> alphaTeam = new List<Jugador>();
    public List<Jugador> betaTeam = new List<Jugador>();

    private void Awake()
    {
        if (singleton != null && singleton != this) { Destroy(this); } else { singleton = this; }

    }
    public Teams GetBalancedTeam()
    {
        return betaTeam.Count > alphaTeam.Count ? Teams.Alpha : Teams.Beta;
    }

    public void RegisterPlayer(Jugador player, Teams elTeam)
    {
        if (elTeam == Teams.Alpha)
        {
            alphaTeam.Add(player);
        }
        else
        {
            betaTeam.Add(player);
        }
    }
}

public enum Teams
{
    Alpha,
    Beta,
    None
}
