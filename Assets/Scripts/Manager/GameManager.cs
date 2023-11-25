using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public MobileController mobileController;

    public Player player;   //플레이어
    public BackgroundScrolling map; //스크롤링할 맵
    public CameraShake cameraShake; //카메라쉐이크

    public Enemy[] enemys;  //적을 종류별로 모아놓은 배열    

    public int stage { get; private set; }  //현재 스테이지

    public int currentScore = 0;    //현재점수
    public int highScore { get; private set; }  //최고점수
    public float gameSpeed { get; private set; }    //게임의 배속
    public bool isPause { get; private set; }   //일시정지 여부

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        GameStart();
    }

    void Update()
    {
        //스코어 최신화
        UIManager.instance.scoreText.text = currentScore.ToString("N0");
    }

    public void GameStart()
    {
        //저장된 최고점수를 불러온다. (없으면 0점)
        highScore = PlayerPrefs.GetInt("HighScore");
        currentScore = 0;
        stage = 0;

        if (player.opponent != null)
            PoolManager.instance.Return(player.opponent.gameObject);
        player.opponent = null;

        SoundManager.instance.PlayBgm(true);
      
        gameSpeed = 1.0f;
        Time.timeScale = gameSpeed;
        UIManager.instance.result_ui.gameObject.SetActive(false);

        player.Init();
        map.Init();

        NextStage();
    }

    public void StartBattle()
    {
        //플레이어, 적 위치 고정
        player.transform.position = new Vector3(-1, player.transform.position.y, 0);
        player.opponent.transform.SetParent(null);
        player.opponent.transform.position = new Vector3(1, player.opponent.transform.position.y, 0);

        //스크립트 활성화
        player.GetComponent<Player>().enabled = true;
        if (mobileController.gameObject.activeSelf) { mobileController.ControlEnable(); }
        player.opponent.GetComponent<Enemy>().enabled = true;
        player.opponent.Init();
    }

    //게임오버 되었을 때, UI 출력
    public void GameOver()
    {
        SoundManager.instance.PlayBgm(false);

        PoolManager.instance.Return(player.opponent.gameObject);

        if (currentScore > highScore)
            PlayerPrefs.SetInt("HighScore", currentScore);

        UIManager.instance.ShowScore();
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

        player.GetComponent<Player>().enabled = false;
        if (mobileController.gameObject.activeSelf) { mobileController.ControlDisable(); }
        map.Scrolling();
    }

    //적 세팅
    public void SetEnemy()
    {
        //스테이지가 2단위로 오를수록 나오는 몬스터 추가되고
        //4단위마다 이전 몬스터가 등장하지않음
        int min = Mathf.Max(0, Mathf.Clamp(stage / 4, 0, enemys.Length - 1));
        int max = Mathf.Min(enemys.Length, stage / 2);
        int random = Random.Range(min, max);
        //int => enum => string
        player.opponent = PoolManager.instance.Get(((EnemyName)random).ToString()).GetComponent<Enemy>();
        player.opponent.transform.SetParent(map.backgrounds[2].transform, false);
    }

    //게임 일시중지
    public void Pause()
    {
        if (isPause == false)
            GamePauseOn();
        else if (isPause == true)
            GamePauseOff();
    }
   
    public void GamePauseOn()
    {
        isPause = true;
        UIManager.instance.pause_ui.gameObject.SetActive(true);
        Time.timeScale = 0;
        cameraShake.PauseVibrate(true);
    }

    public void GamePauseOff()
    {
        isPause = false;
        UIManager.instance.pause_ui.gameObject.SetActive(false);
        Time.timeScale = gameSpeed;
        cameraShake.PauseVibrate(false);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}