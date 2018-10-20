using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;



public class Controller : MonoBehaviour {
    //エディターからアタッチするメンバ変数
    [SerializeField] Player player_ = null; 
    [SerializeField] MeteorCamera meteor_camera_ = null;     
    [SerializeField] UIController ui_controller_ = null;
    [SerializeField] GameObject player_prefab_ = null;
    [SerializeField] GameObject player_clone_ = null;
    [SerializeField] GameObject meteor_camera_obj_ = null; 
    [SerializeField] AudioClip inpact_sound = null;          //被ダメージ時の効果音
    [SerializeField] AudioClip explosion_sound = null;       //死亡時の効果音
    //ステージ
    [SerializeField] GameObject[] stage_;
    private int default_stage_index = 0;
    private int now_stage_index;
    private int last_stage_index;
    private GameObject new_stage;
    private Vector3 new_stage_position 
        = new Vector3(0,0,0);
    private Quaternion new_stage_rotation 
        = new Quaternion(0,0,0,0);
    //プレイヤーの移動
    private Vector3 touch_poz;
    private Vector3 old_player_poz;                         //前フレームでのタッチ位置（スワイプによる上下左右移動処理用）
    private Vector3 new_player_poz;                         //現在フレームでのタッチ位置（スワイプによる上下左右移動処理用）
    private Vector3 move_direction;                         //上下左右移動の移動方向（Player_.Move関数の引数）
    private float move_speed = 20.0f;                       //上下左右移動のスピード調整用の値（Player_.Move関数の引数）
    //private int default_play_time =1;
    private int play_time;                                  //タイムカウンタ、ハイスコア用のプレイタイム
    //    float lifetime = 0.1f;
    private AudioSource audio_source;                       
    private Vector3 continue_position;                      //コンティニュー時の再開地点
    private Vector3 start_position = new Vector3(0,0,0);    //スタート・リトライ時の開始地点
    //タイマー関連
    private bool time_count_flag = false;                   //カウント開始・停止用のフラグ
    private float play_time_minute;                         //タイム（分）
    private float play_time_seconds;                        //タイム（秒）
    //スコア管理関連
    private string high_score_text;                         //
    private float[] best_time;                              //ベストタイム
    private float best_time_minute;                         //
    private float best_time_seconds;                        //
    private string[] high_score_key;                        //ハイスコアの保存先キー
    private bool new_record_flag;                           //

    private void Start()
    {
        //ステージ生成
        new_stage = Instantiate(stage_[default_stage_index],new_stage_position,new_stage_rotation) as GameObject;
        now_stage_index = default_stage_index;
        last_stage_index = stage_.Length -1;

        //ハイスコアの初期化
        PlayerPrefs.DeleteAll();
        //ハイスコアの保存キー配列初期化
        high_score_key = new string[stage_.Length];
        //ベストタイムの配列初期化
        best_time = new float[stage_.Length];
        //ベストタイム取得
        high_score_key[now_stage_index] = "HighScoreKey" + now_stage_index.ToString();
        best_time[now_stage_index] = PlayerPrefs.GetFloat(high_score_key[now_stage_index],999);
        //スタート画面のベストタイムの表示
        //ベストタイムがない場合は「99：99」と表示される
        if (best_time[now_stage_index] == 999)
        {
            best_time_minute = 99.0f;
            best_time_seconds = 99.0f;
            ui_controller_.BestTimeUi(best_time_minute,best_time_seconds,false);
        }else if (best_time[now_stage_index] != 999) {
            best_time_minute = Mathf.Clamp((best_time[now_stage_index] / 60) - (best_time[now_stage_index] % 60), 0, 60);
            best_time_seconds = Mathf.Clamp(best_time[now_stage_index] - best_time_minute * 60, 0, 60);
            ui_controller_.BestTimeUi(best_time_minute, best_time_seconds,false);
        }

        //各種初期設定
        audio_source = gameObject.GetComponent<AudioSource>();
        meteor_camera_ = meteor_camera_obj_.GetComponent<MeteorCamera>();
        
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

        //タイムを計測しタイマーUIに反映
        if (time_count_flag)
        {
            CountTime();
        }

        //画面振動させる処理
        //if (player_.Player_life < 3)
        //{
        //    if (lifetime > 0) {
        //        meteor_camera_.Shake_Camera();
        //        lifetime -= Time.deltaTime;
        //    }
        //}
    }
    //プレイヤー無敵モードで衝突時にコールバックされる処理
    private void OnInvincibleCallBack()
    {
        audio_source.PlayOneShot(explosion_sound);
    }

    //プレイヤーがダメージ受けコールバックされる処理
    private void OnPlayerDamagedCallBack()
    {
        audio_source.PlayOneShot(inpact_sound);
        ui_controller_.DamagedEffect();
    }

    //プレイヤー死亡時にコールバックされる処理
    private void OnPlayerDieCallBack(Vector3 player_die_position)
    {
        Debug.Log("プレイヤー死亡によりコールバック！");
        audio_source.PlayOneShot(explosion_sound);
        continue_position = player_die_position;

        //コールバック関数を解除
        player_.OnPlayerDie -= OnPlayerDieCallBack;
        player_.OnPlayerLifeChaged -= OnplayerLifeChangedCallBack;
        player_.OnPlayerDamaged -= OnPlayerDamagedCallBack;
        player_.OnGoal -= OnGoalCallBack;
        player_.OnInvincible -= OnInvincibleCallBack;
        player_.OnInvinciblePointChange -= OnIPointChangeCallBack;
        player_clone_ = null;

        //タイマーストップフラグ
        TimeCountFlagOff();

        //コンティニューとリトライのボタンだけ表示させる
        ui_controller_.MenuUiActive(UIController.ButtonType.Panel);
        ui_controller_.MenuUiActive(UIController.ButtonType.Continue);
        ui_controller_.MenuUiActive(UIController.ButtonType.Retry);
        ui_controller_.GameOverMessageActive();
    }

    //ゴール時にコールバックされる処理
    private void OnGoalCallBack()
    {
        //クリアフラグ設定
        ui_controller_.ClearFlagOn();
        //タイマーストップフラグ
        TimeCountFlagOff();
        //ベストタイムを分と秒に変換
        best_time_minute = Mathf.Clamp((best_time[now_stage_index] /60) - (best_time[now_stage_index]%60),0,60);
        best_time_seconds = Mathf.Clamp(best_time[now_stage_index] - best_time_minute*60,0,60);

        //ベストタイム更新した際の処理
        if ((play_time_minute < best_time_minute) || ((play_time_minute == best_time_minute) && (play_time_seconds < best_time_seconds)))
        {
            Debug.Log("ベストタイム更新!!"+ play_time_minute.ToString("00") +":"+play_time_seconds.ToString("00"));
            //ベストタイムを保存
            best_time[now_stage_index] = play_time_minute * 60 + play_time_seconds;
            PlayerPrefs.SetFloat(high_score_key[now_stage_index], best_time[now_stage_index]);
            //ベストタイムを表示
            ui_controller_.BestTimeUi(play_time_minute, play_time_seconds,true);
        }
        //ベストタイム更新できなかった際の処理
        else if((play_time_minute > best_time_minute) || ((play_time_minute == best_time_minute) && (play_time_seconds >= best_time_seconds)))
        {
            Debug.Log("更新ならず！ベストタイムは" + best_time_minute.ToString("00") + ":" + best_time_seconds.ToString("00"));
            ui_controller_.BestTimeUi(best_time_minute, best_time_seconds,false);
        }
        ui_controller_.MenuUiActive(UIController.ButtonType.Panel);
        ui_controller_.MenuUiActive(UIController.ButtonType.Retry);
        ui_controller_.BestTimeActive();

        if (now_stage_index < last_stage_index)
        {
            //ステージクリアのメッセージ表示
            Debug.Log("ステージクリア（全クリではない）");
            ui_controller_.SetClearMessageNormal();
            //ネクストステージボタンを表示
            ui_controller_.MenuUiActive(UIController.ButtonType.NextStage);
        }
        else if (now_stage_index >= last_stage_index)
        {
            //全ステージクリアのメッセージ表示
            Debug.Log("全ステージクリア");
            ui_controller_.SetClearMessageAll();
        }
    }

    //プレイヤーライフが変更時コールバックされる処理
    private void OnplayerLifeChangedCallBack(int player_life)
    {
        //プレイヤークローンのライフをUIに反映
        ui_controller_.PlayerLifeUI(player_life);
    }

    //スタートボタンでコールバックされる処理
    private void OnStartButtonCallBack()
    {
        Debug.Log("スタートボタンでコールバックされた！");
        //プレイヤーを生成
        ClonePlayer(start_position);
        player_clone_.SetActive(true);
        //カメラを設定
        meteor_camera_.PlayerClone = player_clone_;
        //タイマースタート
        TimeCountFlagOn();
    }

    //コンティニューボタンでコールバックされる処理
    private void OnContinueButtonCallBack()
    {
        Debug.Log("コンティニューボタンでコールバックされた！");
        //プレイヤーを生成
        ClonePlayer(continue_position);
        //ノーダメージ期間の実装
        player_.NoDamageModeOn();       
        player_clone_.SetActive(true);
        //カメラを設定
        meteor_camera_.PlayerClone = player_clone_;
        //タイマースタート
        TimeCountFlagOn();
    }

    //リトライボタンでコールバックされる処理
    private void OnRetryButtonCallBack()
    {
        Debug.Log("リトライボタンでコールバックされた！");
        //ステージを再生成
        GameObject.Destroy(new_stage);
        new_stage = null;
        new_stage = Instantiate(stage_[now_stage_index], new_stage_position, new_stage_rotation) as GameObject;
        //プレイヤーを生成
        ClonePlayer(start_position);
        player_clone_.SetActive(true);
        //カメラを設定
        meteor_camera_.PlayerClone = player_clone_;
        //タイマースタート
        TimeCountFlagOn();
        ResetTime();
    }

    //ネクストステージボタンでコールバックされる処理
    private void OnNextStageButtonCallBack()
    {
        Debug.Log("ネクストボタンでコールバックされた！");

        ui_controller_.MenuUiActive(UIController.ButtonType.Panel);
        ui_controller_.MenuUiActive(UIController.ButtonType.Start);

        //プレイヤー位置を初期化
        GameObject.Destroy(player_clone_);
        //カメラ位置を初期化
        meteor_camera_.PlayerClone = null;
        meteor_camera_.SetDefaultPosition();
        //タイマーリセット
        ResetTime();

        //ステージ変更
        if (now_stage_index < last_stage_index)
        {
            //次ステージを生成
            now_stage_index++;
            GameObject.Destroy(new_stage);
            new_stage = null;
            new_stage = Instantiate(stage_[now_stage_index], new_stage_position, new_stage_rotation) as GameObject;
            //ステージ名を表示
            ui_controller_.StageNameActive(now_stage_index + 1);
        }

        //次のステージのベストタイム取得
        //次のステージのハイスコアキーを取得
        high_score_key[now_stage_index] = "HighScoreKey" + now_stage_index.ToString();
        best_time[now_stage_index] = PlayerPrefs.GetFloat(high_score_key[now_stage_index], 999);
        //ベストタイムなら更新      
        if (best_time[now_stage_index] == 999)
        {
            best_time_minute = 99.0f;
            best_time_seconds = 99.0f;
            ui_controller_.BestTimeUi(best_time_minute, best_time_seconds, false);
        }
        else if (best_time[now_stage_index] != 999)
        {
            best_time_minute = Mathf.Clamp((best_time[now_stage_index] / 60) - (best_time[now_stage_index] % 60), 0, 60);
            best_time_seconds = Mathf.Clamp(best_time[now_stage_index] - best_time_minute * 60, 0, 60);
            ui_controller_.BestTimeUi(best_time_minute, best_time_seconds, false);
        }
        ui_controller_.BestTimeActive();
    }

    private void OnIPointChangeCallBack(float invincivle_point_)
    {
        ui_controller_.InvinciblePointUI(invincivle_point_);
    }

    //ボタン押下時にプレイヤーのクローンを生成
    private void ClonePlayer(Vector3 clone_position_)
    {
        GameObject new_player = Instantiate(player_prefab_, clone_position_, gameObject.transform.rotation) as GameObject;
        player_clone_ = new_player;
        player_ = player_clone_.GetComponent<Player>();

        //プレイヤー死亡時のコールバック関数を登録
        player_.OnPlayerDie += OnPlayerDieCallBack;
        //プレイヤーライフが変更されたときのコールバック関数を登録
        player_.OnPlayerLifeChaged += OnplayerLifeChangedCallBack;
        //プレイヤー被ダメージ時のコールバック関数を登録
        player_.OnPlayerDamaged += OnPlayerDamagedCallBack;
        //ゴール時のコールバック関数を登録
        player_.OnGoal += OnGoalCallBack;
        //無敵モードでの衝突時のコールバック関数を登録
        player_.OnInvincible += OnInvincibleCallBack;
        //無敵モードポイント変化時のコールバック関数を登録
        player_.OnInvinciblePointChange += OnIPointChangeCallBack;
    }

    //時間を計測する関数
    private void CountTime()
    {
        play_time_seconds += Time.deltaTime;
        if (play_time_seconds >= 60)
        {
            Debug.Log("繰り上げ！");
            play_time_minute++;
            play_time_seconds -= 60;
        }
        ui_controller_.CountTimeUi(play_time_minute,play_time_seconds);
    }
    //タイムをリセット
    private void ResetTime()
    {
        play_time_minute = 0;
        play_time_seconds = 0;
    }
    //UIのタイムカウントを進める/止めるのフラグ設定
    public void TimeCountFlagOn()
    {
        time_count_flag = true;
    }
    public void TimeCountFlagOff()
    {
        time_count_flag = false;
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
