using UnityEngine;

public class Ending : CutScene
{
    protected override void GoToLobby()
    {
        AccountManager.Instance.SaveEnding();
        
        base.GoToLobby();
    }
}