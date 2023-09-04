using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public RectTransform result_ui; //결과창 ui
    public Text scoreText;  //현재 점수를 인게임에 표시

    public RectTransform pause_ui;  //게임 일시중지 ui
    public bool pause = false;

    public Player player;   //플레이어
    public BackgroundScrolling map; //스크롤링할 맵
    public CameraShake cameraShake; //카메라쉐이크

    public Enemy[] enemys;  //적을 종류별로 모아놓은 배열
    public Enemy currentEnemy; //현재 상대하고있는 적
    public Enemy nextEnemy; //다음에 상대할 적

    public int stage;  //현재 스테이지
    public float maxHp; //플레이어의 최대체력(소진하면 게임오버)
    public float _currentHp;   //플레이어 현재체력
    public float currentHp
    {
        get { return _currentHp; }
        set
        {
            _currentHp = value;
            //1. 플레이어에게 게임오버를 전달
            if (_currentHp <= 0)
            {
                currentEnemy.GetComponent<Enemy>().enabled = false;
                player.OnGameOver();
            }
        }
    }
    public int currentScore = 0;    //현재점수
    public int highScore { get; private set; }  //최고점수
    public float gameSpeed { get; private set; }    //게임의 배속

    void Awake()
    {
        //싱글톤
        if (instance == null)
            instance = this;

        //저장된 최고점수를 불러온다. (없으면 0점)
        highScore = PlayerPrefs.GetInt("HighScore");
    }

    private void Start()
    {
        GameStart();
    }

    private void Update()
    {
        //스코어 최신화
        scoreText.text = currentScore.ToString("N0");
    }

    //적과 대치할때 스크립트 활성화
    public void StartBattle()
    {
        //플레이어, 적 위치 고정
        player.transform.position = new Vector3(-1, player.transform.position.y, 0);
        currentEnemy.transform.SetParent(null);
        currentEnemy.transform.position = new Vector3(1, currentEnemy.transform.position.y, 0);

        //스크립트 활성화
        player.GetComponent<Player>().enabled = true;
        currentEnemy.GetComponent<Enemy>().enabled = true;

    }

    //적을 물리치고나면 다음 적에게 이동
    public void NextStage()
    {
        //스테이지 증가
        stage++;

        //10스테이지마다 10%증가된 스피드 적용
        gameSpeed = 1.0f + (stage / 10) * 0.1f;
        Time.timeScale = gameSpeed;

        SetEnemy();
        //컴포넌트를 껐다켜서 player의 Disable을 발동
        player.GetComponent<Player>().enabled = true;
        player.GetComponent<Player>().enabled = false;
        map.Scrolling();
    }

    //적 세팅
    public void SetEnemy()
    {
        //nextEnemy로 등록되어있던 몬스터를 currentEnemy로 소환
        if (nextEnemy == null)
            SetRandomEnemy();

        nextEnemy.GetComponent<Enemy>().enabled = false;
        currentEnemy = Instantiate(nextEnemy, map.backgrounds[2].transform, false);
        nextEnemy.transform.position = new Vector2(1.0f, nextEnemy.transform.position.y);

        SetRandomEnemy();
    }

    //게임시작할 때마다 초기화
    public void GameStart()
    {
        SoundManager.instance.PlayBgm(true);
        player.GetComponent<PlayerInput>().currentActionMap.actions[2].Enable();    
        pause = false;
        stage = 0;
        currentHp = maxHp;
        currentScore = 0;
        gameSpeed = 1.0f;
        Time.timeScale = gameSpeed;
        result_ui.gameObject.SetActive(false);
        SetRandomEnemy();
        NextStage();
    }

    //랜덤몬스터 생성
    public void SetRandomEnemy()
    {
        //스테이지가 2단위로 오를수록 나오는 몬스터 추가되고
        //4단위마다 이전 몬스터가 등장하지않음
        int min = Mathf.Max(0, Mathf.Clamp(stage / 4, 0, enemys.Length - 1));
        int max = Mathf.Min(enemys.Length, stage / 2);
        int random = Random.Range(min, max);
        nextEnemy = enemys[random];
    }

    //게임오버 되었을 때, UI 출력
    public void GameOver()
    {
        SoundManager.instance.PlayBgm(false);

        Destroy(currentEnemy.gameObject);
        if (currentScore > highScore)
            PlayerPrefs.SetInt("HighScore", currentScore);
        player.GetComponent<PlayerInput>().currentActionMap.actions[2].Disable();   //중지버튼 막기
        result_ui.gameObject.SetActive(true);
    }

    //게임 일시중지
    public void GamePauseOn()
    {
        pause = true;
        pause_ui.gameObject.SetActive(true);
        Time.timeScale = 0;
        SoundManager.instance.BgmEffect(true);
        cameraShake.PauseVibrate(true);
    }

    public void GamePauseOff()
    {
        pause = false;
        pause_ui.gameObject.SetActive(false);
        Time.timeScale = gameSpeed;
        SoundManager.instance.BgmEffect(false);
        cameraShake.PauseVibrate(false);
    }

    public void GamePause()
    {
        if (pause == false)
            GamePauseOn();
        else if (pause == true)
            GamePauseOff();
    }
}