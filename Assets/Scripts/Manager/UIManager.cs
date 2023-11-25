using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public RectTransform result_ui; //���â ui
    public RectTransform pause_ui;  //���� �Ͻ����� ui
    public Image hpGage;    //hp��
    public Text scoreText;  //���� ���� �ؽ�Ʈ

    [Header("Result")]
    public Text currentScore;
    public Text highScore;

    [Header("Bgm")]
    public Slider bgmValue;
    public Image bgmHandle;
    public Sprite[] bgmHandleImages;
    [Header("Sfx")]
    public Slider sfxValue;
    public Image sfxHandle;
    public Sprite[] sfxHandleImages;
    [Header("Control")]
    public MobileController mobileController;
    public Transform switchControl;
    public Image[] moibleIcons;

    void Awake()
    {
        if (instance == null)
            instance = this;

        Init();
    }

    public void Init()
    {
        //�÷��̾��� ���� ���� �ҷ�����
        if (PlayerPrefs.HasKey("Bgm") && PlayerPrefs.HasKey("Sfx"))
        {
            bgmValue.value = PlayerPrefs.GetFloat("Bgm") * 10.0f;
            sfxValue.value = PlayerPrefs.GetFloat("Sfx") * 10.0f;
        }

        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            //����� ȯ���� �ƴ϶�� �����UI�� ��Ȱ��ȭ
            mobileController.gameObject.SetActive(false);
            switchControl.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        UpdateUI();
    }

    //UI�ֽ�ȭ
    void UpdateUI()
    {
        //���� ü�� ������Ʈ
        hpGage.fillAmount = GameManager.instance.player.currentHP / GameManager.instance.player.health;

        //����â���� �Ҹ��� 0�϶�, �ڵ� ������ ����
        if (SoundManager.instance.bgmVolume <= 0)
            bgmHandle.sprite = bgmHandleImages[1];
        else
            bgmHandle.sprite = bgmHandleImages[0];

        if (SoundManager.instance.sfxVolume <= 0)
            sfxHandle.sprite = sfxHandleImages[1];
        else
            sfxHandle.sprite = sfxHandleImages[0];
    }

    public void SwitchControl()
    {
        //���̾ƿ� �׷쿡�� ��ġ�� ��ȯ
        int left = moibleIcons[0].transform.GetSiblingIndex();
        int right = moibleIcons[1].transform.GetSiblingIndex();

        moibleIcons[0].transform.SetSiblingIndex(right);
        moibleIcons[1].transform.SetSiblingIndex(left);
    }

    public void ShowScore()
    {
        result_ui.gameObject.SetActive(true);
        currentScore.text = GameManager.instance.currentScore.ToString("N0");
        highScore.text = PlayerPrefs.GetInt("HighScore").ToString("N0");
    }
}