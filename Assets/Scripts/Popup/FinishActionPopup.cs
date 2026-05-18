using System;

public class FinishActionPopup : BasePopup
{
    private Action onFinish;
    
    public override void OnClickClose()
    {
        base.OnClickClose();
        
        onFinish?.Invoke();
        onFinish = null;
    }

    public void Show(Action onFinish)
    {
        Show();

        this.onFinish = onFinish;
    }
}