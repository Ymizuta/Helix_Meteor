using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Controller : MonoBehaviour {

    [SerializeField] Player player_ = null; // エディターからアタッチ
    [SerializeField] MeteorCamera meteor_camera_ = null; // エディターからアタッチ
    [SerializeField] GameObject player_prefab_ = null;
    [SerializeField] GameObject player_clone_ = null;
    [SerializeField] GameObject meteor_camera_obj_ = null; 
    [SerializeField] UIController ui_controller_ = null;

    private Vector3 touch_poz;
    private Vector3 old_player_poz;                 //前フレームでのタッチ位置（スワイプによる上下左右移動処理用）
    private Vector3 new_player_poz;                 //現在フレームでのタッチ位置（スワイプによる上下左右移動処理用）
    private Vector3 move_direction;                 //上下左右移動の移動方向（Player_.Move関数の引数）
    private float move_speed = 10.0f;               //上下左右移動のスピード調整用の値（Player_.Move関数の引数）
    private int default_play_time =1;
    private int play_time;
    //    float lifetime = 0.1f;

    private Vector3 continue_position;

    private void Start()
    {
        //コールバックするメソッドを登録
        ui_controller_.OnStartButton += this.OnStartButtonCallBack;
        ui_controller_.OnContinueButton += this.OnContinueButtonCallBack;
        ui_controller_.OnRetryButton += this.OnRetryButtonCallBack;

    }

    // Update is called once per frame
    void Update () {
        if (player_clone_ == null)
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
            player_clone_.GetComponent<Player>().Move(move_direction, move_speed);

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

    //プレイヤー被ダメージ時にコールバックされた際の処理
    public void OnplayerLifeChangedCallBack(int player_life)
    {
        //プレイヤークローンのライフをUIに反映
        ui_controller_.LifeTextUI.GetComponent<Text>().text = "LIFE:" + player_clone_.GetComponent<Player>().Player_life.ToString();
    }

    //プレイヤー死亡時にコールバックされた際の処理
    public void OnPlayerDieCallBack(Vector3 player_die_position)
    {
        ui_controller_.PlayerDieFlag = true;
        continue_position = player_die_position;
    }

    //スタートボタンでコールバックされた際の処理
    public void OnStartButtonCallBack()
    {
    Debug.Log("スタートボタンでコールバックされた！");
    //プレイヤーを生成
    Vector3 start_position = new Vector3(0, 0, 0);
    GameObject new_player = Instantiate(player_prefab_, start_position, gameObject.transform.rotation) as GameObject;
    player_clone_ = new_player;
    player_clone_.SetActive(true);
    //プレイヤー死亡時のコールバック関数を登録
    player_clone_.GetComponent<Player>().OnPlayerDie += this.OnPlayerDieCallBack;
    //プレイヤー被ダメージ時のコールバック関数を登録
    player_clone_.GetComponent<Player>().OnPlayerLifeChaged += OnplayerLifeChangedCallBack;
    //カメラを設定
    meteor_camera_obj_.GetComponent<MeteorCamera>().PlayerClone = new_player;
    //UI設定
    //プレイヤークローンの初期ライフをUIに反映
    ui_controller_.LifeTextUI.GetComponent<Text>().text = "LIFE:" + player_clone_.GetComponent<Player>().Player_life.ToString();
    }

    //コンティニューボタンでコールバックされた際の処理
    public void OnContinueButtonCallBack()
    {
        Debug.Log("コンティニューボタンでコールバックされた！");
        //プレイヤーを生成
        GameObject new_player = Instantiate(player_prefab_, continue_position, gameObject.transform.rotation) as GameObject;
        player_clone_ = new_player;
        //プレイヤー死亡時のコールバック関数を登録
        player_clone_.GetComponent<Player>().OnPlayerDie += this.OnPlayerDieCallBack;
        //プレイヤー被ダメージ時のコールバック関数を登録
        player_clone_.GetComponent<Player>().OnPlayerLifeChaged += OnplayerLifeChangedCallBack;
        //ノーダメージ期間の実装
        player_clone_.GetComponent<Player>().NoDamageModeOn();
        player_clone_.SetActive(true);
        //カメラを設定
        meteor_camera_obj_.GetComponent<MeteorCamera>().PlayerClone = new_player;
    }

    public void OnRetryButtonCallBack()
    {
        Debug.Log("リトライボタンでコールバックされた！");
        //プレイヤーを生成
        Vector3 retry_position = new Vector3(0, 0, 0);
        GameObject new_player = Instantiate(player_prefab_, retry_position, gameObject.transform.rotation) as GameObject;
        player_clone_ = new_player;
        new_player.SetActive(true);
        //プレイヤー死亡時のコールバック関数を登録
        player_clone_.GetComponent<Player>().OnPlayerDie += this.OnPlayerDieCallBack;
        //プレイヤー被ダメージ時のコールバック関数を登録
        player_clone_.GetComponent<Player>().OnPlayerLifeChaged += OnplayerLifeChangedCallBack;
        //カメラを設定
        meteor_camera_obj_.GetComponent<MeteorCamera>().PlayerClone = new_player;
    }

    private void CountTime()
    {
        float temp_play_time = Time.time;
        play_time = (int)temp_play_time;
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
            player_clone_ = value;
        }
    }
}
