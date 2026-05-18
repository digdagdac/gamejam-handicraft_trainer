using System;
using UnityEngine;
using UnityEngine.UI;

public class BookSlot : MonoBehaviour
{
    [SerializeField] private Image image;
    private int id;
    private Action onClickBook;
    public void Setup(int id, Sprite sprite)
    {
        this.id = id;
        image.sprite = sprite;
    }

    public void SetClickAction(Action onClickBook)
    {
        this.onClickBook = onClickBook;
    }

    public void OnClickBook()
    {
        onClickBook?.Invoke();
    }
}