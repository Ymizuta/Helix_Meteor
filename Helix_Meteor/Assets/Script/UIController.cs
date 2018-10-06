using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour {

    [SerializeField] GameObject prefav_player = null;
    [SerializeField] GameObject Player_ = null;
    [SerializeField] Camera meteor_camera = null;

    //表示UI（ライフ、タイム等）
    [SerializeField] GameObject LifeText = null;
    [SerializeField] GameObject PlayTimeText = null;
    //操作UI（スタート、リトライ等のボタン）
    [SerializeField] GameObject MainPanel_ = null;
    [SerializeField] GameObject StartButton_ = null;
    [SerializeField] GameObject ContinueButton_ = null;
    [SerializeField] GameObject RetryButton_ = null;
    [SerializeField] Controller Controller_ = null;

    private bool time_count_flag = false;
    private float play_time_minute;
    private float play_time_seconds;
    private Vector3 dying_position;
    private bool player_die_flag;

    private void Start()
    {
        //各UIボタンに関数を割り当て
        StartButton_.GetComponent<Button>().onClick.AddListener(PushStartButton);
        ContinueButton_.GetComponent<Button>().onClick.AddListener(PushContinueButton);
        RetryButton_.GetComponent<Button>().onClick.AddListener(PushRetryButton);
    }

    private void Update()
    {
        //時間計測
        if (time_count_flag)
        {
            CountTime();
        }
        
        //プレイヤー死亡を検知（Player.csから受け取った値で検知）
        if (player_die_flag)
        {
            time_count_flag = false;
            MainPanel_.SetActive(true);
            ContinueButton_.SetActive(true);
            RetryButton_.SetActive(true);
        }
    }

    //スタートボタン押下時の処理
    private void PushStartButton()
    {
        //if(Player_ == null)
        //{
        //    return;
        //}
        Vector3 start_position = new Vector3(0, 0, 0);
        GameObject new_player = Instantiate(prefav_player, start_position, gameObject.transform.rotation) as GameObject;
        meteor_camera.GetComponent<MeteorCamera>().PlayerObjPro = new_player;
        Controller_.PlayerObjProp = new_player;

        //タイム計測
        time_count_flag = true;

        new_player.SetActive(true);
        MainPanel_.SetActive(false);
        StartButton_.SetActive(false);
    }
    //コンティニューボタン押下時の処理
    private void PushContinueButton()
    {
        Debug.Log(dying_position);
        Vector3 continue_position = dying_position;
        GameObject new_player = Instantiate(prefav_player, continue_position, gameObject.transform.rotation) as GameObject;
        meteor_camera.GetComponent<MeteorCamera>().PlayerObjPro = new_player;
        Controller_.PlayerObjProp = new_player;

        //タイム計測
        time_count_flag = true;

        MainPanel_.SetActive(false);
        RetryButton_.SetActive(false);
        ContinueButton_.SetActive(false);
        player_die_flag = false;

        //ノーダメージ期間の実装
        new_player.GetComponent<Player>().NoDamageModeOn();

        new_player.SetActive(true);
    }
    //リトライボタン押下時の処理
    private void PushRetryButton()
    {
        Vector3 retry_position = new Vector3(0,0,0);
        GameObject new_player = Instantiate(prefav_player,retry_position,gameObject.transform.rotation) as GameObject;
        meteor_camera.GetComponent<MeteorCamera>().PlayerObjPro = new_player;
        Controller_.PlayerObjProp = new_player;

        //タイム計測
        time_count_flag = true;
        ResetTime();

        MainPanel_.SetActive(false);
        RetryButton_.SetActive(false);
        ContinueButton_.SetActive(false);
        player_die_flag = false;

        new_player.SetActive(true);
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

    private void ResetTime()
    {
        play_time_minute = 0;
        play_time_seconds = 0;
    }

    //セッター（プレイヤー死亡時にコンティニュー用に位置を受け取る）
    public Vector3 DyingPosition
    {
        set
        {
            dying_position = value;
        }
    }

    //セッター（プレイヤー死亡時に死亡検知用にフラグを受け取る）
    public bool PlayerDieFlag
    {
        set
        {
            player_die_flag = value;
        }
    }

}