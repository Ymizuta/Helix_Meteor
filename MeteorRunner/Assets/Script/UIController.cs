using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class UIController : MonoBehaviour {

    //[SerializeField] GameObject Player_ = null;
    //[SerializeField] Camera meteor_camera = null;
    //[SerializeField] Controller Controller_ = null;

    //表示UI（ライフ、タイム等）
    [SerializeField] GameObject life_text = null;
    [SerializeField] GameObject play_time_text = null;
    [SerializeField] GameObject clear_message_ui= null;
    [SerializeField] GameObject game_over_message_ui = null;
    [SerializeField] GameObject best_time_ui = null;
    private string clear_message;
    private string normal_clear_message = "STAGE CLEAR";
    private string all_clear_message = "CONGRATULATION!";
    [SerializeField] GameObject stage_name = null;
    [SerializeField] GameObject invincible_point_gauge;
    //操作UI（スタート、リトライ等のボタン）
    [SerializeField] GameObject MainPanel_ = null;
    [SerializeField] GameObject StartButton_ = null;
    [SerializeField] GameObject start_button_text_ = null;
    [SerializeField] GameObject ContinueButton_ = null;
    [SerializeField] GameObject RetryButton_ = null;
    [SerializeField] GameObject NextStageButton_ = null;
    private Text start_text_component; 
    //エフェクト関連
    [SerializeField] GameObject DamageImage = null;
    private Image damage_img;
    //プレイヤークラスから値を受け取る変数
    private Vector3 dying_position;                         //プレイヤー消滅位置
    private bool stage_clear_flag;                          //ステージクリアのフラグ
    //コールバック関数
    public System.Action OnStartButton;
    public System.Action OnContinueButton;
    public System.Action OnRetryButton;
    public System.Action OnNextStageButton;
    //UI表示用のディクショナリ(MenuUiActive用)
    private Dictionary<ButtonType, GameObject> buttons = new Dictionary<ButtonType, GameObject>();
    //キー
    public enum ButtonType
    {
        None,
        Start,
        Continue,
        Retry,
        NextStage,
        Panel
    }

    private void Start()
    {
        //各UIボタンに関数を割り当て
        StartButton_.GetComponent<Button>().onClick.AddListener(PushStartButton);
        ContinueButton_.GetComponent<Button>().onClick.AddListener(PushContinueButton);
        RetryButton_.GetComponent<Button>().onClick.AddListener(PushRetryButton);
        NextStageButton_.GetComponent<Button>().onClick.AddListener(PushNextStageButton);
        //ダメージエフェクト用のImage設定
        damage_img = DamageImage.GetComponent<Image>();
        damage_img.color = Color.clear;
        //設定
        start_text_component = start_button_text_.GetComponent<Text>();
        //ディクショナリ登録
        buttons[ButtonType.None] = null;
        buttons[ButtonType.Start] = StartButton_;
        buttons[ButtonType.Continue] = ContinueButton_;
        buttons[ButtonType.Retry] = RetryButton_;
        buttons[ButtonType.NextStage] = NextStageButton_;
        buttons[ButtonType.Panel] = MainPanel_;
    }

    private void Update()
    {
        //画面の色をリセット
        DamageEffectReset();
        //スタートボタン点滅
        start_text_component.color = new Color(255,0,0, Mathf.PingPong(Time.time, 1));
        //クリア時の処理
        if (stage_clear_flag)
        {
            //クリアのメッセージ表示を点滅させる           
            clear_message_ui.GetComponent<Text>().text = clear_message;
            clear_message_ui.SetActive(true);
            clear_message_ui.GetComponent<Text>().color = new Color(255f,247f,0f,Mathf.PingPong(Time.time,1));
            return;
        }else if (stage_clear_flag == false)
        {
            clear_message_ui.SetActive(false);
        }
    }
    //クリアメッセージの設定(ノーマル)
    public void SetClearMessageNormal()
    {
        clear_message = normal_clear_message;
    }

    //クリアメッセージの設定(全クリ)
    public void SetClearMessageAll()
    {
        clear_message = all_clear_message;
    }

    //ゲームオーバーメッセージ表示
    public void GameOverMessageActive()
    {
        game_over_message_ui.SetActive(true);
    }

    //スタートボタン押下時の処理
    private void PushStartButton()
    {
        //UI非表示
        MainPanel_.SetActive(false);
        StartButton_.SetActive(false);
        stage_name.SetActive(false);
        best_time_ui.SetActive(false);
        //コールバック（player生成等の処理を実行）
        if (OnStartButton != null)
        {
            OnStartButton();
        }
    }
    //コンティニューボタン押下時の処理
    private void PushContinueButton()
    {
        //UI表示
        MainPanel_.SetActive(false);
        RetryButton_.SetActive(false);
        ContinueButton_.SetActive(false);
        stage_name.SetActive(false);
        game_over_message_ui.SetActive(false);
        //コールバック（player生成等の処理を実行）
        if (OnContinueButton != null)
        {
            OnContinueButton();
        }
    }
    //リトライボタン押下時の処理
    private void PushRetryButton()
    {
        //UI表示
        MainPanel_.SetActive(false);
        RetryButton_.SetActive(false);
        ContinueButton_.SetActive(false);
        NextStageButton_.SetActive(false);
        stage_name.SetActive(false);
        game_over_message_ui.SetActive(false);
        best_time_ui.SetActive(false);
        //ステージクリア→リトライ時に「Clear」の文字を表示させないため
        ClearFlagOff();
        //コールバック（player生成等の処理を実行）
        if (OnRetryButton != null)
        {
            OnRetryButton();
        }
    }
    //ネクストステージボタン押下時の処理
    private void PushNextStageButton()
    {
        //UI表示
        MainPanel_.SetActive(false);
        RetryButton_.SetActive(false);
        ContinueButton_.SetActive(false);
        NextStageButton_.SetActive(false);
        best_time_ui.SetActive(false);
        //ステージクリア→リトライ時に「Clear」の文字を表示させないため
        ClearFlagOff();
        //コールバック
        if (OnNextStageButton != null)
        {
            OnNextStageButton();
        }
    }
    //メニューUIを表示する
    public void MenuUiActive(ButtonType button_type_)
    {
        buttons[button_type_].SetActive(true);
    }
    
    //プレイヤーライフをUIに反映
    public void PlayerLifeUI(int player_life)
    {
        life_text.GetComponent<Text>().text = "LIFE × " + player_life.ToString();
    }

    //無敵モードポイントをUIに反映
    public void InvinciblePointUI(float invincible_point_)
    {
        invincible_point_gauge.GetComponent<Slider>().value = invincible_point_;
    }

    internal void MenuUiActive(UIController ui_controller_)
    {
        throw new NotImplementedException();
    }

    internal void MenuUiActive(Action start)
    {
        throw new NotImplementedException();
    }

    //タイマーUIに時間を反映
    public void CountTimeUi(float play_time_minute_,float play_time_seconds_)
    {
        play_time_text.GetComponent<Text>().text = play_time_minute_.ToString("00") + ":" + play_time_seconds_.ToString("00");
    }

    //ベストタイム表示(後で処理を統合する)
    public void BestTimeActive()
    {
        best_time_ui.SetActive(true);
    }

    //ベストタイムを表示(フラグでメッセージが変化)
    public void BestTimeUi(float best_time_minute_,float best_time_seconds_,bool new_record_flag_)
    {
        if (new_record_flag_)
        {
            string best_score_message = "NEW RECORD!";
            best_time_ui.GetComponent<Text>().text = best_score_message + "\n" + best_time_minute_.ToString("00") + ":" + best_time_seconds_.ToString("00");
            return;
        }
        else if(new_record_flag_ == false)
        {
            string best_score_message = "BEST TIME";
            best_time_ui.GetComponent<Text>().text = best_score_message + "\n" + best_time_minute_.ToString("00") + ":" + best_time_seconds_.ToString("00");
        }
    }

    //ステージ名を表示
    public void StageNameActive(int stage_number)
    {
        stage_name.GetComponent<Text>().text = "STAGE "+ stage_number.ToString("00");
        stage_name.SetActive(true);
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

    //プレイヤーライフを表示するUI
    public GameObject LifeTextUI
    {
        get
        {
            return life_text;
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
    //クリアフラグON
    public void ClearFlagOn()
    {
        stage_clear_flag = true;
    }
    //クリアフラグOFF
    public void ClearFlagOff()
    {
        stage_clear_flag = false;
    }
}