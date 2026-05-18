using System;
using UnityEngine;

public class TimingStyleGame : BaseGame
{
    [SerializeField] private float destinationTime;
    protected override void StartGame()
    {
        base.StartGame();
    }

    protected override void FinishGame(bool bSuccess = true)
    {
        base.FinishGame(bSuccess);
    }

    protected override int GetResult()
    {
        return base.GetResult();
    }

    protected override bool UpdateGame()
    {
        return base.UpdateGame();
    }

    protected virtual void StopAndCheckTiming()
    {
    }
}