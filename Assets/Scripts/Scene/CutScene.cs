using UnityEngine;

public class CutScene : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    private int currentIndex;
    private void Awake()
    {
        GoToNextPage();
    }

    private void GoToNextPage()
    {
        if (currentIndex >= pages.Length)
        {
            GoToLobby();
            return;
        }
        
        foreach (var page in pages)
        {
            page.SetActive(false);
        }
        
        pages[currentIndex].SetActive(true);
        currentIndex++;
    }

    public void OnClickGoToLobby()
    {
        GoToNextPage();
    }

    protected virtual void GoToLobby()
    {
        GameManager.Instance.LoadLobby();
    }
}