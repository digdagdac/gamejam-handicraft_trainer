using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class BaseGame : MonoBehaviour
{
    [SerializeField] private GameManager.GameMode gameMode;
    [SerializeField] private GameManager.GameMode nextMode;
    [SerializeField] protected int limitTime;
    protected float gameTime { get; private set; }
    public bool IsPlaying { get; private set; }
    protected Data _curGameData;

    private int score;

    protected void SetTime(float gameTime)
    {
        this.gameTime = gameTime;
    }

    protected virtual void StartGame()
    {
        gameTime = 0;
        IsPlaying = true;
        GameManager.Instance.OnStartGame?.Invoke();
        
        if (gameMode != GameManager.GameMode.None)
        {
            int datanumber = AccountManager.Instance.collections.Count > 5 ?
        UnityEngine.Random.Range(0, 6) : AccountManager.Instance.collections.Count;
            _curGameData = DataManager.Instance.GetData(datanumber * 3 + (int)(gameMode) - 1);
            limitTime = _curGameData.TimeLimit;
        }
    }

    protected virtual void FinishGame(bool bSuccess = true)
    {
        IsPlaying = false;
        
        if (bSuccess)
        {
            AccountManager.Instance.AddGamePoint(GetResult());
            SoundManager.Instance.Play2DSound(SFX.Success);
        }

        else
            SoundManager.Instance.Play2DSound(SFX.Fail);

        GameManager.Instance.OnFinishGame?.Invoke(bSuccess, gameMode, nextMode);
    }

    protected virtual int GetResult()
    {
        return 0;
    }

    protected virtual bool UpdateGame()
    {
        gameTime += Time.deltaTime;
        GameManager.Instance.OnUpdateTime?.Invoke(1 - gameTime / limitTime);
        return true;
    }

    public void AddScore(int points) {
        score += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI() {
        // UI 텍스트 업데이트 로직 추가
    }

    void Awake()
    {
        StartGame();
    }

    private void Update()
    {
        if (IsPlaying)
        {
            UpdateGame();
        }
    }
    
    /// <summary>
    /// For Test
    /// </summary>
    public void OnClickFinish()
    {
        GameManager.Instance.FinishGame(gameMode, gameMode);
    }
}