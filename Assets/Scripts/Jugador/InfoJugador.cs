using UnityEngine;

public class InfoJugador : MonoBehaviour
{
    private string usuario;
    private int sombrero;

    private string[] adj = {"Pixelated", "Salty", "Epic", "Chonky", "Sweaty", "Cracked", "Laggy", "Turbo","Yeet", "Shadow", "Dank", "Nerfed", "Buffed", "Ragey", "Greedy", "NoScope", "Saucy", "Looted", "404", "Speedy", "Cursed", "Viral"};
    private string[] noun = {"Wizard", "Banana", "Goblin", "Sniper", "Dragon", "Hamster", "Glitch", "Doge", "Nugget", "Mage", "Bronnie", "Waffle", "Zombie", "Lootbox", "Knight", "Capybara", "Orc", "NPC", "Speedrunner", "Duck", "Looter", "Boss", "Skeleton", "Gremlin", "Crab"};

    public void AsignarUsuario(string texto)
    {
        usuario = texto;
    }

    public string PideUsuario()
    {
        if (string.IsNullOrEmpty(usuario))
        {
            return GeneraAleatorio();
        } else
        {
            return usuario;
        }
    }
    private string GeneraAleatorio()
    {
        string a = adj[Random.Range(0, adj.Length)];
        string n = noun[Random.Range(0, noun.Length)];
        string c = Random.Range(10, 100).ToString();
        return $"{a}{n}{c}";
    }

    public void AsignarSombrero(int numero)
    {
        sombrero = numero;
    }

    public int PideSombrero()
    {
        if (sombrero < 0 || sombrero > 5) 
        {
            return 0;
        }
        else
        {
            return sombrero;
        }
    }
}
