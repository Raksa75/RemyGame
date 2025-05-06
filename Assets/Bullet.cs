using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Tooltip("Dur�e de vie de la balle avant auto-destruction")]
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision coll)
    {
        // Ici tu peux ajouter un effet d�impact, du son, etc.
        Destroy(gameObject);
    }
}
