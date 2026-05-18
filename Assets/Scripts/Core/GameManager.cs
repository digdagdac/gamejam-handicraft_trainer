using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    public enum GameMode
    {
        None,
        Game1,
        Game2,
        Game3,
    }
    public static GameManager Instance { get; } = new GameManager();

    public GameMode CurrentMode { get; private set; }
    
    public Action OnStartGame { get; private set; }
    public Action<float> OnUpdateTime { get; private set; }
    public Action<bool, GameMode, GameMode> OnFinishGame { get; private set; }

    ~GameManager()
    {
        OnStartGame = null;
        OnUpdateTime = null;
    }
    
    public void ResetAll()
    {
        Time.timeScale = 1;
        AccountManager.Instance.ResetPoint();
        SceneManager.LoadScene("0.Intro");
        CurrentMode = GameMode.None;
    }

    public void LoadLobby()
    {
        Time.timeScale = 1;
        
        AccountManager.Instance.ResetPoint();
        
        Fade.StartFade(1.5f, () =>
        {
            SceneManager.LoadScene("1.Lobby");
            CurrentMode = GameMode.None;
        });
    }
    
    public void LoadEnding()
    {
        Time.timeScale = 1;
        
        Fade.StartFade(2.5f, () =>
        {
            SceneManager.LoadScene("99.Ending");
            CurrentMode = GameMode.None;
        });
    }

    public void StartGame(GameMode gameMode)
    {
        Fade.StartFade(1f, () =>
        {
            SceneManager.LoadScene($"2.{gameMode}", LoadSceneMode.Additive);
            CurrentMode = gameMode;
        });
    }

    public void StartGameImmediately(GameMode gameMode)
    {
        if (gameMode != GameMode.None)
        {
            SceneManager.LoadScene($"2.{gameMode}", LoadSceneMode.Additive);
        }
        else
        {
            CurrentMode = GameMode.None;
        }
    }
    
    public void FinishGame(GameMode gameMode, GameMode nextMode)
    {
        Fade.StartFinishFade(gameMode, nextMode);
    }
    
    public void SetStartGameHandler(Action onStartGame)
    {
        this.OnStartGame += onStartGame;
    }

    public void SetUpdateTimeHandler(Action<float> onUpdateTime)
    {
        this.OnUpdateTime += onUpdateTime;
    }

    public void SetFinishGameHandler(Action<bool, GameMode, GameMode> onFinishGame)
    {
        this.OnFinishGame += onFinishGame;
    }
}

public static class GameModeExtension
{
    public static bool IsFinalMode(this GameManager.GameMode gameMode)
    {
        return gameMode == GameManager.GameMode.Game3;
    }
}