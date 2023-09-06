using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    public RectTransform result_ui; //결과창 ui
    public RectTransform pause_ui;  //게임 일시중지 ui
    public Image hpGage;    //hp바
    public Text scoreText;  //현재 점수 텍스트

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
        //플레이어의 사운드 설정 불러오기
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

    //UI최신화
    void UpdateUI()
    {       
        //현재 체력 업데이트
        hpGage.fillAmount = GameManager.instance.currentHp / GameManager.instance.maxHp;

        //설정창에서 소리가 0일때, 핸들 아이콘 변경
        if (SoundManager.instance.bgmVolume <= 0)
            bgmHandle.sprite = bgmHandleImages[1];
        else
            bgmHandle.sprite = bgmHandleImages[0];

        if (SoundManager.instance.sfxVolume <= 0)
            sfxHandle.sprite = sfxHandleImages[1];
        else
            sfxHandle.sprite = sfxHandleImages[0];
    }

    //Pause 창에서 버튼 이미지 교체
    public void SwitchControl()
    {      
        Sprite sprtie = icons[0].sprite;
        icons[0].sprite = icons[1].sprite;
        icons[1].sprite = sprtie;
    }

    //게임오버 후 점수 최신화
    public void ShowScore()
    {       
        currentScore.text = GameManager.instance.currentScore.ToString("N0");
        highScore.text = PlayerPrefs.GetInt("HighScore").ToString("N0");
    }
}