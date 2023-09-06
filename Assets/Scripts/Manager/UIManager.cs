using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

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
    public Image[] icons;

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (instance == null)
            instance = this;
        //�÷��̾��� ���� ���� �ҷ�����
        if (PlayerPrefs.HasKey("Bgm") && PlayerPrefs.HasKey("Sfx"))
        {
            bgmValue.value = PlayerPrefs.GetFloat("Bgm") * 10.0f;
            sfxValue.value = PlayerPrefs.GetFloat("Sfx") * 10.0f;
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
        hpGage.fillAmount = GameManager.instance.currentHp / GameManager.instance.maxHp;

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

    //Pause â���� ��ư �̹��� ��ü
    public void SwitchControl()
    {      
        Sprite sprtie = icons[0].sprite;
        icons[0].sprite = icons[1].sprite;
        icons[1].sprite = sprtie;
    }

    //���ӿ��� �� ���� �ֽ�ȭ
    public void ShowScore()
    {       
        currentScore.text = GameManager.instance.currentScore.ToString("N0");
        highScore.text = PlayerPrefs.GetInt("HighScore").ToString("N0");
    }
}