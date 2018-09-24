using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToStartPotision : MonoBehaviour {

    public GameObject player_;

    private void OnTriggerEnter(Collider other)
    {
        Vector3 Player_position = player_.transform.position;
        player_.GetComponent<Player>().player_posz = 0;
        player_.transform.position = new Vector3(Player_position.x, Player_position.y,0);
        Debug.Log(other.gameObject);
    }

}
