using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

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
        //플레이어의 사운드 설정 불러오기
        if (PlayerPrefs.HasKey("Bgm") && PlayerPrefs.HasKey("Sfx"))
        {
            bgmValue.value = PlayerPrefs.GetFloat("Bgm") * 10.0f;
            sfxValue.value = PlayerPrefs.GetFloat("Sfx") * 10.0f;
        }

        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            //모바일 환경이 아니라면 모바일UI를 비활성화
            mobileController.gameObject.SetActive(false);
            switchControl.gameObject.SetActive(false);
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
        hpGage.fillAmount = GameManager.instance.player.currentHP / GameManager.instance.player.health;

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

    public void SwitchControl()
    {
        //레이아웃 그룹에서 위치를 교환
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