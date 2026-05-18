using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Sound
{
    Effect,
    Bgm
}

public class CAudio
{
    private AudioSource _audioSource;
    private Sound _type;

    private void InitVolume(Scene prev, Scene next)
    {
        if (_audioSource != null)
            switch (_type)
            {
                case Sound.Effect:
                    //볼륨 초기화
                    SetVolume(SoundManager.Instance.Volume.Effect);
                    break;

                case Sound.Bgm:
                    //볼륨 초기화
                    SetVolume(SoundManager.Instance.Volume.BGM);
                    break;
            }
        else
            Clear();
    }

    private void SetVolume(float value)
    {
        _audioSource.volume = value;
        _audioSource.pitch = 1;
    }

    public void SetPitch(float value)
    {
        _audioSource.pitch = value;
    }

    public CAudio(AudioSource a, Sound type)
    {
        _audioSource = a;
        _type = type;
        switch (_type)
        {
            case Sound.Effect:
                SoundManager.Instance.EffectAction -= SetVolume;
                SoundManager.Instance.EffectAction += SetVolume;
                SetVolume(SoundManager.Instance.Volume.Effect);
                break;

            case Sound.Bgm:
                SoundManager.Instance.BGMAction -= SetVolume;
                SoundManager.Instance.BGMAction += SetVolume;
                SetVolume(SoundManager.Instance.Volume.BGM);
                break;
        }

        SceneManager.activeSceneChanged -= InitVolume;
        SceneManager.activeSceneChanged += InitVolume;
    }

    public void PlaySound(AudioClip clip, float pitch = 1f)
    {
        if (clip != null)
        {
            PlaySound(clip, _type, pitch);
        }

        else
        {
            SetPitch(pitch);
        }
    }

    public void PlaySound(AudioClip clip, Sound type, float pitch = 1f)
    {

        if (_audioSource.isPlaying)
        {
            if (_audioSource.clip != clip)
                _audioSource.Stop();
            else
                return;
        }

        _audioSource.pitch = pitch;

        if (type == Sound.Bgm)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }

        else
            _audioSource.PlayOneShot(clip);
    }

    public void StopSoundFade(float time = 1.5f)
    {
        _audioSource.DOFade(0, time);
    }

    public void StopSound()
    {
        if (_audioSource.isPlaying)
            _audioSource.Stop();
    }

    public void Clear()
    {
        SceneManager.activeSceneChanged -= InitVolume;
        SoundManager.Instance.EffectAction -= SetVolume;
        SoundManager.Instance.BGMAction -= SetVolume;
        _audioSource = null;
    }
}