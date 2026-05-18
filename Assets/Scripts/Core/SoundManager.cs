using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum SFX
{
    Achive,
    Click,
    Drum,
    Fail,
    Hammer,
    Keyboard,
    Magma,
    Meow,
    Page,
    Ppok,
    Shave,
    SSG,
    Stamp,
    Success,
    Wrong
}

public class SoundManager
{
    public static SoundManager Instance { 
        get 
        {
            if (s_instance == null)
                s_instance = new SoundManager();
            return s_instance;
        } 
    }

    private static SoundManager s_instance;

    public SoundVolume Volume { get => _volume; }
    private SoundVolume _volume;

    public Dictionary<SFX, AudioClip> SFXs { get => _sfxs; }
    private Dictionary<SFX, AudioClip> _sfxs = new Dictionary<SFX, AudioClip>();

    public event Action<float> BGMAction;
    public event Action<float> EffectAction;

    public SoundManager()
    {
        _volume = LoadVolume(Path.Combine(Application.persistentDataPath, "volume"));
        BGMAction?.Invoke(_volume.BGM);
        EffectAction?.Invoke(_volume.Effect);
        AudioClip[] sfx = Resources.LoadAll<AudioClip>("Sound/SFX");
        var sfxs = Enum.GetValues(typeof(SFX)) as SFX[];

        for (int i = 0; i < sfx.Length; ++i)
            _sfxs.Add(sfxs[i], sfx[i]);
    }

    public void Play2DSound(SFX type)
    {
        AudioSource.PlayClipAtPoint(_sfxs[type], Vector3.zero, Volume.Effect);
    }

    public void Play2DSound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
    }

    public void SaveSound(SoundVolume vol)
    {
        string voldata = JsonUtility.ToJson(vol);
        string path = Path.Combine(Application.persistentDataPath, "volume");
        File.WriteAllText(path, voldata);
        _volume = vol;
        BGMAction?.Invoke(_volume.BGM);
        EffectAction?.Invoke(_volume.Effect);
    }

    public void SetVolumeTemporary(float vol, Sound type)
    {
        if (type == Sound.Bgm)
            BGMAction?.Invoke(vol);
        else
            EffectAction?.Invoke(vol);
    }

    private SoundVolume LoadVolume(string path)
    {
        if (File.Exists(path))
        {
            string voldata = File.ReadAllText(path);
            return JsonUtility.FromJson<SoundVolume>(voldata);
        }
        else
        {
            return new SoundVolume(0.3f, 0.3f);
        }
    }
}