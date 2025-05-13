using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    [Tooltip("Force de déplacement")]
    public float speed = 5f;
    [Tooltip("Liste des joueurs actifs")]
    public List<GameObject> players = new List<GameObject>();

    void Update()
    {
        if (players.Count == 0) return;

        float moveInput = Input.GetAxis("Horizontal");

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            Vector3 moveDirection = new Vector3(moveInput, 0f, 0f) * speed * Time.deltaTime;

            foreach (GameObject player in players)
            {
                if (player != null)
                    player.transform.position += moveDirection;
            }
        }
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
