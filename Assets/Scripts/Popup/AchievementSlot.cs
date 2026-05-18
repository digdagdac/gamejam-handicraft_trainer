using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image check;
    private int id;
    public void Setup(int id, string desc)
    {
        this.id = id;
        text.text = desc;
        
        check.enabled = false;
    }
}