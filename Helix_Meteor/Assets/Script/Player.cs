using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    //[SerializeField] Controller Controller_ = null;         //エディターからアタッチする
    [SerializeField] GameObject PlayerObj = null;           //エディターからアタッチする(nullチェック用)
    //[SerializeField] GameObject GameManager_ = null;     //[SerializeField] GameObject GameManager_ = null;
    public GameObject MainUIPanel_ = null;
    //[SerializeField] GameObject StartButton_ = null;
    public GameObject ContinueButton_ = null;
    public GameObject RetryButton_ = null;
    //暫定変数
    public GameObject GameManager_;
    public GameObject Controller_;

    //プレイヤーの位置に関する変数
    public Vector3 player_poz;                              //プレイヤーの位置
    const float MIN_X_POSITION = -3;
    const float MAX_X_POSITION = 3;
    const float MIN_Y_POSITION = -3;
    const float MAX_Y_POSITION = 3;
    //プレイヤーのライフに関する変数
    private int default_player_life = 2;                    //プレイヤーのライフ初期値
    private int player_life;                                 //プレイヤーのライフ
    //プレイヤーの速度に関する変数
    private float default_fall_speed = 0.75f;                //前方に移動する初期速度
    private float fall_speed;                               //前方に移動する速度
    private float add_speed = 0.1f;                         //時間経過で加算されるプレイヤーの直進速度
    private float reduce_speed = 1.0f;                      //障害物衝突時に減るプレイヤーの直進速度
    const float MIN_SPEED = 0.75f;                           //プレイヤーの直進スピード下限
    const float MAX_SPEED = 2.0f;                           //プレイヤーの直進スピード上限
    //無敵モードポイントに関する変数
    private bool invincible_flag = false;                   //無敵モードフラグ
    private float defaul_invincible_point = 0f;             //無敵モードポイントの初期値
    private float invincible_point;                         //無敵モードポイント（累積）
    private float add_invincible_point = 1.0f;              //加算される無敵モードポイント
    private float reduced_invincible_point = 1.0f;          //減じられる無敵モードポイント
    const float MIN_INVINCIBLE_POINT = 0f;                  //無敵モードポイントの下限値
    const float MAX_INVINCIBLE_POINT = 10.0f;               //無敵モードポイントの上限値
    //無敵時間に関する変数
    private float default_invincible_time = 0f;             //無敵モード中の経過時間の初期値
    private float invincible_time;                          //無敵モード中の経過時間（累積）
    private float add_invincible_time = 1.0f;               //加算される無敵モード中の経過時間
    const float MAX_INVINCIBLE_TIME = 5.0f;                 //無敵モード時間の上限
    //被ダメージ直後のノーダメージタイムに関する変数
    private bool no_damage_flag;                            //ノーダメージモードフラグ
    private float default_no_damage_time =0f;               //ノーダメージモード中の経過時間の初期値
    private float no_damage_time;                           //ノーダメージモード中の経過時間（累積）
    private float add_no_damage_time = 1.0f;                //加算されるノーダメージモード中の経過時間
    const float MAX_NO_DAMAGE_TIME = 1.0f;                  //ノーダメージモード時間の上限

    //演出
    [SerializeField] ParticleSystem pt_red_fire = null;     //プレイヤーのエフェクト（通常時・赤い炎）
    //public ParticleSystem pt_blue_fire;                   //プレイヤーのエフェクト（無敵モード時・青い炎）
    [SerializeField] GameObject Jet_Effect = null;          //プレイヤーのエフェクト（無敵モード時・効果線）
    [SerializeField] GameObject Inpact_Effect = null;       //被ダメージ時の爆発エフェクト
    [SerializeField] GameObject explosion_effect = null;    //死亡時の爆発エフェクト

    //被ダメージ時の画面エフェクト用フラグ
    private bool damaged_flag;

    //コールバック関数
    public System.Action<Vector3> OnPlayerDie;              //プレイヤー死亡時に呼び出すコールバック関数
    public System.Action<int> OnPlayerLifeChaged;           //プレイヤーのライフ増減時に呼び出すコールバック関数（UIへのライフ反映等）
    public System.Action OnPlayerDamaged;                   //プレイヤーの被ダメージ時に呼び出すコールバック関数（ダメージ演出等のため）
    public System.Action OnGoal;                            //ゴール時に呼び出すコールバック関数
    public System.Action<float> OnInvinciblePointChange;
    public System.Action OnInvincible;                      //

    private void Start()
    {
        //GM取得
        GameManager_ = GameObject.Find("GameManager");
        //プレイヤー位置格納
        player_poz = gameObject.transform.position;
        //ライフ初期化
        player_life = default_player_life;
        //UIにライフの値を渡す
        if (OnPlayerLifeChaged != null)
        {
            OnPlayerLifeChaged(player_life);
        }
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

        //UIに無敵モードポイントを反映
        if(OnInvinciblePointChange != null)
        {
            OnInvinciblePointChange(invincible_point);
        }

        //被ダメージ直後のノーダメージタイム（数秒間・即死防止用）
        if (no_damage_flag)
        {
            no_damage_time += add_no_damage_time * Time.deltaTime;
            if (CheckNoDamage(no_damage_time))
            {
                NoDamageModeOff();
            }
        }

        //無敵モードの時、一定時間経過で無敵モード解除・無敵モードポイント初期化
        if (invincible_flag)
        {
            //無敵時間を計測
            invincible_time += add_invincible_time * Time.deltaTime;
            //一定時間経過で無敵モード解除
            if (CheckInvincibleTime())
            {
                InvincibleModeOff();
            }
        }

        //通常モードの時、無敵モードポイントを累積→一定値を超えると無敵モードへ移行
        if (invincible_flag == false)
        {
            InvincibleModeOn();
        }
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
    public float InvinciblePoint
    {
        get
        {
            return invincible_point;
        }
        set
        {
            invincible_point = value;
        }
    }

    //プレイヤーの上下左右斜め移動関数(スワイプ/マウスドラッグによる操作)
    public void Move(Vector3 move_direction,float move_speed)
    {
        //nullcheck
        if (PlayerObj == null){return;}

        Vector3 player_poz_ = gameObject.transform.position;
        player_poz_ += move_direction * move_speed;
        player_poz_.x = Mathf.Clamp(player_poz_.x,MIN_X_POSITION,MAX_X_POSITION);
        player_poz_.y = Mathf.Clamp(player_poz_.y, MIN_Y_POSITION, MAX_Y_POSITION);
        gameObject.transform.position = player_poz_;
    }

    //被ダメージ直後のノーダメージタイム移行（即死防止）
    public void NoDamageModeOn()
    {
        gameObject.GetComponent<SphereCollider>().enabled = false;
        no_damage_flag = true;
        Debug.Log("ノーダメージ中！");
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

    //時間経過でプレイヤーを加速する関数
    private float SpeedUp()
    {
        //返り値用ローカル変数を宣言
        float fall_speed_ = fall_speed;
        fall_speed_ += add_speed * Time.deltaTime;
        //プレイヤーの速度は上限・下限の範囲で変動
        return Mathf.Clamp(fall_speed_, MIN_SPEED, MAX_SPEED);
    }

    //障害物衝突時にプレイヤーを減速する関数
    private float SpeedDown()
    {
        //返り値用ローカル変数を宣言
        float fall_speed_ = fall_speed;
        fall_speed_ -= reduce_speed;
        //プレイヤーの速度は上限・下限の範囲で変動
        return Mathf.Clamp(fall_speed_,MIN_SPEED,MAX_SPEED);
    }

    //時間経過で無敵モードポイントを累積
    private float AddInvinciblePoint()
    {
        float invincible_point_ = invincible_point;
        invincible_point_ += add_invincible_point * Time.deltaTime;
        return Mathf.Clamp(invincible_point_, MIN_INVINCIBLE_POINT, MAX_INVINCIBLE_POINT);
    }

    //オブジェクトに衝突した際の処理の関数
    private void OnTriggerEnter(Collider other)
    {
        //ゴールした際の処理
        if (other.gameObject.tag == "Goal")
        {
            Debug.Log("ゴール！！");
            Vector3 transport_poz = new Vector3(0,0,10000);         //仮置きの処理（後で削除）
            gameObject.transform.position = transport_poz;          //仮置きの処理（後で削除）
            //コールバック
            if (OnGoal != null)
            {
                OnGoal();
            }
        }

        //無敵モード時は障害物の影響を受けない
        if (invincible_flag)
        {
            Debug.Log("無敵モード中！");

            //障害物の爆発エフェクト
            ExplosionEffect(other);
            //障害物の爆発音
            if (OnInvincible != null)
            {
                OnInvincible();
            }

            //障害物の消滅
            GameObject.Destroy(other.gameObject);
            return;
        }

        //障害物に衝突した時の処理
        if (other.gameObject.tag == "Obstacle")
        {
            Debug.Log("衝突！");
            //減速
            fall_speed = SpeedDown();
            //無敵モードポイントの減少
            invincible_point -= reduced_invincible_point;
            //ライフ減少
            player_life -= 1;
            //ライフ減少をUIに反映コールバック
            if (OnPlayerLifeChaged != null)
            {
                OnPlayerLifeChaged(player_life);
            }
            //ノーダメージ処理
            NoDamageModeOn();
            //画面演出処理用コールバック
            if (OnPlayerDamaged != null)
            {
                OnPlayerDamaged();
            }
            //衝突エフェクト
            DamagedEffect();
            //ライフが０になるとゲームオーバー
            if (player_life <= 0)
            {
                PlayerDie();
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
        //pt_blue_fire.Stop();
    }

    //被ダメージ時のエフェクト
    private void DamagedEffect()
    {
        GameObject new_inpact_effect = Instantiate(Inpact_Effect, transform.position, transform.rotation) as GameObject;
        GameObject.Destroy(new_inpact_effect, 1f);
    }

    //プレイヤーの爆発エフェクト
    private void PlayerExplosionEffect()
    {
        GameObject new_explosion_effect = Instantiate(explosion_effect, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
        GameObject.Destroy(new_explosion_effect, 3f);
    }

    //障害物の爆発エフェクト
    private void ExplosionEffect(Collider other_)
    {
        GameObject new_explosion_effect = Instantiate(explosion_effect, other_.gameObject.transform.position, other_.gameObject.transform.rotation) as GameObject;
        GameObject.Destroy(new_explosion_effect, 1f);
    }

    //無敵モードポイントが最大ならtrueを返す
    private bool CheckInvinciblePoint()
    {
        if (invincible_point >= MAX_INVINCIBLE_POINT)
        {
            return true;
        }
        else{
            return false;
        }
    }

    //無敵状態が一定時間経過するとtrueを返す
    private bool CheckInvincibleTime()
    {
        if (invincible_time >= MAX_INVINCIBLE_TIME)
        {
            return true;
        }
        else{
            return false;
        }
    }
    
    //無敵モードに切り替え
    private void InvincibleModeOn()
    {
        //時間経過で無敵モード・ポイントを累積
        invincible_point = AddInvinciblePoint();
        //無敵モードポイントが最大値の時、無敵モードフラグを立てる
        if (CheckInvinciblePoint())
        {
            Debug.Log("無敵モード！");
            //無敵モードフラグON
            invincible_flag = true;
            //プレイヤーのエフェクト変化
            InvincibleEffectOn();
        }
    }
    
    //無敵モードを解除
    private void InvincibleModeOff()
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

    //プレイヤーDestroy時にメンバ変数にnull格納
    private void OnDestroy()
    {
        Controller_ = null;
        PlayerObj = null;
    }

    //プレイヤー死亡時の処理
    private void PlayerDie()
    {
        Debug.Log("死亡！");
        //爆発エフェクト
        PlayerExplosionEffect();
        //コールバック
        if (OnPlayerDie != null) {
            OnPlayerDie(transform.position);
        }        
        //プレイヤー消滅
        GameObject.Destroy(gameObject);
    }
    
    //被ダメージ直後のノーダメージタイム解除（即死防止）
    private void NoDamageModeOff()
    {
        gameObject.GetComponent<SphereCollider>().enabled = true;
        no_damage_time = default_no_damage_time;
        no_damage_flag = false;
        Debug.Log("ノーダメージ解除！");
    }
    //被ダメージ直後のノーダメージタイムの解除判定の関数
    bool CheckNoDamage(float no_damage_time_)
    {
        if (no_damage_time_ >= MAX_NO_DAMAGE_TIME)
            {
                return true;
            }
            else
            {
                return false;
            }           
    }
}