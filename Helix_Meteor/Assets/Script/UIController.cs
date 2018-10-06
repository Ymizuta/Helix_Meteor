using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour {

    [SerializeField] GameObject prefav_player = null;
    [SerializeField] GameObject Player_ = null;
    [SerializeField] Camera meteor_camera = null;
    [SerializeField] GameObject MainPanel_ = null;
    [SerializeField] GameObject StartButton_ = null;
    [SerializeField] GameObject ContinueButton_ = null;
    [SerializeField] GameObject RetryButton_ = null;
    [SerializeField] Controller Controller_ = null;

    private Vector3 dying_position;
    public bool ui_flag;

    private void Start()
    {
        StartButton_.GetComponent<Button>().onClick.AddListener(PushStartButton);
        //        StartButton_.onClick.AddListener(PushStartButton);
        ContinueButton_.GetComponent<Button>().onClick.AddListener(PushContinueButton);
        //ContinueButton_.onClick.AddListener(PushContinueButton);
        RetryButton_.GetComponent<Button>().onClick.AddListener(PushRetryButton);
        //RetryButton_.onClick.AddListener(PushRetryButton);
    }

    private void Update()
    {
        if(prefav_player  == null)
        {
            MainPanel_.SetActive(true);
        }
        if (ui_flag)
        {
            ContinueButton_.SetActive(true);
            RetryButton_.SetActive(true);
        }
    }

    //スタートボタン押下時の処理
    public void PushStartButton()
    {
        //if(Player_ == null)
        //{
        //    return;
        //}
        Vector3 start_position = new Vector3(0, 0, 0);
        GameObject new_player = Instantiate(prefav_player, start_position, gameObject.transform.rotation) as GameObject;
        meteor_camera.GetComponent<MeteorCamera>().PlayerObjPro = new_player;
        Controller_.PlayerObjProp = new_player;
        new_player.SetActive(true);
        MainPanel_.SetActive(false);
        StartButton_.SetActive(false);
    }
    //コンティニューボタン押下時の処理
    public void PushContinueButton()
    {
        Debug.Log(dying_position);
        Vector3 continue_position = dying_position;
        GameObject new_player = Instantiate(prefav_player, continue_position, gameObject.transform.rotation) as GameObject;
        meteor_camera.GetComponent<MeteorCamera>().PlayerObjPro = new_player;
        Controller_.PlayerObjProp = new_player;

        MainPanel_.SetActive(false);
        RetryButton_.SetActive(false);
        ContinueButton_.SetActive(false);
        ui_flag = false;
        
        //ノーダメージ期間の実装

        new_player.SetActive(true);
    }
    //リトライボタン押下時の処理
    public void PushRetryButton()
    {
        Vector3 retry_position = new Vector3(0,0,0);
        GameObject new_player = Instantiate(prefav_player,retry_position,gameObject.transform.rotation) as GameObject;
        meteor_camera.GetComponent<MeteorCamera>().PlayerObjPro = new_player;
        Controller_.PlayerObjProp = new_player;

        MainPanel_.SetActive(false);
        RetryButton_.SetActive(false);
        ContinueButton_.SetActive(false);
        ui_flag = false;

        new_player.SetActive(true);
    }

    public Vector3 DyingPosition
    {
        set
        {
            dying_position = value;
        }
    }

    public GameObject RetryButton
    {
        get
        {
            return RetryButton_;
        }
    }

    public GameObject ContinueButton
    {
        get
        {
            return ContinueButton_;
        }

    }

}