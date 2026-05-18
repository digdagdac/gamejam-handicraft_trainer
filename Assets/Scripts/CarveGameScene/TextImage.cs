using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextImage : MonoBehaviour
{
    private Image _image;
    private RectTransform _rect;

    public void Init()
    {
        _image = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
    }

    public void FadeFeverImage(Sprite sprite, Vector2 pos, Vector3 rotate, Vector2 size)
    {
        _image.sprite = sprite;
        _rect.anchoredPosition = pos;
        _rect.localEulerAngles = rotate;
        _rect.sizeDelta = size;
        DOTween.Sequence()
            .AppendCallback(() => gameObject.SetActive(true))
            .Append(_image.DOColor(Color.white, 0.5f))
            .Join(_rect.DOAnchorPos(_rect.anchoredPosition + new Vector2(Random.Range(-30, 30), Random.Range(-30, 30)), 1))
            .Append(_image.DOColor(new Color(1, 1, 1, 0), 0.5f))
            .AppendCallback(() => gameObject.SetActive(false));
    }

    public void FadeFailImage(Sprite sprite, Vector2 pos, Vector2 size)
    {
        _image.sprite = sprite;
        _rect.anchoredPosition = pos;
        _rect.sizeDelta = size;
        DOTween.Sequence()
            .Append(_image.DOColor(Color.white, 0.5f))
            .Join(_rect.DOSizeDelta(size, 1))
            .Append(_image.DOColor(new Color(1, 1, 1, 0), 0.5f))
            .AppendCallback(() => gameObject.SetActive(false));
    }
}
