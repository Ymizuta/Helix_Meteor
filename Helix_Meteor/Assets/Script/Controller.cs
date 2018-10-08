﻿using System.Collections;
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
    [SerializeField] AudioClip inpact_sound = null;         //被ダメージ時の効果音
    [SerializeField] AudioClip explosion_sound = null;      //死亡時の効果音
    //ステージ
    [SerializeField] GameObject stage01_ = null;
    [SerializeField] GameObject stage02_ = null;

    private Vector3 touch_poz;
    private Vector3 old_player_poz;                 //前フレームでのタッチ位置（スワイプによる上下左右移動処理用）
    private Vector3 new_player_poz;                 //現在フレームでのタッチ位置（スワイプによる上下左右移動処理用）
    private Vector3 move_direction;                 //上下左右移動の移動方向（Player_.Move関数の引数）
    private float move_speed = 10.0f;               //上下左右移動のスピード調整用の値（Player_.Move関数の引数）
    private int default_play_time =1;
    private int play_time;
    //    float lifetime = 0.1f;

    private AudioSource audio_source;
    private Vector3 continue_position;              //コンティニュー時の再開地点

    private void Start()
    {
        audio_source = gameObject.GetComponent<AudioSource>();
        //コールバックするメソッドを登録
        ui_controller_.OnStartButton += this.OnStartButtonCallBack;
        ui_controller_.OnContinueButton += this.OnContinueButtonCallBack;
        ui_controller_.OnRetryButton += this.OnRetryButtonCallBack;
        ui_controller_.OnNextStageButton += this.OnNextStageButtonCallBack;
    }

    // Update is called once per frame
    void Update () {
        //nullチェック
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
            //処理なし
        }

        //プレイタイムを累積
        CountTime();

        //画面振動させる処理
        //if (player_.Player_life < 3)
        //{
        //    if (lifetime > 0) {
        //        meteor_camera_.Shake_Camera();
        //        lifetime -= Time.deltaTime;
        //    }
        //}
    }
    
    //プレイヤーがダメージ受けコールバックされた際の処理
    public void OnPlayerDamagedCallBack()
    {
        audio_source.PlayOneShot(inpact_sound);
        ui_controller_.DamagedEffect();
    }

    //プレイヤー死亡時にコールバックされた際の処理
    public void OnPlayerDieCallBack(Vector3 player_die_position)
    {
        Debug.Log("プレイヤー死亡によりコールバック！");
        audio_source.PlayOneShot(explosion_sound);
        continue_position = player_die_position;

        //コンティニューとリトライのボタンだけ表示させる
        bool start_button_ = false;
        bool continue_button_ = true;
        bool retry_button_ = true;
        bool next_stage_button = false;
        ui_controller_.MenuUiOn(start_button_, continue_button_, retry_button_, next_stage_button);
    }

    //プレイヤーライフが変更時コールバックされた際の処理
    public void OnplayerLifeChangedCallBack(int player_life)
    {
        //プレイヤークローンのライフをUIに反映
        ui_controller_.PlayerLifeUI(player_life);
    }

    //ゴール時にコールバックされた際の処理
    public void OnGoalCallBack()
    {
        //クリアフラグ設定
        ui_controller_.StageClearFlag = true;
        //リトライとネクストのボタンだけ表示させる
        bool start_button_ = false;
        bool continue_button_ = false;
        bool retry_button_ = true;
        bool next_stage_button = true;
        ui_controller_.MenuUiOn(start_button_,continue_button_,retry_button_,next_stage_button);
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
    //プレイヤーライフが変更されたときのコールバック関数を登録
    player_clone_.GetComponent<Player>().OnPlayerLifeChaged += OnplayerLifeChangedCallBack;
    //プレイヤー被ダメージ時のコールバック関数を登録
    player_clone_.GetComponent<Player>().OnPlayerDamaged = OnPlayerDamagedCallBack;
    //ゴール時のコールバック関数を登録
    player_clone_.GetComponent<Player>().OnGoal = OnGoalCallBack;
    //カメラを設定
    meteor_camera_obj_.GetComponent<MeteorCamera>().PlayerClone = new_player;
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
        //プレイヤーライフが変更された時のコールバック関数を登録
        player_clone_.GetComponent<Player>().OnPlayerLifeChaged += OnplayerLifeChangedCallBack;
        //プレイヤー被ダメージ時のコールバック関数を登録
        player_clone_.GetComponent<Player>().OnPlayerDamaged = OnPlayerDamagedCallBack;
        //ゴール時のコールバック関数を登録
        player_clone_.GetComponent<Player>().OnGoal = OnGoalCallBack;
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
        //プレイヤーライフが変更された時のコールバック関数を登録
        player_clone_.GetComponent<Player>().OnPlayerLifeChaged += OnplayerLifeChangedCallBack;
        //プレイヤー被ダメージ時のコールバック関数を登録
        player_clone_.GetComponent<Player>().OnPlayerDamaged = OnPlayerDamagedCallBack;
        //ゴール時のコールバック関数を登録
        player_clone_.GetComponent<Player>().OnGoal = OnGoalCallBack;
        //カメラを設定
        meteor_camera_obj_.GetComponent<MeteorCamera>().PlayerClone = new_player;
    }

    //ネクストステージボタンでコールバックされた際の処理
    public void OnNextStageButtonCallBack()
    {
        Debug.Log("ネクストボタンでコールバックされた！");

        //スタートボタンだけ表示させる
        bool start_button_ = true;
        bool continue_button_ = false;
        bool retry_button_ = false;
        bool next_stage_button = false;
        ui_controller_.MenuUiOn(start_button_, continue_button_, retry_button_, next_stage_button);
        //ステージ２と表示する

        //プレイヤー位置を初期化
        GameObject.Destroy(player_clone_);
        //カメラ位置を初期化
        meteor_camera_.PlayerClone = null;
        meteor_camera_obj_.GetComponent<MeteorCamera>().SetDefaultPosition();
        //ステージ変更
        stage01_.SetActive(false);
        stage02_.SetActive(true);
    }

    private void CountTime()
    {
        float play_time_ = Time.time;
        play_time = (int)play_time_;
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
