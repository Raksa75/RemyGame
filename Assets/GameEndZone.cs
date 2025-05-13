using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndZone : MonoBehaviour
{
    [Tooltip("UI de fin de partie")]
    public GameObject gameOverUI;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[GameEndZone] Détection d'un objet : {other.gameObject.name}, Tag : {other.tag}");

        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"[GameEndZone] Un ennemi ({other.gameObject.name}) est entré dans la zone critique ! Fin du jeu.");
            EndGame();
        }
    }

    void EndGame()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        SceneManager.LoadScene("Menu"); // Mets le nom exact de ta scène de jeu
        Debug.Log("[GameEndZone] Le jeu est maintenant stoppé.");
    }
}
