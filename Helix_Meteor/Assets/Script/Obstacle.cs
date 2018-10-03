using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    private float rotation_speed_x = -60;

	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(rotation_speed_x, 0, 0) * Time.deltaTime, Space.World);
    }
}
