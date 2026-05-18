using System.Collections.Generic;
using System.Diagnostics;

public class AccountManager
{
    public static AccountManager Instance { get; } = new AccountManager();

    public int gamePoint { get; private set; }
    public List<int> collections { get; private set; } = new List<int>();
    public List<int> achievements { get; private set; } = new List<int>();

    public List<Collection> tempColl { get; private set; } = new List<Collection>();
    private Collection curCollection;

    public void SetCollection(int[] nums, float[] y)
    {
        curCollection = new Collection();
        curCollection.Numbers = nums;
        curCollection.YValues = y;
    }

    public bool hasEnding { get; private set; }

    public void ResetPoint()
    {
        this.gamePoint = 0;
    }
    public void AddGamePoint(int gamePoint)
    {
        this.gamePoint += gamePoint;
    }

    public void AddCollection(int id)
    {
        collections.Add(id);
        curCollection.Book = DataManager.Instance.GetCompleteBook(id);
        tempColl.Add(curCollection);
        curCollection = null;
    }
    
    public void AddAchievement(int id)
    {
        achievements.Add(id);
    }

    public void Load()
    {
        
    }

    public void Save()
    {
        
    }

    public void SaveEnding()
    {
        hasEnding = true;
    }
}