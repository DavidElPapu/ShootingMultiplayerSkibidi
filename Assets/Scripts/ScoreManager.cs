using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class ScoreManager : NetworkBehaviour
{
	public static ScoreManager singleton;

    private List<Jugador> players = new List<Jugador>();


    private void Awake()
    {
        if (singleton != null && singleton != this) { Destroy(this); } else { singleton = this; }
    }

    public void RegisterPlayer(Jugador elPlayer)
    {
        players.Add(elPlayer);
    }

    public List<(string name, int kills)> GetSortedScore()
    {
        return players.OrderByDescending(p => p.kills).Select(p => (p.name, p.kills)).ToList();
    }
}
