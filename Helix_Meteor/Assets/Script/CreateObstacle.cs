using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObstacle : MonoBehaviour {

    public GameObject Obstacle;
    Vector3 obstacle_spawn_position;

	// Use this for initialization
	void Start () {
        GameObject newObstacle =
            Instantiate(Obstacle,gameObject.transform.position,gameObject.transform.rotation) as GameObject;
	}	
}
