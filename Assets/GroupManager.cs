using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    [Tooltip("Liste des joueurs actifs")]
    public List<GameObject> players = new List<GameObject>();

    [Tooltip("UI de fin de partie (optionnel)")]
    public GameObject gameOverUI;

    void Update()
    {
        CheckGameOver(); // Vérifie si tous les joueurs sont morts
    }

    void CheckGameOver()
    {
        if (players.Count == 0) // Aucun joueur restant ?
        {
            Debug.Log("[GroupManager] Plus de joueurs en vie. Fin du jeu !");
            EndGame();
        }
    }

    void EndGame()
    {
        // Activer l'écran de fin si disponible
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // Arrêter complètement le jeu
        Time.timeScale = 0f;
    }

    public void RegisterPlayer(GameObject newPlayer)
    {
        if (!players.Contains(newPlayer))
            players.Add(newPlayer);
    }

    public void RemovePlayer(GameObject player)
    {
        players.Remove(player);
    }
}
