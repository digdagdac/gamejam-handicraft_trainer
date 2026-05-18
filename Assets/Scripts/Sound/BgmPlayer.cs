using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmPlayer : MonoBehaviour
{
    private CAudio _cAudio;
    [SerializeField]
    private AudioClip _introBGMclip;
    [SerializeField]
    private AudioClip _gameBGMclip;
    [SerializeField]
    private AudioClip _EndingBGMclip;
    [SerializeField]
    private AudioSource _audio;

    public static Action<float> Pitch;

    private static BgmPlayer _instance;
    public static BgmPlayer Bgm { get { return _instance; } }

    void Start()
    {
        if (_instance == null)
            Init();
        else
            Destroy(this);
    }

    private void Init()
    {
        _instance = this;
        _audio = GetComponent<AudioSource>();
        _cAudio = new CAudio(_audio, Sound.Bgm);
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += SelectBgm;
        //SceneManager.activeSceneChanged += BgmVolumeFade;
        Pitch -= _cAudio.SetPitch;
        Pitch += _cAudio.SetPitch;
        _cAudio.PlaySound(_introBGMclip, Sound.Bgm);
    }

    private void BgmVolumeFade(Scene prev, Scene next)
    {
        _cAudio.StopSoundFade();
    }

    private void SelectBgm(Scene prev, Scene next)
    {
        switch (next.name)
        {
            case "0.Intro":
                _cAudio.PlaySound(_introBGMclip);
                break;

            case "1.Lobby":
                _cAudio.PlaySound(_gameBGMclip);
                break;
                
            case "99.Ending":
                _cAudio.PlaySound(_EndingBGMclip);
                break;
        }
    }

    public void BgmTrigger(AudioClip clip, float pitch = 1.0f)
    {
        if (clip != null)
            _cAudio.PlaySound(clip, Sound.Bgm, pitch);
        else
            _cAudio.SetPitch(pitch);
    }
}

