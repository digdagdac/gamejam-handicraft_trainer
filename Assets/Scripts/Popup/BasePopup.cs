using UnityEngine;

public class BasePopup : MonoBehaviour
{
    public virtual void OnClickClose()
    {
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}