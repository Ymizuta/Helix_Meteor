using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorCamera : MonoBehaviour {

    [SerializeField] Player Player_ = null;      // エディターからアタッチする

    float camera_positionz;
    float distance_from_player = 10;
//    public GameObject player;
    private Vector3 camera_poz;
	
	// Update is called once per frame
	void Update () {
        camera_poz = gameObject.transform.position;
        camera_positionz = Player_.transform.position.z - distance_from_player;
        camera_poz.z = camera_positionz;
        gameObject.transform.position = camera_poz;
	}
}
