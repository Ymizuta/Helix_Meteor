using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller : MonoBehaviour {

    [SerializeField] Player player_ = null; // エディターからアタッチする
    [SerializeField] Camera meteor_camera_ = null; // エディターからアタッチする

    public enum Direction
    {
        Right,
        Left
    }
	
	// Update is called once per frame
	void Update () {

        //左右入力取得
        //アップデート内にDirection direction;はいらないのでは
        //スワイプにしたとき、角度と値を渡すことになる
        Direction direction;
        //右入力
        if (Input.GetAxis("Horizontal") > 0)
        {
            direction = Direction.Right;
            player_.Move(direction);
        }
        else
        //左入力
        if (Input.GetAxis("Horizontal") < 0)
        {
            direction = Direction.Left;
            player_.Move(direction);
        }
    }
}
