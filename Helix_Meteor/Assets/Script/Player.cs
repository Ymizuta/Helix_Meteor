using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] Controller Controller_ = null;         //エディターからアタッチする

    //プレイヤーの位置に関する変数
    public Vector3 player_poz;                              //プレイヤーの位置
    const float MIN_X_POSITION = -3;
    const float MAX_X_POSITION = 3;
    const float MIN_Y_POSITION = -3;
    const float MAX_Y_POSITION = 3;
    //プレイヤーのライフに関する変数
    private int default_player_life = 3;                    //プレイヤーのライフ初期値
    private int player_life;                                 //プレイヤーのライフ
    //プレイヤーの速度に関する変数
    private float default_fall_speed = 0.5f;                //前方に移動する初期速度
    private float fall_speed;                                //前方に移動する速度
    private float add_speed = 0.1f;                         //時間経過で加算されるプレイヤーの直進速度
    private float reduce_speed = 1.0f;                       //障害物衝突時に減るプレイヤーの直進速度
    const float MIN_SPEED = 0.5f;                           //プレイヤーの直進スピード下限
    const float MAX_SPEED = 2.0f;                           //プレイヤーの直進スピード上限
    //無敵モードポイントに関する変数
    private bool invincible_flag = false;                   //無敵モードフラグ
    private float defaul_invincible_point = 0f;             //無敵モードポイントの初期値
    private float invincible_point;                          //無敵モードポイント（累積）
    private float add_invincible_point = 1.0f;              //加算される無敵モードポイント
    private float reduced_invincible_point = 1.0f;          //減じられる無敵モードポイント
    const float MIN_INVINCIBLE_POINT = 0f;                  //無敵モードポイントの下限値
    const float MAX_INVINCIBLE_POINT = 5.0f;                //無敵モードポイントの上限値
    //無敵時間に関する変数
    private float default_invincible_time = 0f;             //無敵モード中の経過時間の初期値
    private float invincible_time;                          //無敵モード中の経過時間（累積）
    private float add_invincible_time = 1.0f;               //加算される無敵モード中の経過時間
    const float MAX_INVINCIBLE_TIME = 5.0f;                 //無敵モード時間の上限

    //演出
    [SerializeField] ParticleSystem pt_red_fire = null;     //プレイヤーのエフェクト（通常時）
//    public ParticleSystem pt_blue_fire;                   //プレイヤーのエフェクト（無敵モード時）
    [SerializeField] GameObject Jet_Effect = null;          //プレイヤーのエフェクト（無敵モード時）
    [SerializeField] GameObject explosion_effect = null;

    //円周上を移動するための変数
    //float player_degree;                                
    //float move_speed = 5f;                               //左右移動の速度
    //float height = 3f;
    //float width = 3f;

    private void Start()
    {
        //プレイヤー位置格納
        player_poz = gameObject.transform.position;
        
        //ライフ初期化
        player_life = default_player_life;

        //直進スピードの初期化
        fall_speed = default_fall_speed;

        //無敵モード・ポイントの初期化
        invincible_point = defaul_invincible_point;

        //無敵時間の初期化
        invincible_time = default_invincible_time;

        //プレイヤーのエフェクト初期化
        InvincibleEffectOff();
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
                //プレイヤーのエフェクト初期化
                InvincibleEffectOff();
            }
        }

        //通常モードの時、無敵モードポイント累積・一定値を超えると無敵モード
        if (invincible_flag == false)
        {
            //時間経過で無敵モード・ポイントを累積
            invincible_point = Charge();
            //無敵モードフラグを立てる
            if (invincible_point >= MAX_INVINCIBLE_POINT) {
                invincible_flag = true;
                Debug.Log("無敵モード！");
                Debug.Log("無敵モードポイント：" + invincible_point);
                //プレイヤーのエフェクト変化
                InvincibleEffectOn();
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
        Vector3 temp_player_poz = gameObject.transform.position;
        temp_player_poz += move_direction * move_speed;
        temp_player_poz.x = Mathf.Clamp(temp_player_poz.x,MIN_X_POSITION,MAX_X_POSITION);
        temp_player_poz.y = Mathf.Clamp(temp_player_poz.y, MIN_Y_POSITION, MAX_Y_POSITION);
        gameObject.transform.position = temp_player_poz;
    }

    //時間経過でプレイヤーを加速する関数
    private float SpeedUp()
    {
        //返り値用ローカル変数を宣言
        float temp_fall_speed = fall_speed;
        temp_fall_speed += add_speed * Time.deltaTime;
        //プレイヤーの速度は上限・下限の範囲で変動
        return Mathf.Clamp(temp_fall_speed, MIN_SPEED, MAX_SPEED);
    }

    //障害物衝突時にプレイヤーを減速する関数
    private float SpeedDown()
    {
        //返り値用ローカル変数を宣言
        float temp_fall_speed = fall_speed;
        temp_fall_speed -= reduce_speed;
        //プレイヤーの速度は上限・下限の範囲で変動
        return Mathf.Clamp(temp_fall_speed,MIN_SPEED,MAX_SPEED);
    }

    //時間経過で無敵モードポイントを累積
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
            //障害物の爆発エフェクト
            GameObject new_explosion_effect = Instantiate(explosion_effect ,other.gameObject.transform.position,other.gameObject.transform.rotation) as GameObject;
            GameObject.Destroy(new_explosion_effect, 1f);
            //障害物の消滅
            GameObject.Destroy(other.gameObject);
            return;
        }

        Debug.Log("衝突！");
        //障害物に衝突した時に減速
        if (other.gameObject.tag == "Obstacle")
        {
            Debug.Log("減速");
            //減速
            fall_speed = SpeedDown();

            //無敵モードポイントの減少
            invincible_point -= reduced_invincible_point;

            //ライフ減少
            player_life -= 1;
            //ライフが０になるとゲームオーバー
            if(player_life <= 0)
            {
                GameObject new_explosion_effect = Instantiate(explosion_effect, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                GameObject.Destroy(new_explosion_effect, 3f);
                GameObject.Destroy(gameObject);
            }
        }
    }

    //プレイヤーのエフェクトの切り替え（無敵モード時）
    private void InvincibleEffectOn()
    {
        Jet_Effect.SetActive(true);
        //        pt_red_fire.Stop();
        //        pt_blue_fire.Play();
    }
    //プレイヤーのエフェクトの切り替え（通常時）
    private void InvincibleEffectOff()
    {
        pt_red_fire.Play();
        Jet_Effect.SetActive(false);
 //       pt_blue_fire.Stop();
    }

    //ゲッター
    public int Player_life
    {
        get
        {
            return player_life;
        }
    }
    //ゲッター
    public float Fall_speed
    {
        get
        {
            return fall_speed;
        }
    }
    //ゲッター
    public float Invincible_point
    {
        get
        {
            return invincible_point;
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