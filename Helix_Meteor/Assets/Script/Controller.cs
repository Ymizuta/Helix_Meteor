using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller : MonoBehaviour {

    [SerializeField] Player player_ = null; // エディターからアタッチする
    [SerializeField] Camera meteor_camera_ = null; // エディターからアタッチする

    public enum DIRECTION
    {
        Right,
        Left
    }
	
	// Update is called once per frame
	void Update () {

        //左右入力取得
        DIRECTION direction;
        //右入力
        if (Input.GetAxis("Horizontal") > 0)
        {
            direction = DIRECTION.Right;
            player_.Move(direction);
        }
        else
        //左入力
        if (Input.GetAxis("Horizontal") < 0)
        {
            direction = DIRECTION.Left;
            player_.Move(direction);
        }
    }
}
