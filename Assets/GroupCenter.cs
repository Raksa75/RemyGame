using UnityEngine;

public class GroupCenter : MonoBehaviour
{
    void Update()
    {
        if (CharacterManager.Instance == null || CharacterManager.Instance.Characters.Count == 0)
            return;

        // Calcul du centre moyen des personnages
        Vector3 center = Vector3.zero;
        foreach (var chara in CharacterManager.Instance.Characters)
        {
            center += chara.transform.position;
        }
        center /= CharacterManager.Instance.Characters.Count;

        // Déplace cet objet au centre du groupe
        transform.position = center;
    }
}
