using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public MobileController mobileController;

    public Text scoreText;  //���� ������ �ΰ��ӿ� ǥ��
    public RectTransform result_ui; //���â ui
    public RectTransform pause_ui;  //���� �Ͻ����� ui

    public GameObject player;   //�÷��̾�
    public BackgroundScrolling map; //��ũ�Ѹ��� ��
    public CameraShake cameraShake; //ī�޶���ũ

    public Enemy[] enemys;  //���� �������� ��Ƴ��� �迭
    public Enemy currentEnemy; //���� ����ϰ��ִ� ��
    public Enemy nextEnemy; //������ ����� ��

    public int stage { get; private set; }  //���� ��������
    public float maxHp; //�÷��̾��� �ִ�ü��(�����ϸ� ���ӿ���)
    public float _currentHp;   //�÷��̾� ����ü��
    public float currentHp
    {
        get { return _currentHp; }
        set
        {
            _currentHp = value;
            //1. �÷��̾�� ���ӿ����� ����
            if (_currentHp <= 0)
            {
                currentEnemy.GetComponent<Enemy>().enabled = false;
                player.GetComponent<Player>().OnGameOver();
            }
        }
    }
    public int currentScore = 0;    //��������
    public int highScore { get; private set; }  //�ְ�����
    public float gameSpeed { get; private set; }    //������ ���
    public bool isPause { get; private set; }

    void Awake()
    {
        Init();       
    }

    public void Init()
    {
        //�̱���
        if (instance == null)
            instance = this;

        //����� �ְ������� �ҷ��´�. (������ 0��)
        highScore = PlayerPrefs.GetInt("HighScore");
    }


    void Start()
    {
        GameStart();
    }

    void Update()
    {
        //���ھ� �ֽ�ȭ
        scoreText.text = currentScore.ToString("N0");
    }

    //���� ��ġ�Ҷ� ��ũ��Ʈ Ȱ��ȭ
    public void StartBattle()
    {
        //�÷��̾�, �� ��ġ ����
        player.transform.position = new Vector3(-1, player.transform.position.y, 0);
        currentEnemy.transform.SetParent(null);
        currentEnemy.transform.position = new Vector3(1, currentEnemy.transform.position.y, 0);

        //��ũ��Ʈ Ȱ��ȭ
        player.GetComponent<Player>().ControlEnable();
        if (mobileController.gameObject.activeSelf) { mobileController.ControlEnable(); }
        currentEnemy.GetComponent<Enemy>().enabled = true;

    }

    //���ӽ����� ������ �ʱ�ȭ
    public void GameStart()
    {
        if (currentEnemy != null)
            Destroy(currentEnemy.gameObject);

        currentEnemy = null;
        nextEnemy = null;

        map.Init();

        SoundManager.instance.PlayBgm(true);

        stage = 0;

        currentHp = maxHp;
        currentScore = 0;

        gameSpeed = 1.0f;
        Time.timeScale = gameSpeed;

        result_ui.gameObject.SetActive(false);
        GamePauseOff();
        SetRandomEnemy();
        NextStage();
    }

    //���ӿ��� �Ǿ��� ��, UI ���
    public void GameOver()
    {
        SoundManager.instance.PlayBgm(false);

        Destroy(currentEnemy.gameObject);
        if (currentScore > highScore)
            PlayerPrefs.SetInt("HighScore", currentScore);

        UIManager.instance.ShowScore();
        result_ui.gameObject.SetActive(true);
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

        player.GetComponent<Player>().ControlDisable();
        if (mobileController.gameObject.activeSelf) { mobileController.ControlDisable(); }
        map.Scrolling();
    }

    //�� ����
    public void SetEnemy()
    {
        //nextEnemy�� ��ϵǾ��ִ� ���͸� currentEnemy�� ��ȯ
        if (nextEnemy == null)
            SetRandomEnemy();

        nextEnemy.GetComponent<Enemy>().enabled = false;
        currentEnemy = Instantiate(nextEnemy, map.backgrounds[2].transform, false);
        nextEnemy.transform.position = new Vector2(1.0f, nextEnemy.transform.position.y);

        SetRandomEnemy();
    }

    //�������� ����
    public void SetRandomEnemy()
    {
        //���������� 2������ �������� ������ ���� �߰��ǰ�
        //4�������� ���� ���Ͱ� ������������
        int min = Mathf.Max(0, Mathf.Clamp(stage / 4, 0, enemys.Length - 1));
        int max = Mathf.Min(enemys.Length, stage / 2);
        int random = Random.Range(min, max);
        nextEnemy = enemys[random];
    }

    

    public void Pause()
    {
        if (isPause == false)
            GamePauseOn();
        else if (isPause == true)
            GamePauseOff();
    }

    //���� �Ͻ�����
    public void GamePauseOn()
    {
        isPause = true;
        pause_ui.gameObject.SetActive(true);
        Time.timeScale = 0;
        cameraShake.PauseVibrate(true);
    }

    public void GamePauseOff()
    {
        isPause = false;
        pause_ui.gameObject.SetActive(false);
        Time.timeScale = gameSpeed;
        cameraShake.PauseVibrate(false);
    }

    //���� ����
    public void GameQuit()
    {
        Application.Quit();
    }
}