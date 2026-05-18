using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPooling : MonoBehaviour
{
    [SerializeField]
    private Sprite[] _feverSprites;
    [SerializeField]
    private Sprite _wrongSprite;

    private readonly Vector2 _baseSize = new Vector2(500, 125);

    private Queue<TextImage> _images = new Queue<TextImage>(20);

    public void Init()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            TextImage img = transform.GetChild(i).GetComponent<TextImage>();
            _images.Enqueue(img);
            img.Init();
            
        }
    }

    public void FeverEffect()
    {
        int rand = Random.Range(-20, 20);
        
        TextImage img = _images.Dequeue();
        img.FadeFeverImage(_feverSprites[Random.Range(0, 2)], 
            new Vector2(Random.Range(360, 720) * Mathf.Pow(- 1, (rand % 2)), Random.Range(240, 480) * Mathf.Pow(-1, (rand % 2))), 
            Vector3.forward * Random.Range(-30, 31),
            _baseSize + new Vector2(rand * 4, rand));
        _images.Enqueue(img);
    }

    //public void FailEffect()
    //{
    //    int rand = Random.Range(10, 20);
    //
    //    TextImage img = _images.Dequeue();
    //    img.FadeFailImage(_wrongSprite, Vector2.zero,
    //        _baseSize + new Vector2(rand * 4, rand));
    //    _images.Enqueue(img);
    //}
}
