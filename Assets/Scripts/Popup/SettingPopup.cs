using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : BasePopup
{
    [SerializeField] private SettingResetPopup settingResetPopup;
    [SerializeField] private SettingExitPopup settingExitPopup;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider SFXSlider;

    private void Awake()
    {
        bgmSlider.value = SoundManager.Instance.Volume.BGM;
        SFXSlider.value = SoundManager.Instance.Volume.Effect;
    }

    public void OnBGMValueChanged(Slider slider)
    {
        SoundManager.Instance.SetVolumeTemporary(slider.value, Sound.Bgm);
    }
    
    public void OnSFXValueChanged(Slider slider)
    {
        SoundManager.Instance.SetVolumeTemporary(slider.value, Sound.Effect);
    }
    
    public void OnClickReset()
    {
        settingResetPopup.Show();
    }

    public void OnClickExit()
    {
        settingExitPopup.Show();
    }

    public override void Show()
    {
        base.Show();

        Time.timeScale = 0;
    }

    public override void OnClickClose()
    {
        if (settingResetPopup.IsActive())
        {
            settingResetPopup.OnClickClose();
            return;
        }

        if (settingExitPopup.IsActive())
        {
            settingExitPopup.OnClickClose();
            return;
        }
        
        base.OnClickClose();
        SoundManager.Instance.SaveSound(new SoundVolume(bgmSlider.value, SFXSlider.value));
        Time.timeScale = 1;
    }
}