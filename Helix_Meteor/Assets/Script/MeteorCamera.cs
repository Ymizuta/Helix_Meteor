using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorCamera : MonoBehaviour {

    [SerializeField] Player Player_ = null;         // エディターからアタッチする
    [SerializeField] GameObject player_clone_ = null;   //エディターからアタッチする(nullチェック用)

    private Vector3 default_camera_position = new Vector3(0,1,-10);
    float camera_positionz;
    float distance_from_player = 30;
    private Vector3 camera_poz;

    //カメラ振動用（テスト用）
    const float MAX_SHAKE_RANGE_X = +0.5f;
    const float MIN_SHAKE_RANGE_X = -0.5f;
    const float MAX_SHAKE_RANGE_Y = +0.5f;
    const float MIN_SHAKE_RANGE_Y = -0.5f;
    private float shake_range_x;
    private float shake_range_y;

    // Update is called once per frame
    void Update () {
        //nullcheck?
        if (player_clone_ == null)
        {
            return;
        }
        //プレイヤーに追従
        camera_poz = gameObject.transform.position;
//        camera_positionz = Player_.transform.position.z - distance_from_player;
        camera_positionz = player_clone_.transform.position.z - distance_from_player;
        camera_poz.z = camera_positionz;
        ////上下左右に振動
        //shake_range_x = Random.Range(MIN_SHAKE_RANGE_X,MAX_SHAKE_RANGE_X);
        //shake_range_y = Random.Range(MIN_SHAKE_RANGE_Y,MAX_SHAKE_RANGE_Y);
        //camera_poz.x = shake_range_x;
        //camera_poz.y = shake_range_y;
        //カメラ位置を指定
        gameObject.transform.position = camera_poz;
    }
    //カメラを振動させる
    public void Shake_Camera()
    {
        //プレイヤーに追従
        camera_poz = gameObject.transform.position;
        camera_positionz = Player_.transform.position.z - distance_from_player;
        camera_poz.z = camera_positionz;
        //上下左右に振動
        shake_range_x = Random.Range(MIN_SHAKE_RANGE_X, MAX_SHAKE_RANGE_X);
        shake_range_y = Random.Range(MIN_SHAKE_RANGE_Y, MAX_SHAKE_RANGE_Y);
        camera_poz.x = shake_range_x;
        camera_poz.y = shake_range_y;
        //カメラ位置を指定
        gameObject.transform.position = camera_poz;
    }
    //位置初期化
    public void SetDefaultPosition()
    {
        Debug.Log("カメラデフォルト！");
        gameObject.transform.position = default_camera_position;
        Debug.Log(default_camera_position);
    }

    public GameObject PlayerClone
    {
        set
        {
            player_clone_ = value;
        }
    }

}
