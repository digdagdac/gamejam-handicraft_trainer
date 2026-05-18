using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    public static Fade Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = Instantiate(Resources.Load<GameObject>("Fade"));
                instance = obj.GetComponent<Fade>();
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }

    private static Fade instance;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public static void StartFade(float time, Action onFadeAction)
    {
        Instance.StartCoroutine(StartFadeRoutine(time, onFadeAction));
    }

    public static void StartFinishFade(GameManager.GameMode gameMode, GameManager.GameMode nextMode)
    {
        Instance.StartCoroutine(StartFinishFadeRoutine(gameMode, nextMode));
    }
    
    private static IEnumerator StartFinishFadeRoutine(GameManager.GameMode gameMode, GameManager.GameMode nextMode)
    {
        yield return SceneManager.UnloadSceneAsync($"2.{gameMode}");

        yield return StartFadeRoutine(1f, () =>
        {
            GameManager.Instance.StartGameImmediately(nextMode);
        });
    }

    private static IEnumerator StartFadeRoutine(float time, Action onFadeAction)
    {
        Instance.canvasGroup.interactable = false;
        Instance.canvasGroup.blocksRaycasts = false;
        
        Instance.canvasGroup.alpha = 0f;
        yield return Instance.canvasGroup.DOFade(1f, time);
        Instance.canvasGroup.alpha = 1f;
        
        onFadeAction?.Invoke();
        
        yield return Instance.canvasGroup.DOFade(0f, time);
        Instance.canvasGroup.alpha = 0f;
        
        Instance.canvasGroup.interactable = false;
        Instance.canvasGroup.blocksRaycasts = false;
    }
}