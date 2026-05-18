using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ComboText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _keepTime = 1.5f;
    private CancellationTokenSource _cts;

    public void Init()
    {
        _text.text = "";
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        Fade().Forget();
    }

    private async UniTaskVoid Fade()
    {
        try 
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                await UniTask.Yield(_cts.Token);
            }
        }
        catch (System.OperationCanceledException)
        {
            // 정상적인 취소는 무시
        }
    }

    public void GetInput(int combo)
    {
        if (combo > 0)
        {
            _text.text = $"{combo} COMBO!";
        }
        else
        {
            _text.text = "";
        }
    }

    public void Clear()
    {
        try
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
        catch (System.ObjectDisposedException)
        {
            // 이미 dispose된 경우 무시
        }
        _text.text = "";
    }

    private void OnDisable()
    {
        Clear();
    }
}
