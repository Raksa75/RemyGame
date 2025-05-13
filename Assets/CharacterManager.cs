using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; } // Singleton

    // Liste de tous les personnages actifs
    private List<GameObject> characters = new List<GameObject>();

    public List<GameObject> Characters => characters; // Ajoute cette propriété

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (characters.Count == 0)
        {
            GameOver();
        }
    }

    public void RegisterCharacter(GameObject chara)
    {
        characters.Add(chara);
    }

    public void RemoveCharacter(GameObject chara)
    {
        characters.Remove(chara);
    }

    void GameOver()
    {
        Debug.Log("Game Over - Plus aucun personnage en jeu !");
    }
}
