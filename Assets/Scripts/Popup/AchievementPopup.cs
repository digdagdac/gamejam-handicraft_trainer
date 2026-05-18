using UnityEngine;
using UnityEngine.U2D;

public class AchievementPopup : BasePopup
{
    [SerializeField] private AchievementSlot[] achievementSlots;
    public override void Show()
    {
        base.Show();

        int index = 0;
        foreach (var achievement in DataManager.Instance.GetAchievements())
        {
            if (index < achievementSlots.Length)
            {
                achievementSlots[index].Setup(achievement.Id, achievement.Txt);
                index++;
            }
        }
    }
}