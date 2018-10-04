using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller : MonoBehaviour {

    [SerializeField] Player player_ = null; // エディターからアタッチ
    [SerializeField] MeteorCamera meteor_camera_ = null; // エディターからアタッチ
    [SerializeField] GameObject PlayerObj = null;

    private Vector3 touch_poz;
    private Vector3 old_player_poz;                 //前フレームでのタッチ位置（スワイプによる上下左右移動処理用）
    private Vector3 new_player_poz;                 //現在フレームでのタッチ位置（スワイプによる上下左右移動処理用）
    private Vector3 move_direction;                 //上下左右移動の移動方向（Player_.Move関数の引数）
    private float move_speed = 10.0f;               //上下左右移動のスピード調整用の値（Player_.Move関数の引数）
    private int default_play_time =1;
    private int play_time;
//    float lifetime = 0.1f;


    //円状移動のための列挙型
    //public enum Direction
    //{
    //    Right,
    //    Left
    //}

    // Update is called once per frame
    void Update () {
        if (PlayerObj == null)
        {
            return;
        }

        //タッチ操作
        TouchInfo info = AppUtil.GetTouch();
        if (info == TouchInfo.Began)
        {
            //スワイプによる移動処理のためタッチ位置を取得
            touch_poz = AppUtil.GetTouchPosition();
            touch_poz.z = 1.0f;                                         //z座標は扱いは別途調整
            old_player_poz = Camera.main.ScreenToWorldPoint(touch_poz);
            old_player_poz.z = 0f;
        }
        else
        if (info == TouchInfo.Moved)
        {
            //mouse_pozにタッチしている位置を返す
            touch_poz = AppUtil.GetTouchPosition();
            //ScreenToWorldPointによる不具合防止のためZ座標を指定
            touch_poz.z = 1.0f;
            //スクリーン座標をワールド座標に変換
            new_player_poz = Camera.main.ScreenToWorldPoint(touch_poz);
            //Z座標は移動させないよう初期化
            new_player_poz.z = 0f;
            //移動のためのベクトルを取得
            move_direction = new_player_poz - old_player_poz;

            //プレイヤー上下左右移動
//            player_.Move(move_direction,move_speed);
            PlayerObj.GetComponent<Player>().Move(move_direction, move_speed);

            //次フレームでの移動処理のためold_player_pozに現在のフレームのタッチ位置(new_player_poz)を格納
            old_player_poz = new_player_poz;
        }
        else
        if (info == TouchInfo.Ended)
        {
        }

        CountTime();

        //画面振動させる処理
        //if (player_.Player_life < 3)
        //{
        //    if (lifetime > 0) {
        //        meteor_camera_.Shake_Camera();
        //        lifetime -= Time.deltaTime;
        //    }
        //}

        //左右入力取得（キーボード操作・円状移動）
        //Direction direction;
        ////右入力
        //if (Input.GetAxis("Horizontal") > 0)
        //{
        //    direction = Direction.Right;
        //    player_.Move(direction);
        //}
        //else
        ////左入力
        //if (Input.GetAxis("Horizontal") < 0)
        //{
        //    direction = Direction.Left;
        //    player_.Move(direction);
        //}
    }

    private void CountTime()
    {
        float temp_play_time = Time.time;
        play_time = (int)temp_play_time;
//        Debug.Log(play_time);
    }

    public int Play_time
    {
        get
        {
            return play_time;
        }
    }
    //セッター
    public GameObject PlayerObjProp
    {
        set
        {
            PlayerObj = value;
        }
    }
}
