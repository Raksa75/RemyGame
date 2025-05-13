using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    [Tooltip("Force de déplacement du groupe")]
    public float speed = 5f;
    [Tooltip("Force qui attire les clones vers le centre")]
    public float cohesionStrength = 2f;
    [Tooltip("Liste des joueurs actifs")]
    public List<GameObject> players = new List<GameObject>();

    void Update()
    {
        if (players.Count == 0) return;

        float moveInput = Input.GetAxis("Horizontal");
        Vector3 moveDirection = new Vector3(moveInput, 0f, 0f) * speed * Time.deltaTime;

        foreach (GameObject player in players)
        {
            if (player != null)
            {
                player.transform.position += moveDirection;

                // Ajout de la force de réorganisation
                ApplyCohesion(player);
            }
        }
    }

    void ApplyCohesion(GameObject player)
    {
        if (players.Count < 2) return;

        Vector3 center = Vector3.zero;
        foreach (var p in players)
        {
            if (p != null)
                center += p.transform.position;
        }
        center /= players.Count;

        Vector3 cohesionForce = (center - player.transform.position).normalized * cohesionStrength * Time.deltaTime;

        player.transform.position += cohesionForce;
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
