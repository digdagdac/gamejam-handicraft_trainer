using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndSprite : MonoBehaviour
{
    public Image RealEnd3rd;

    public Image End3rdImage;
    public Image End3rdImage2;
    public Image End3rdImage3;

    [SerializeField]
    private Sprite sprite;

    int EndValue =0;
    // Update is called once per frame
    private void Start()
    {
        
        if (EndValue == 1)
        {
            sprite =End3rdImage.sprite;
            RealEnd3rd.GetComponent<Image>().sprite = sprite;
        }
        else if(EndValue == 2)
        {
            sprite=End3rdImage2.sprite;
            RealEnd3rd.GetComponent<Image>().sprite = sprite;
        }
        else
        {
            sprite=End3rdImage3.sprite;
            RealEnd3rd.GetComponent<Image>().sprite = sprite;
        }
        
    }


}
