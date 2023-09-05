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

    //Melee가 2개이기때문에 Hit부터 값 지정
    public enum Sfx { Melee, Hit = 2, Guard, Parring }

    private void Awake()
    {
        instance = this;
        Init();
    }

    public void Init()
    {
        //배경음악, 효과음 사운드 정보가 둘중 하나라도 없으면 그냥 초기화
        if (PlayerPrefs.HasKey("Bgm") && PlayerPrefs.HasKey("Sfx"))
        {
            bgmVolume = PlayerPrefs.GetFloat("Bgm");
            sfxVolume = PlayerPrefs.GetFloat("Sfx");
        }

        //bgm 초기화
        GameObject bgmObject = new GameObject("bgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        //sfx 초기화
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

    //배경음악 재생
    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
            bgmPlayer.Play();
        else
            bgmPlayer.Stop();
    }

    //효과음 재생
    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            //쉬고있는 채널에서 효과음을 재생
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;
            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            int ranIndex = 0;
            //공격사운드는 2개중에 랜덤재생
            if (sfx == Sfx.Melee)
                ranIndex = Random.Range(0, 2);  //0~1

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    //사운드 설정에서 배경음악과 효과음 크기조절
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