using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public MobileController mobileController;

    public Player player;   //�÷��̾�
    public BackgroundScrolling map; //��ũ�Ѹ��� ��
    public CameraShake cameraShake; //ī�޶���ũ

    public Enemy[] enemys;  //���� �������� ��Ƴ��� �迭    

    public int stage { get; private set; }  //���� ��������

    public int currentScore = 0;    //��������
    public int highScore { get; private set; }  //�ְ�����
    public float gameSpeed { get; private set; }    //������ ���
    public bool isPause { get; private set; }   //�Ͻ����� ����

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
        //���ھ� �ֽ�ȭ
        UIManager.instance.scoreText.text = currentScore.ToString("N0");
    }

    public void GameStart()
    {
        //����� �ְ������� �ҷ��´�. (������ 0��)
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
        //�÷��̾�, �� ��ġ ����
        player.transform.position = new Vector3(-1, player.transform.position.y, 0);
        player.opponent.transform.SetParent(null);
        player.opponent.transform.position = new Vector3(1, player.opponent.transform.position.y, 0);

        //��ũ��Ʈ Ȱ��ȭ
        player.GetComponent<Player>().enabled = true;
        if (mobileController.gameObject.activeSelf) { mobileController.ControlEnable(); }
        player.opponent.GetComponent<Enemy>().enabled = true;
        player.opponent.Init();
    }

    //���ӿ��� �Ǿ��� ��, UI ���
    public void GameOver()
    {
        SoundManager.instance.PlayBgm(false);

        PoolManager.instance.Return(player.opponent.gameObject);

        if (currentScore > highScore)
            PlayerPrefs.SetInt("HighScore", currentScore);

        UIManager.instance.ShowScore();
    }

    //���� ����ġ���� ���� ������ �̵�
    public void NextStage()
    {
        //�������� ����
        stage++;

        //10������������ 10%������ ���ǵ� ����
        gameSpeed = 1.0f + (stage / 10) * 0.1f;
        Time.timeScale = gameSpeed;

        SetEnemy();

        player.GetComponent<Player>().enabled = false;
        if (mobileController.gameObject.activeSelf) { mobileController.ControlDisable(); }
        map.Scrolling();
    }

    //�� ����
    public void SetEnemy()
    {
        //���������� 2������ �������� ������ ���� �߰��ǰ�
        //4�������� ���� ���Ͱ� ������������
        int min = Mathf.Max(0, Mathf.Clamp(stage / 4, 0, enemys.Length - 1));
        int max = Mathf.Min(enemys.Length, stage / 2);
        int random = Random.Range(min, max);
        //int => enum => string
        player.opponent = PoolManager.instance.Get(((EnemyName)random).ToString()).GetComponent<Enemy>();
        player.opponent.transform.SetParent(map.backgrounds[2].transform, false);
    }

    //���� �Ͻ�����
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