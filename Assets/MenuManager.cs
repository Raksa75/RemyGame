using UnityEngine;
using UnityEngine.SceneManagement; // Import n�cessaire

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene"); // Mets le nom exact de ta sc�ne de jeu
        print("caca");
    }
}
