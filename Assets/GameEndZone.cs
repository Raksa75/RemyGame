using UnityEngine;

public class GameEndZone : MonoBehaviour
{
    [Tooltip("UI de fin de partie")]
    public GameObject gameOverUI;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[GameEndZone] D�tection d'un objet : {other.gameObject.name}, Tag : {other.tag}");

        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"[GameEndZone] Un ennemi ({other.gameObject.name}) est entr� dans la zone critique ! Fin du jeu.");
            EndGame();
        }
    }

    void EndGame()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Time.timeScale = 0f; // Arr�t du jeu
        Debug.Log("[GameEndZone] Le jeu est maintenant stopp�.");
    }
}
