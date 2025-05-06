using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Mouvement horizontal")]
    [Tooltip("Vitesse de d�placement du joueur sur l'axe X")]
    public float speed = 5f;

    private CharacterController cc;

    void Start()
    {
        // R�cup�re le CharacterController
        cc = GetComponent<CharacterController>();

        // Verrouille et cache le curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // R�cup�re uniquement l'axe horizontal (A/D ou Fl�ches Gauche/Droite)
        float h = Input.GetAxis("Horizontal");

        // Pr�pare un d�placement sur X seulement
        Vector3 move = new Vector3(h, 0f, 0f);

        // Si le joueur appuie suffisamment, on d�place
        if (Mathf.Abs(h) > 0.1f)
        {
            cc.Move(move.normalized * speed * Time.deltaTime);
        }
        // ATTENTION : on ne met PAS transform.forward = �, 
        // le personnage garde toujours son orientation initiale (vers +Z)
    }
}
