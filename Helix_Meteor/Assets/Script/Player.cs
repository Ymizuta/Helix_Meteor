using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] Controller Controller_ = null;         // エディターからアタッチする

    private int default_player_life = 3;                    //プレイヤーのライフ初期値
    public int player_life;                                 //プレイヤーのライフ

    public Vector3 player_poz;                              //プレイヤーの位置
    private float default_fall_speed = 0.5f;                //前方に移動する初期速度
    public float fall_speed;                                //前方に移動する速度
    private float add_speed = 0.1f;                         //時間経過で加算されるプレイヤーの直進速度
    public float reduce_speed = 1.0f;                       //障害物衝突時に減るプレイヤーの直進速度
    const float MIN_SPEED = 0.5f;                           //プレイヤーの直進スピード下限
    const float MAX_SPEED = 2.0f;                           //プレイヤーの直進スピード上限

    //無敵モードポイント
    private bool invincible_flag = false;
    private float defaul_invincible_point = 0f;
    public float invincible_point;
    private float add_invincible_point = 1.0f;
    private float reduced_invincible_point = 1.0f;
    const float MIN_INVINCIBLE_POINT = 0f;
    const float MAX_INVINCIBLE_POINT = 5.0f;
    //無敵時間
    private float default_invincible_time = 0f;
    private float invincible_time;
    private float add_invincible_time = 1.0f;
    const float MAX_INVINCIBLE_TIME = 5.0f;

    //円周上を移動するための変数
    //float player_degree;                                
    //float move_speed = 5f;                               //左右移動の速度
    //float height = 3f;
    //float width = 3f;

    private void Start()
    {
        //ライフ初期化
        player_life = default_player_life;

        //直進スピードの初期化
        fall_speed = default_fall_speed;

        //無敵モード・ポイントの初期化
        invincible_point = defaul_invincible_point;

        //無敵時間の初期化
        invincible_time = default_invincible_time;
    }

    // Update is called once per frame  
    void Update()
    {
        //プレイヤーが直進
        Fall();

        //時間経過でプレイヤーの直進スピードアップ
        fall_speed = SpeedUp();

        //無敵モードの時、一定時間経過で無敵モード解除・無敵モードポイント初期化
        if (invincible_flag)
        {
            //無敵時間を計測
            invincible_time += add_invincible_time * Time.deltaTime;
            //一定時間経過で無敵モード解除
            if (invincible_time >= MAX_INVINCIBLE_TIME)
            {
                //無敵モードフラグ解除
                invincible_flag = false;
                //無敵時間を初期化
                invincible_time = default_invincible_time;
                //無敵モードポイントを初期化
                invincible_point = defaul_invincible_point;
                Debug.Log("無敵モード解除");
            }
        }

        //無敵モードポイント蓄積・一定値を超えると無敵モード
        if (invincible_flag == false)
        {
            //時間経過で無敵モード・ポイントを蓄積
            invincible_point = Charge();
            //無敵モードフラグを立てる
            if (invincible_point >= MAX_INVINCIBLE_POINT) {
                invincible_flag = true;
                Debug.Log("無敵モード！");
                Debug.Log("無敵モードポイント：" + invincible_point);
            }
        }
    }

    //プレイヤーを直進させる関数
    private void Fall()
    {
        player_poz = gameObject.transform.position;
        player_poz.z += fall_speed;
        player_poz.x = gameObject.transform.position.x;
        player_poz.y = gameObject.transform.position.y;
        gameObject.transform.position = player_poz;
    }

    //プレイヤーの上下左右斜め移動関数(スワイプ/マウスドラッグによる操作)
    public void Move(Vector3 move_direction,float move_speed)
    {
        gameObject.transform.position += move_direction * move_speed;
    }

    //時間経過でプレイヤーを加速する関数
    private float SpeedUp()
    {
        fall_speed += add_speed * Time.deltaTime;
        //プレイヤーの速度は上限・下限の範囲で変動
        return Mathf.Clamp(fall_speed, MIN_SPEED, MAX_SPEED);
    }

    //障害物衝突時にプレイヤーを減速する関数
    private float SpeedDown()
    {
        fall_speed -= reduce_speed;
        //プレイヤーの速度は上限・下限の範囲で変動
        return Mathf.Clamp(fall_speed,MIN_SPEED,MAX_SPEED);
    }

    //時間経過で無敵モードポイントを蓄積
    private float Charge()
    {
        invincible_point += add_invincible_point * Time.deltaTime;
        return Mathf.Clamp(invincible_point, MIN_INVINCIBLE_POINT, MAX_INVINCIBLE_POINT);
    }

    //オブジェクトに衝突した際の処理の関数
    private void OnTriggerEnter(Collider other)
    {   
        //無敵モード時は障害物の影響を受けない
        if (invincible_flag)
        {
            Debug.Log("無敵モード中！");
            return;
        }

        Debug.Log("衝突！");
        //障害物に衝突した時に減速
        if (other.gameObject.tag == "Obstacle")
        {
            Debug.Log("減速");
            //減速
            fall_speed = SpeedDown();

            //ライフ減少
            player_life -= 1;
            //ライフが０になるとゲームオーバー
            if(player_life == 0)
            {
                Debug.Log("ライフ０");
                GameObject.Destroy(gameObject);
            }

        }
    }

    //プレイヤーの左右移動関数(矢印キーでの操作)
    //public void Move(Controller.Direction direction)
    //{
    //    switch (direction)
    //    {
    //        //右に移動
    //        case Controller.Direction.Right:
    //            player_degree += move_speed;
    //            break;
    //        //左に移動
    //        case Controller.Direction.Left:
    //            player_degree -= move_speed;
    //            break;
    //        default:
    //            break;
    //    }
    //    player_poz.x = Mathf.Cos(player_degree * Mathf.Deg2Rad) * width;
    //    player_poz.y = Mathf.Sin(player_degree * Mathf.Deg2Rad) * height;
    //    gameObject.transform.position = new Vector3(player_poz.x, player_poz.y, player_poz.z);
    //}
}