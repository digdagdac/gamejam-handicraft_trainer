using UnityEngine;

public class SettingExitPopup : BasePopup
{
    public void OnClickYes()
    {
        Application.Quit();
    }
}