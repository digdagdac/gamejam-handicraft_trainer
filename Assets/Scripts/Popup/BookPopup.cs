using UnityEngine;
using UnityEngine.UI;

public class BookPopup : BasePopup
{
    [SerializeField] private Sprite[] samples;
    [SerializeField] private Image image;

    public override void Show()
    {
        base.Show();

        image.sprite = samples[Random.Range(0, samples.Length)];
    }
}