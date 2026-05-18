using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Data
{
    public int ID;
    public int Type;
    public int Score;
    public int TimeLimit;
    public int LetterTypeCount;
    public int LetterLength;
    public string Letter;
}

[Serializable]
public class DataLoad
{
    public List<Data> data;

    public void Load(out Dictionary<int, Data> dic)
    {
        dic = new Dictionary<int, Data>();
        for (int i = 0; i < data.Count; ++i)
            dic.Add(data[i].ID, data[i]);
    }
}


[Serializable]
public struct SoundVolume
{
    public float BGM;
    public float Effect;

    public SoundVolume(float bgm = 0.5f, float effect = 0.5f)
    {
        BGM = bgm;
        Effect = effect;
    }
}

[Serializable]
public struct CompleteBook
{
    public int Id;
    public int Level;
    public string Txt;
}

[Serializable]
public class BookLoad
{
    public List<CompleteBook> data3;

    public void Load(out Dictionary<int, CompleteBook> dic)
    {
        dic = new Dictionary<int, CompleteBook>();
        for (int i = 0; i < data3.Count; ++i)
            dic.Add(data3[i].Id, data3[i]);
    }
}

[Serializable]
public struct Achievement
{
    public int Id;
    public string Txt;
}

[Serializable]
public class AchievementLoad
{
    public List<Achievement> data2;

    public void Load(out Dictionary<int, Achievement> dic)
    {
        dic = new Dictionary<int, Achievement>();
        for (int i = 0; i < data2.Count; ++i)
            dic.Add(data2[i].Id, data2[i]);
    }
}

public class Collection 
{
    public CompleteBook Book;
    public int[] Numbers = new int[7];
    public float[] YValues = new float[7];

    public Collection()
    {

    }

    public Collection(CompleteBook book, int[] nums, float[] y) 
    {
        Book = book;
        Numbers = nums;
        YValues = y;
    }
}