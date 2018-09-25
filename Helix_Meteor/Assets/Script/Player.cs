using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] Controller Controller_ = null;     // エディターからアタッチする

    public Vector3 player_poz;                          //プレイヤーの位置
    public float default_fall_speed = 0.5f;             //前方に移動する初期速度
    public float fall_speed;                            //前方に移動する速度
    public float add_speed = 0.01f;                     //時間経過で加算される速度

    //円周上を移動するための変数
    float player_degree;                                
    float move_speed = 5f;                              //左右移動の速度
    float height = 3f;
    float width = 3f;

    private void Start()
    {
        //直進スピードの初期値を設定
        fall_speed = default_fall_speed;
    }

    // Update is called once per frame  
    void Update()
    {
        //直進
        player_poz = gameObject.transform.position;
        player_poz.z += fall_speed;
        player_poz.x = gameObject.transform.position.x;
        player_poz.y = gameObject.transform.position.y;
        gameObject.transform.position = player_poz;

        //時間経過でスピードアップ
        fall_speed = SpeedUp();
    }

    //プレイヤーの左右移動関数
    public void Move(Controller.Direction direction)
    {
        switch (direction)
        {
            //右に移動
            case Controller.Direction.Right:
                player_degree += move_speed;
                break;
            //左に移動
            case Controller.Direction.Left:
                player_degree -= move_speed;
                break;
            default:
                break;
        }
        player_poz.x = Mathf.Cos(player_degree * Mathf.Deg2Rad) * width;
        player_poz.y = Mathf.Sin(player_degree * Mathf.Deg2Rad) * height;
        gameObject.transform.position = new Vector3(player_poz.x, player_poz.y, player_poz.z);
    }
    
    //時間経過でプレイヤーを加速する関数
    public float SpeedUp()
    {
        fall_speed += add_speed * Time.deltaTime;
        return fall_speed;
    }

    //オブジェクトに衝突した際の処理関数
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("衝突！");
        //障害物に衝突した時に原則する
        if (other.gameObject.tag == "Obstacle")
        {
            Debug.Log("減速");
            //直進スピードを初期化
            fall_speed = default_fall_speed;
        }
    }
}