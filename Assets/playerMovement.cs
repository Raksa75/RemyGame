using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Mouvement horizontal")]
    [Tooltip("Vitesse de déplacement du joueur sur l'axe X")]
    public float speed = 5f;

    private CharacterController cc;

    void Start()
    {
        // Récupère le CharacterController
        cc = GetComponent<CharacterController>();

        // Verrouille et cache le curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Récupère uniquement l'axe horizontal (A/D ou Flèches Gauche/Droite)
        float h = Input.GetAxis("Horizontal");

        // Prépare un déplacement sur X seulement
        Vector3 move = new Vector3(h, 0f, 0f);

        // Si le joueur appuie suffisamment, on déplace
        if (Mathf.Abs(h) > 0.1f)
        {
            cc.Move(move.normalized * speed * Time.deltaTime);
        }
        // ATTENTION : on ne met PAS transform.forward = …, 
        // le personnage garde toujours son orientation initiale (vers +Z)
    }
}
