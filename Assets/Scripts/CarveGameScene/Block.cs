using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    private RectTransform _rect;
    private Image[] _image = new Image[5];
    private int[] randnum = new int[5];

    private Sprite[] _afterSprite = new Sprite[5];
    private TextMeshProUGUI _text;
    private int _totalCount;

    public void Init(SpriteAtlas atlas, Vector2 pos, int totalCount, int curCount)
    {
        _rect = GetComponent<RectTransform>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _rect.anchoredPosition = pos;
        _totalCount = totalCount;
        _text.text = $"{curCount} / {totalCount}";

        for (int i = 0; i < 5; ++i)
        {
            _image[i] = transform.GetChild(i).GetComponent<Image>();
            randnum[i] = Random.Range(0, 3);
            _image[i].sprite = atlas.GetSprite((atlas.spriteCount - randnum[i]).ToString());
            _afterSprite[i] = atlas.GetSprite((Random.Range(0, 20) * 3 + randnum[i] + 1).ToString());
        }
        gameObject.SetActive(true);
        _rect.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutQuart);
    }

    public void SetEffect(int num)
    {
        _text.text = $"{num} / {_totalCount}";
        num = (num - 1) % 5;
        _image[num].sprite = _afterSprite[num];

        if (num == 4)
        {
            _rect.DOAnchorPos(new Vector2(-1920, 0), 0.5f);
        }
    }
}
