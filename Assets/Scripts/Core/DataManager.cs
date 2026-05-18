using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataManager
{
    private Dictionary<int, Data> _dataTable;
    private Dictionary<int, Achievement> _achievementTable;
    private Dictionary<int, CompleteBook> _completeBookTable;

    public static DataManager Instance
    {
        get {
            if (d_instance == null)
                d_instance = new DataManager();

            return d_instance;
        }
    }

    private static DataManager d_instance;

    private DataManager()
    {
        TextAsset data = Resources.Load<TextAsset>("minigame");
        JsonUtility.FromJson<DataLoad>(data.text).Load(out _dataTable);
        data = Resources.Load<TextAsset>("Todolist (2)");
        JsonUtility.FromJson<AchievementLoad>(data.text).Load(out _achievementTable);
        data = Resources.Load<TextAsset>("CompleteBook");
        JsonUtility.FromJson<BookLoad>(data.text).Load(out _completeBookTable);
    }

    public Data GetData(int id)
    {
        _dataTable.TryGetValue(id, out var data);
        return data;
    }

    public Achievement GetAchievement(int id)
    {
        _achievementTable.TryGetValue(id, out var achievement);
        return achievement;
    }

    public IEnumerable<Achievement> GetAchievements()
    {
        return _achievementTable.Values;
    }

    public CompleteBook GetCompleteBook(int id)
    {
        _completeBookTable.TryGetValue(id, out var completeBook);
        return completeBook;
    }
    
    public IEnumerable<CompleteBook> GetCompleteBooks()
    {
        return _completeBookTable.Values;
    }
}
