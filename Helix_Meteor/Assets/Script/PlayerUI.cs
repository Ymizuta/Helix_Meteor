using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    public Text speed_text;
    public Text invincible_point_text;

    // Use this for initialization
    void Start () {
        string speed = gameObject.GetComponent<Player>().fall_speed.ToString();
        string i_point = gameObject.GetComponent<Player>().invincible_point.ToString();
        speed_text.text = "速度：" + speed;
        invincible_point_text.text = "無敵化ポイント：" + i_point;
	}
	
	// Update is called once per frame
	void Update () {
        string speed = gameObject.GetComponent<Player>().fall_speed.ToString();
        string i_point = gameObject.GetComponent<Player>().invincible_point.ToString();
        speed_text.text = "速度：" + speed;
        invincible_point_text.text = "無敵化ポイント：" + i_point;
    }
}
