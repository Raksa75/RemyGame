using UnityEngine;
using UnityEngine.SceneManagement; // Import nécessaire

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene"); // Mets le nom exact de ta scène de jeu
        print("caca");
    }
}
