using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SuccessPopup : FinishActionPopup
{
    [SerializeField] private SpriteAtlas atlas;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI comment;

    public override void Show()
    {
        base.Show();

        var id2 = Mathf.Clamp(AccountManager.Instance.gamePoint / 100 + 1, 1, 35);
        var book = DataManager.Instance.GetCompleteBook(id2);
        comment.text = book.Txt;
        image.sprite = atlas.GetSprite($"Result{book.Level}");
        
        AccountManager.Instance.AddCollection(id2);
    }
}