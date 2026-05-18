using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TypingArea : MonoBehaviour
{
    private RectTransform _rect;
    [SerializeField]
    private Image[] _image;
    [SerializeField]
    private TextMeshProUGUI[] _text;

    [SerializeField]
    private Sprite _sprite;

    private Dictionary<KeyCode, char> _keyCodeNCharPair = new Dictionary<KeyCode, char>()
    {
        {KeyCode.Q, 'q'}, {KeyCode.W, 'w'}, {KeyCode.E, 'e'}, {KeyCode.R, 'r'},
        {KeyCode.T, 't'}, {KeyCode.Y, 'y'}, {KeyCode.U, 'u'}, {KeyCode.I, 'i'},
        {KeyCode.O, 'o'}, {KeyCode.P, 'p'}, {KeyCode.A, 'a'}, {KeyCode.S, 's'},
        {KeyCode.D, 'd'}, {KeyCode.F, 'f'}, {KeyCode.G, 'g'}, {KeyCode.H, 'h'},
        {KeyCode.J, 'j'}, {KeyCode.K, 'k'}, {KeyCode.L, 'l'}, {KeyCode.Z, 'z'},
        {KeyCode.X, 'x'}, {KeyCode.C, 'c'}, {KeyCode.V, 'v'}, {KeyCode.B, 'b'},
        {KeyCode.N, 'n'}, {KeyCode.M, 'm'},
        {KeyCode.Alpha1, '1'}, {KeyCode.Alpha2, '2'}, {KeyCode.Alpha3, '3'},
        {KeyCode.Alpha4, '4'}, {KeyCode.Alpha5, '5'}, {KeyCode.Alpha6, '6'},
        {KeyCode.Alpha7, '7'}, {KeyCode.Alpha8, '8'}, {KeyCode.Alpha9, '9'},
        {KeyCode.Alpha0, '0'}
    };

    public void Init(Vector2 pos, string str)
    {
        _rect = GetComponent<RectTransform>();
        
        _rect.anchoredPosition = pos;
        for (int i = 0; i < 5; ++i)
        {
            _text[i].text = str[i].ToString();
            _image[i].gameObject.SetActive(true);
            _image[i].sprite = _sprite;
        }
    }

    public void MoveToTargetPos(Vector2 targetPos, float time = 0.5f)
    {
        _rect.DOAnchorPos(targetPos, time).SetEase(Ease.InCubic);
    }

    public void SetImage(int num)
    {
        _text[num].text = string.Empty;
        _image[num].gameObject.SetActive(false);
    }

    public string GetCharacters()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 5; ++i)
        {
            if (_image[i].gameObject.activeSelf)
            {
                sb.Append(_text[i].text);
            }
        }
        return sb.ToString();
       
    }
}
