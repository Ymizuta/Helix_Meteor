using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class UIController : MonoBehaviour {

    [SerializeField] GameObject prefav_player = null;
    [SerializeField] GameObject Player_ = null;
    [SerializeField] Camera meteor_camera = null;

    //表示UI（ライフ、タイム等）
    [SerializeField] GameObject LifeText = null;
    [SerializeField] GameObject PlayTimeText = null;
    [SerializeField] GameObject ClearMassage= null;
    //操作UI（スタート、リトライ等のボタン）
    [SerializeField] GameObject MainPanel_ = null;
    [SerializeField] GameObject StartButton_ = null;
    [SerializeField] GameObject ContinueButton_ = null;
    [SerializeField] GameObject RetryButton_ = null;
    [SerializeField] Controller Controller_ = null;
    //エフェクト関連
    [SerializeField] GameObject DamageImage = null;
    private Image damage_img;
    private AudioSource audio_source;
    [SerializeField] AudioClip inpact_sound = null;         //被ダメージ時の効果音
    [SerializeField] AudioClip explosion_sound = null;      //死亡時の効果音
    //タイム計測
    private bool time_count_flag = false;
    private float play_time_minute;
    private float play_time_seconds;
    //プレイヤークラスから値を受け取る変数
    private Vector3 dying_position;                         //プレイヤー消滅位置
    private bool player_die_flag;                           //プレイヤー死亡フラグ
    private int player_life;                                //プレイヤーのライフ
    private bool damaged_flag;                              //被ダメージ時のフラグ
    private bool stage_clear_flag;                          //ステージクリアのフラグ

    //コールバック関数
    public System.Action OnStartButton;
    public System.Action OnContinueButton;
    public System.Action OnRetryButton;

    private void Start()
    {
        //各UIボタンに関数を割り当て
        StartButton_.GetComponent<Button>().onClick.AddListener(PushStartButton);
        ContinueButton_.GetComponent<Button>().onClick.AddListener(PushContinueButton);
        RetryButton_.GetComponent<Button>().onClick.AddListener(PushRetryButton);
        //ダメージエフェクト用のImage設定
        damage_img = DamageImage.GetComponent<Image>();
        damage_img.color = Color.clear;
        //オーディオソース設定
        audio_source = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        //画面の色をリセット
        DamageEffectReset();

        //クリア時の処理
        if (stage_clear_flag)
        {
            ClearMassage.SetActive(true);
            ClearMassage.GetComponent<Text>().color = new Color(255f,255f,255f,Mathf.PingPong(Time.time,1));
            return;
        }

        //タイムを計測しタイマーUIに反映
        if (time_count_flag)
        {
            CountTime();
        }        
    }

    //スタートボタン押下時の処理
    private void PushStartButton()
    {
        //タイム計測
        time_count_flag = true;
        //UI非表示
        MainPanel_.SetActive(false);
        StartButton_.SetActive(false);
        //コールバック（player生成等の処理を実行）
        if(OnStartButton != null)
        {
            OnStartButton();
        }
    }
    //コンティニューボタン押下時の処理
    private void PushContinueButton()
    {
        //タイム計測
        time_count_flag = true;
        //UI表示
        MainPanel_.SetActive(false);
        RetryButton_.SetActive(false);
        ContinueButton_.SetActive(false);
        player_die_flag = false;
        //コールバック（player生成等の処理を実行）
        if (OnContinueButton != null)
        {
            OnContinueButton();
        }

    }
    //リトライボタン押下時の処理
    private void PushRetryButton()
    {
        //タイム計測
        time_count_flag = true;
        ResetTime();
        //UI表示
        MainPanel_.SetActive(false);
        RetryButton_.SetActive(false);
        ContinueButton_.SetActive(false);
        player_die_flag = false;
        //コールバック（player生成等の処理を実行）
        if (OnRetryButton != null)
        {
            OnRetryButton();
        }
    }

    //時間を計測する関数
    private void CountTime()
    {
        play_time_seconds += Time.deltaTime;
        if (play_time_minute > 60)
        {
            play_time_minute++;
            play_time_seconds -= 60;
        }
        PlayTimeText.GetComponent<Text>().text = play_time_minute.ToString("00") + ":" + play_time_seconds.ToString("00");
    }

    //プレイヤー死亡に伴いメニューUIを表示する
    public void MenuUiOn()
    {
        //フラグ設定
        time_count_flag = false;
        MainPanel_.SetActive(true);
        ContinueButton_.SetActive(true);
        RetryButton_.SetActive(true);
    }

    //タイムカウンタのUIをリセット
    private void ResetTime()
    {
        play_time_minute = 0;
        play_time_seconds = 0;
    }
    
    //プレイヤーライフをUIに反映
    public void PlayerLifeUI(int player_life)
    {
        LifeText.GetComponent<Text>().text = "LIFE:" + player_life.ToString();
    }

    //被ダメージのエフェクト
    public void DamagedEffect()
    {
            //画面が赤く点滅
            damage_img.color = new Color(0.5f, 0f, 0f, 0.5f);
    }

    //被ダメージ時の画面エフェクトリセット
    public void DamageEffectReset()
    {
        //一秒でリセット
        damage_img.color = Color.Lerp(damage_img.color, Color.clear, Time.deltaTime);
    }

    //セッター（プレイヤー死亡時に死亡検知用にフラグを受け取る）
    public bool PlayerDieFlag
    {
        set
        {
            player_die_flag = value;
        }
    }

    //セッター（プレイヤーライフUI用）
    public int PlayerLife
    {
        set{
            player_life = value; 
        }
    }

    //セッター（ダメージエフェクト用）
    public bool DamagedFlag
    {
        set
        {
            damaged_flag = value;
        }
    }

    //セッター（ステージクリア）
    public bool StageClearFlag
    {
        set
        {
            stage_clear_flag = true;
        }
    }

    //プレイヤーライフを表示するUI
    public GameObject LifeTextUI
    {
        get
        {
            return LifeText;
        }
    }

    //プレイヤーライフを表示するUI
    public GameObject DamageImageUI
    {
        get
        {
            return DamageImage;
        }
    }
}