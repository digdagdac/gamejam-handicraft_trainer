using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;

public class MatchingTimingStyleGame : TimingStyleGame
{
    [SerializeField] private Transform[] blocks;
    [SerializeField] private Transform line;
    [SerializeField] private SpriteAtlas atlas;

    [SerializeField] private float lineMin;
    [SerializeField] private float lineMax;
    [SerializeField] private float speedMin;
    [SerializeField] private float speedMax;
    
    [SerializeField] private float cutline;
    
    [SerializeField] private TimeBar timeBar;
    
    [SerializeField] private GameObject[] lifeObjects;
    [SerializeField] private int life;
    [FormerlySerializedAs("lifeCount")] [SerializeField] private int decreaseLifeCount;

    private float baseLineY;
    private float[] blockSpeeds;
    private int blockIndex;
    private int[] spriteIndex = new int[7];
    private float[] blockY = new float[7];

    protected override void StartGame()
    {
        base.StartGame();

        baseLineY = UnityEngine.Random.Range(lineMin, lineMax);
        line.localPosition = new Vector3(0, baseLineY, 0);
        blockSpeeds = new float[blocks.Length];
        for (int i = 0; i < blocks.Length; i++)
        {
            var pos = blocks[i].localPosition;
            pos.y = UnityEngine.Random.Range(lineMin, lineMax);
            blocks[i].localPosition = pos;
            blockSpeeds[i] = UnityEngine.Random.Range(speedMin, speedMax);

            var sprite = blocks[i].GetComponent<SpriteRenderer>();
            spriteIndex[i] = UnityEngine.Random.Range(1, 61);
            sprite.sprite = atlas.GetSprite($"{spriteIndex[i]:D2}");
        }
    }

    protected override bool UpdateGame()
    {
        timeBar.SetFillAmount(1 - gameTime / limitTime);
        
        if (IsPlaying)
        {
            for (int i = blockIndex; i < blocks.Length; i++)
            {
                var pos = blocks[i].localPosition;
                pos.y += blockSpeeds[i] * Time.deltaTime;
                if (pos.y > lineMax)
                {
                    pos.y = lineMax;
                    blockSpeeds[i] *= -1;
                }
                else if (pos.y < lineMin)
                {
                    pos.y = lineMin;
                    blockSpeeds[i] *= -1;
                }

                blocks[i].localPosition = pos;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SoundManager.Instance.Play2DSound(SFX.Hammer);
                StopAndCheckTiming();
            }
        }
        
        return base.UpdateGame();
    }
    
    protected override void StopAndCheckTiming()
    {
        var isSuccess = IsSuccess(blockIndex);
        
        if (!isSuccess)
        {
            blocks[blockIndex].GetComponent<SpriteRenderer>().color = Color.gray;

            if (decreaseLifeCount < lifeObjects.Length)
            {
                lifeObjects[decreaseLifeCount].SetActive(false);
                decreaseLifeCount++;
            }
        }
        
        blockIndex++;
        
        if (blockIndex >= blocks.Length || !IsAllSuccess())
        {
            FinishGame(IsAllSuccess());
        }
    }

    protected override int GetResult()
    {
        float rate = 0;
        for (int i = 0; i < blocks.Length; ++i)
            rate += Mathf.Abs(blocks[i].localPosition.y - baseLineY);
        return (int)((limitTime - gameTime + _curGameData.Score) * (1.0f - rate/7));
    }

    private bool IsSuccess(int index)
    {
        blockY[index] = blocks[index].localPosition.y;
        return Mathf.Abs(blocks[index].localPosition.y - baseLineY) <= cutline;
    }

    private bool IsAllSuccess()
    {
        int failCount = 0;
        for (int i = 0; i < blockIndex; i++)
        {
            if (!IsSuccess(i))
            {
                failCount++;
            }
        }

        return failCount < life;
    }

    protected override void FinishGame(bool bSuccess = true)
    {
        AccountManager.Instance.SetCollection(spriteIndex, blockY);
        base.FinishGame(bSuccess);
    }
}