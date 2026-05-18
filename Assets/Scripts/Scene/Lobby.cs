using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    
    [SerializeField] private SettingPopup settingPopup;
    [SerializeField] private SuccessPopup successPopup;
    [SerializeField] private FailPopup failPopup;
    [SerializeField] private AchievementPopup achievementPopup;
    [SerializeField] private CollectionPopup collectionPopup;
    
    [SerializeField] private GameObject[] achievementObjects;
    [SerializeField] private GameObject[] collectionObjects;
    [SerializeField] private TextMeshProUGUI collectionCount;

    [SerializeField] private Image craftTitle;
    [SerializeField] private Sprite craftSprite;
    [SerializeField] private Sprite endingSprite;

    private bool normalCraftEnter;

    private void Awake()
    {
        settingPopup.gameObject.SetActive(false);
        successPopup.gameObject.SetActive(false);
        failPopup.gameObject.SetActive(false);
        achievementPopup.gameObject.SetActive(false);
        collectionPopup.gameObject.SetActive(false);
        
        GameManager.Instance.SetStartGameHandler(OnStartGame);
        GameManager.Instance.SetUpdateTimeHandler(OnUpdateTime);
        GameManager.Instance.SetFinishGameHandler(OnFinishGame);
        
        RefreshRewardObjects();
    }

    private void OnStartGame()
    {
        
    }

    private void OnUpdateTime(float rate)
    {
        
    }
    
    private void OnFinishGame(bool bSuccess, GameManager.GameMode current, GameManager.GameMode next)
    {
        if (bSuccess)
        {
            if (current.IsFinalMode())
            {
                successPopup.Show(() =>
                {
                    GameManager.Instance.FinishGame(current, GameManager.GameMode.None);
                    RefreshRewardObjects();
                });
            }
            else
            {
                GameManager.Instance.FinishGame(current, next);
            }
        }
        else
        {
            failPopup.Show(() =>
            {
                GameManager.Instance.FinishGame(current, GameManager.GameMode.None);
            });
        }
    }

    private void RefreshRewardObjects()
    {
        var completeBookCount = 6;//DataManager.Instance.GetCompleteBooks().Count();

        // var achievements = AccountManager.Instance.achievements;
        // for (int i = 0; i < this.achievementObjects.Length; i++)
        // {
        //     this.achievementObjects[i].SetActive((i == 1 || i == 3 || i == 5));
        // }

        var collections = AccountManager.Instance.collections;
        for (int i = 0; i < this.collectionObjects.Length; i++)
        {
            this.collectionObjects[i].SetActive(i < collections.Count);
        }

        collectionCount.text = $"{collections.Count}/{completeBookCount}";

        this.achievementObjects[0].SetActive(collections.Count >= 1);
        this.achievementObjects[1].SetActive(collections.Count >= 3);
        this.achievementObjects[2].SetActive(collections.Count >= 6);

        normalCraftEnter = collections.Count != completeBookCount || AccountManager.Instance.hasEnding;
        craftTitle.sprite = normalCraftEnter ? craftSprite : endingSprite;
    }

    private void Update()
    {
        var isLoadedGame = SceneManager.sceneCount > 1;
        canvasGroup.interactable = !isLoadedGame;
        canvasGroup.blocksRaycasts = !isLoadedGame;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var list = new List<BasePopup>(){successPopup, failPopup, achievementPopup, collectionPopup};

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsActive())
                {
                    list[i].OnClickClose();
                    return;
                }
            }
            
            if (!settingPopup.IsActive())
            {
                settingPopup.Show();
            }
            else
            {
                settingPopup.OnClickClose();
            }
        }
    }

    public void OnClickStart()
    {
        if (normalCraftEnter)
        {
            GameManager.Instance.StartGame(GameManager.GameMode.Game1);
        }
        else
        {
            GameManager.Instance.LoadEnding();
        }
    }

    public void OnClickCollection()
    {
        collectionPopup.Show();
    }

    public void OnClickAchievement()
    {
        achievementPopup.Show();
    }

    public void OnClickSetting()
    {
        settingPopup.Show();
    }
}