using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {

    float camera_positionz;
    float distance_from_player = 10;
    public GameObject player;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        camera_positionz = player.transform.position.z - distance_from_player;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y,camera_positionz);
	}
}
