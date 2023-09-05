using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    //Melee�� 2���̱⶧���� Hit���� �� ����
    public enum Sfx { Melee, Hit = 2, Guard, Parring }

    private void Awake()
    {
        instance = this;
        Init();
    }

    public void Init()
    {
        //�������, ȿ���� ���� ������ ���� �ϳ��� ������ �׳� �ʱ�ȭ
        if (PlayerPrefs.HasKey("Bgm") && PlayerPrefs.HasKey("Sfx"))
        {
            bgmVolume = PlayerPrefs.GetFloat("Bgm");
            sfxVolume = PlayerPrefs.GetFloat("Sfx");
        }

        //bgm �ʱ�ȭ
        GameObject bgmObject = new GameObject("bgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        //sfx �ʱ�ȭ
        GameObject sfxObject = new GameObject("sfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    //������� ���
    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
            bgmPlayer.Play();
        else
            bgmPlayer.Stop();
    }

    //ȿ���� ���
    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            //�����ִ� ä�ο��� ȿ������ ���
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;
            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            int ranIndex = 0;
            //���ݻ���� 2���߿� �������
            if (sfx == Sfx.Melee)
                ranIndex = Random.Range(0, 2);  //0~1

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    //���� �������� ������ǰ� ȿ���� ũ������
    public void SetBgm(float volume)
    {
        bgmVolume = Mathf.Round(volume) / 10.0f;
        PlayerPrefs.SetFloat("Bgm", bgmVolume);

        bgmPlayer.volume = bgmVolume;
    }

    private bool SetAudio = false;
    public void SetSfx(float volume)
    {
        sfxVolume = Mathf.Round(volume) / 10.0f;
        PlayerPrefs.SetFloat("Sfx", sfxVolume);

        for (int index = 0; index < sfxPlayers.Length; index++)
            sfxPlayers[index].volume = sfxVolume;

        if (SetAudio)
            PlaySfx(Sfx.Melee);

        SetAudio = true;
    }
}