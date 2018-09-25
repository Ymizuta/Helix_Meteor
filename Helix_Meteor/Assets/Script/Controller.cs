using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller : MonoBehaviour {

    [SerializeField] Player player_ = null; // エディターからアタッチする
//    [SerializeField] Camera meteor_camera_ = null; // エディターからアタッチする

    //serializefieldでうまくアタッチできないため暫定処置
    public Camera meteor_camera;

    private Vector3 new_player_poz;
    private Vector3 old_player_poz;

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

        //タッチ操作
        TouchInfo info = AppUtil.GetTouch();
        if (info == TouchInfo.Began)
        {
//            Debug.Log("Begun!");

            Vector3 mouse_poz = AppUtil.GetTouchPosition();
            mouse_poz.z = 10.0f;
            old_player_poz = Camera.main.ScreenToWorldPoint(mouse_poz);
        }
        else
        if (info == TouchInfo.Moved)
        {
            Debug.Log("Move!");
            Vector3 mouse_poz = AppUtil.GetTouchPosition();
            mouse_poz.z = 10.0f;
            new_player_poz = Camera.main.ScreenToWorldPoint(mouse_poz);

 //           Debug.Log("old:" + old_player_poz + "new" + new_player_poz);

            Vector3 move_direction = new_player_poz - old_player_poz;
            move_direction.z = 0;
 //           Debug.Log(move_direction);
            player_.transform.position += move_direction;

            old_player_poz = new_player_poz;
        }
        else
        if (info == TouchInfo.Ended)
        {
//            Debug.Log("Ended");
        }
    }
}
