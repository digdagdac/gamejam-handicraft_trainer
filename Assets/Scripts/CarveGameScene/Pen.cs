using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pen : MonoBehaviour
{
    private Vector2[] _basePoses = new Vector2[5];
    private Animator _anim;
    private RectTransform _rect;

    public void Init()
    {
        for (int i = 0; i < 5; ++i)
        {
            _basePoses[i] = new Vector2(-350 + 225 * i, 200);
        }
        _anim = GetComponent<Animator>();
        _rect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void GetNumber(int num)
    {
        gameObject.SetActive(true);
        _rect.anchoredPosition = _basePoses[num];
        _anim.Play("Pen");
    }
}
