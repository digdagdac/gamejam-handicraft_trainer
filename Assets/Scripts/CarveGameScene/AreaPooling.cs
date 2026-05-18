using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AreaPooling : MonoBehaviour
{
    private Queue<GameObject> _areaPool = new Queue<GameObject>();
    private List<GameObject> _activeAreas = new List<GameObject>();

    private readonly Vector2 _curAreaPos = new Vector2(0, -350);
    private readonly Vector2 _nextAreaPos = new Vector2(1360, -350);

    public void Init(string str)
    {
        Clear();
        for (int i = 0; i < transform.childCount; ++i)
        {
            _areaPool.Enqueue(transform.GetChild(i).gameObject);
        }
        
        ActiveArea(str);
        _activeAreas[0].GetComponent<TypingArea>().MoveToTargetPos(_curAreaPos);
    }

    private void ActiveArea(string str)
    {
        GameObject temp = _areaPool.Dequeue();
        TypingArea typingArea = temp.GetComponent<TypingArea>();
        typingArea.Init(_nextAreaPos, str);
        temp.SetActive(true);
        _activeAreas.Add(temp);
        typingArea.MoveToTargetPos(_curAreaPos);
    }

    public void ChangeArea(string str)
    {
        GameObject temp = _activeAreas[0];
        temp.SetActive(false);
        _areaPool.Enqueue(temp);
        _activeAreas.RemoveAt(0);
        ActiveArea(str);
    }

    public void ActiveImageEffect(int point)
    {
        _activeAreas[0].GetComponent<TypingArea>().SetImage(point);
    }
    public string GetCurrentCharacters()
    {
        if (_activeAreas.Count > 0)
        {
            return _activeAreas[0].GetComponent<TypingArea>().GetCharacters();
        }
        return string.Empty;
    }

    public void Clear()
    {
        foreach (var area in _activeAreas)
        {
            if (area != null)
            {
                area.SetActive(false);
                _areaPool.Enqueue(area);
            }
        }
        _activeAreas.Clear();
    }
}
