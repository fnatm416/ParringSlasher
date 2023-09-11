using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public MobileController mobileController;

    public GameObject player;   //�÷��̾�
    public BackgroundScrolling map; //��ũ�Ѹ��� ��
    public CameraShake cameraShake; //ī�޶���ũ

    public Enemy[] enemys;  //���� �������� ��Ƴ��� �迭
    public Enemy currentEnemy; //���� ����ϰ��ִ� ��

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
        currentEnemy.Init();
    }

    //���ӽ����� ������ �ʱ�ȭ
    public void GameStart()
    {
        //����� �ְ������� �ҷ��´�. (������ 0��)
        highScore = PlayerPrefs.GetInt("HighScore");
        stage = 0;

        if (currentEnemy != null)
            PoolManager.instance.Return(currentEnemy.gameObject);
        currentEnemy = null;

        SoundManager.instance.PlayBgm(true);

        map.Init();

        currentHp = maxHp;
        currentScore = 0;

        gameSpeed = 1.0f;
        Time.timeScale = gameSpeed;

        UIManager.instance.result_ui.gameObject.SetActive(false);
        GamePauseOff();
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
        UIManager.instance.result_ui.gameObject.SetActive(true);
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
        //���������� 2������ �������� ������ ���� �߰��ǰ�
        //4�������� ���� ���Ͱ� ������������
        int min = Mathf.Max(0, Mathf.Clamp(stage / 4, 0, enemys.Length - 1));
        int max = Mathf.Min(enemys.Length, stage / 2);
        int random = Random.Range(min, max);
        //int => enum => string
        currentEnemy = PoolManager.instance.Get(((EnemyName)random).ToString()).GetComponent<Enemy>();
        currentEnemy.transform.SetParent(map.backgrounds[2].transform, false);
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

    //���� ����
    public void GameQuit()
    {
        Application.Quit();
    }
}