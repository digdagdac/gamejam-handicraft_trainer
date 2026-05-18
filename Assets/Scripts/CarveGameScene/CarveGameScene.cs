using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CarveGameScene : MonoBehaviour 
{
    private readonly StringBuilder _startconsonant = new StringBuilder("ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ");
    private readonly StringBuilder _vowel = new StringBuilder("ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ");
    private readonly StringBuilder _endconsonant = new StringBuilder("ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ");
    private readonly StringBuilder _excludedLetter = new StringBuilder("ㅃㅉㄸㄲㅆㅒㅖㅘㅙㅚㅝㅞㅟㅢㄳㄵㄶㄺㄻㄼㄽㄾㄿㅀㅄ");

    private readonly ushort _koreanStart = 0xAC00;
    private readonly ushort _koreanEnd = 0xD79F;
    private readonly ushort _numberStart = 48;
    private readonly ushort _numberEnd = 57;

    private Dictionary<KeyCode, char> _keyCodeNCharPair = new Dictionary<KeyCode, char>();

    System.Random _rand = new System.Random();

    private Dictionary<char, int> _appearLetterSearchChar = new Dictionary<char, int>();

    private StringBuilder _nonDuplicateString;
    //List<KeyCode> _nonDuplicateEnum = new List<KeyCode>();
    private int _stringPointer = 0;

    public AreaPooling _area; // private -> public으로 변경
    [SerializeField]
    private BlockPooling _block;
    [SerializeField]
    private ComboText _comboText;
    [SerializeField]
    private Image _failImg;
    [SerializeField]
    private Image _fever;
    [SerializeField]
    private RectTransform _feverRect;
    [SerializeField]
    private RectTransform _feverRectText;
    private Sequence _feverSeq;
    [SerializeField]
    private RectTransform _feverLetter;
    [SerializeField]
    private TextPooling _textPool;
    [SerializeField]
    private Pen _pen;

    public float ComboTime = 1;
    private float _curComboTime;

    [SerializeField] private TimeBar timeBar;
    [SerializeField] 
    private AudioListener audioListener; // 오디오 리스너 추가
    [SerializeField]
    private TextMeshProUGUI  scoreText; // 점수 표시를 위한 UI 텍스트

    private int score; // 현재 점수

    private int _combo;
    private int _wrongCount;
    private int _feverCount;

    private CancellationTokenSource _cts = new CancellationTokenSource();
    private IDisposable _checkInput;
    private IDisposable _checkFail;

    protected float gameTime { get; private set; }
    protected int limitTime = 60; // 기본 제한시간 설정
    protected bool _isTimeOver;
    protected bool IsPlaying { get; private set; }
    protected Data _curGameData;

    private void Awake()
    {
        score = 0; // 초기 점수 설정
        UpdateScoreUI(); // UI 업데이트
        StartGame();
    }

    private void Update()
    {
        if (IsPlaying)
        {
            UpdateGame();
        }
        
        // if (gameTime >= limitTime)
        // {
        //     // 제한 시간이 다 되면 씬을 다시 로드
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // }
    }

    private void StartGame()
    {
        gameTime = 0;
        IsPlaying = true;
        Init();
    }

    protected void SetTime(float time)
    {
        gameTime = time;
    }

    public bool UpdateGame() // private -> public으로 변경
    {
        if (!IsPlaying) return false;
        
        gameTime += Time.deltaTime;
        timeBar.SetFillAmount(1 - gameTime / limitTime);
        return gameTime < limitTime;
    }

    public void FinishGame(bool bSuccess = true)
    {
        IsPlaying = false;
        _isTimeOver = !bSuccess;
        Clear();
    }

    public void InitializeGameState()
    {
        gameTime = 0f;
        score = 0;
        _wrongCount = 0;
        _feverCount = 0;
        _combo = 0;
        _curComboTime = ComboTime;
        _stringPointer = 0;
        IsPlaying = true;
        _isTimeOver = false;

        // UI 초기화
        timeBar.SetFillAmount(1f);
        UpdateScoreUI();
        if (_fever != null) 
        {
            _fever.gameObject.SetActive(false);
        }
    }

    public float GetRemainingTime()
    {
        return limitTime - gameTime;
    }

    public float GetTotalTime()
    {
        return limitTime;
    }

    private void OnTimeOver()
    {
        _isTimeOver = true;
        FinishGame(false); // 게임 종료 후 재시작
    }

    public void RestartGame()
    {
        Clear(); // 기존 상태 정리
        Init();  // 새 게임 초기화
        Debug.Log("게임 재시작...");
    
    // 게임 상태 초기화
    gameTime = 0f;
    score = 0;
    _wrongCount = 0;
    _feverCount = 0;
    _combo = 0;
    _curComboTime = ComboTime;
    _stringPointer = 0;
    IsPlaying = true;
    _isTimeOver = false;

    // UI 초기화
    timeBar.SetFillAmount(1f);
    UpdateScoreUI();
    if (_fever != null) 
    {
        _fever.gameObject.SetActive(false);
    }
    
    // 게임 영역 초기화
    StringBuilder str = new StringBuilder(5);
    for (int i = 0; i < 5; ++i)
    {
        str.Append(_nonDuplicateString[i]);
    }
    _area.Init(str.ToString());
    
    // 게임 시작
    StartGame();
    }   
    private string GetRandomKoreanString()
    {
        string[] sampleTexts = new string[] {
            "안녕하세요반갑습니다",
            "한글타자연습게임",
            "즐거운게임시간입니다",
            "열심히연습하세요",
            "화이팅파이팅해요",
            "신나는타자게임",
            "오늘도좋은하루",
            "타자실력늘어요",
            "한글을사랑해요",
            "게임을즐겨요"
        };
        return sampleTexts[UnityEngine.Random.Range(0, sampleTexts.Length)];
    }
    
    public void Init()
    {
        // DataManager 의존성 제거하고 랜덤 문자열 사용
        _curGameData = new Data {
            ID = 1,
            Type = 1,
            Score = 100,
            TimeLimit = 60,
            LetterTypeCount = 10,
            LetterLength = 20,
            Letter = GetRandomKoreanString()
        };

        Divide_Letter(new StringBuilder(_curGameData.Letter));
        CheckDuplicate(_curGameData.LetterLength, _curGameData.LetterTypeCount);
        _appearLetterSearchChar.Clear();
        MakeKeyCodeNCharTable();
        _textPool.Init();
        _pen.Init();

        // _feverSeq 초기화 전에 _fever가 null이 아닌지 확인
        if (_fever != null)
        {
            _feverSeq = DOTween.Sequence()
                    .AppendCallback(() => _fever.gameObject.SetActive(true))
                    .Append(_feverRect.DOScale(new Vector3(1.2f, 1.2f, 1), 0.25f))
                    .Join(_feverRectText.DOScale(new Vector3(1.2f, 1.2f, 1), 0.25f))
                    .Join(_fever.DOColor(Color.white, 0.25f))
                    .SetAutoKill(false)
                    .Pause();
        }

        StringBuilder str = new StringBuilder(5);

        for (int i = 0; i < 5; ++i)
            str.Append(_nonDuplicateString[i]);

        _area.Init(str.ToString());
        _block.Init(_nonDuplicateString.Length);
        _comboText.Init();
        _curComboTime = ComboTime;
        CheckComboTime().Forget();
        _checkInput = this.UpdateAsObservable().Subscribe(_ => CheckInput());
        _checkFail = this.UpdateAsObservable().Subscribe(_ => {
            if (gameTime >= limitTime)
            {
                FinishGame(false);
            }
        });
    }

    // private async UniTaskVoid TickTime()
    // {
    //     while (time > 0)
    //     {
    //         //�ɼ�â active����?
    //         await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
    //         time -= Time.deltaTime;
    //         _timeLimit.value = time;
    //     }
    // }

    private async UniTaskVoid ShakeSlider(float time = 0.3f, float amount = 0.1f)
    {
        Transform t = timeBar.GetComponent<Transform>();
        Vector2 startpos = t.position;
        while (time > 0.0f)
        {
            t.position = startpos + new Vector2(UnityEngine.Random.Range(-amount, amount),
                UnityEngine.Random.Range(-amount, amount));
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
            t.position = startpos;
            time -= Time.deltaTime;
        }
    }

    private async UniTaskVoid CheckComboTime()
    {
        while (true)
        {
            await UniTask.WaitUntil(() => _curComboTime > 0.5f, cancellationToken: _cts.Token);

            while (_curComboTime > 0.0f)
            {
                await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
                _curComboTime -= Time.deltaTime;
            }

            _combo = 0;
            _comboText.GetInput(_combo);
        }
    }

    private async UniTaskVoid ShakeFever(float time = 0.5f, float amount = 10f)
    {
        Vector2 startpos = _feverLetter.anchoredPosition;
        _feverLetter.gameObject.SetActive(true);
        while (time > 0.0f)
        {
            _feverLetter.anchoredPosition = startpos + new Vector2(UnityEngine.Random.Range(-amount, amount),
                UnityEngine.Random.Range(-amount, amount));
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
            _feverLetter.anchoredPosition = startpos;
            time -= Time.deltaTime;
        }
        _feverLetter.gameObject.SetActive(false);
    }

    private void FeverEffect(bool bSuccess)
    {
        // null 체크 추가
        if (_fever == null || _feverSeq == null) return;

        if (bSuccess)
        {
            _feverSeq.Restart();
            if (BgmPlayer.Bgm != null)
                BgmPlayer.Bgm.BgmTrigger(null, 1.2f);
        }
        else
        {
            _feverSeq.Pause();
            _feverRect.localScale = new Vector3(1, 1, 1);
            _feverRectText.localScale = new Vector3(1, 1, 1);
            _fever.gameObject.SetActive(false);
            if (BgmPlayer.Bgm != null)
                BgmPlayer.Bgm.BgmTrigger(null, 1);
        }
    }

    private void CheckInput()
    {
        foreach(KeyCode key in _keyCodeNCharPair.Keys)
        {
            if (Input.GetKeyDown(key) && Time.timeScale != 0)
            {
                SoundManager.Instance.Play2DSound(SFX.Keyboard);
                if (_nonDuplicateString[_stringPointer % _nonDuplicateString.Length] == _keyCodeNCharPair[key])
                {
                    _area.ActiveImageEffect(_stringPointer % 5);
                    _pen.GetNumber(_stringPointer % 5);
                    
                    // 블록 업데이트는 현재 입력 횟수 기준으로
                    if (_block != null)
                    {
                        _block.SetBlock(_stringPointer + 1);
                    }
                    
                    ++_stringPointer;
                    ++_combo;
                    _curComboTime = ComboTime;
                    SoundManager.Instance.Play2DSound(SFX.Shave);
                    
                    if (_combo == 5 && _fever != null) // null 체크 추가
                    {
                        FeverEffect(true);
                    }

                    if (_combo >= 5)
                    {
                        if (_combo % 5 == 0)
                            ShakeFever().Forget();
                        _textPool.FeverEffect();
                    }

                    _comboText.GetInput(_combo);

                    if (_stringPointer % 5 == 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder(5);
                        for (int i = 0; i < 5; ++i)
                        {
                            int index = (_stringPointer + i) % _nonDuplicateString.Length; // 무한 반복을 위해 모듈로 연산 사용
                            stringBuilder.Append(_nonDuplicateString[index]);
                        }
                        _area.ChangeArea(stringBuilder.ToString());
                    }

                    // 점수 증가
                    score += 10;
                    UpdateScoreUI();
                }
                else
                {
                    ShakeSlider().Forget();
                    SetTime(gameTime + 3);
                    _combo = 0;
                    _comboText.GetInput(_combo);
                    ++_wrongCount;
                    FeverEffect(false);
                    SoundManager.Instance.Play2DSound(SFX.Wrong);

                    // 점수 감소
                    score -= 5;
                    UpdateScoreUI();
                }
            }
        }
    }

    public void CheckInput(KeyCode key)
    {
        // 기존 CheckInput 메서드의 내용을 키 입력을 받아 처리하도록 수정
        if (_keyCodeNCharPair.ContainsKey(key))
        {
            if (_nonDuplicateString[_stringPointer % _nonDuplicateString.Length] == _keyCodeNCharPair[key])
            {
                _area.ActiveImageEffect(_stringPointer % 5);
                _pen.GetNumber(_stringPointer % 5);
                
                // 블록 업데이트는 현재 입력 횟수 기준으로
                if (_block != null)
                {
                    _block.SetBlock(_stringPointer + 1);
                }
                
                ++_stringPointer;
                ++_combo;
                _curComboTime = ComboTime;
                SoundManager.Instance.Play2DSound(SFX.Shave);
                
                if (_combo == 5 && _fever != null) // null 체크 추가
                {
                    FeverEffect(true);
                }

                if (_combo >= 5)
                {
                    if (_combo % 5 == 0)
                        ShakeFever().Forget();
                    _textPool.FeverEffect();
                }

                _comboText.GetInput(_combo);

                if (_stringPointer % 5 == 0)
                {
                    StringBuilder stringBuilder = new StringBuilder(5);
                    for (int i = 0; i < 5; ++i)
                    {
                        int index = (_stringPointer + i) % _nonDuplicateString.Length; // 무한 반복을 위해 모듈로 연산 사용
                        stringBuilder.Append(_nonDuplicateString[index]);
                    }
                    _area.ChangeArea(stringBuilder.ToString());
                }

                // 점수 증가
                score += 10;
                UpdateScoreUI();
            }
            else
            {
                ShakeSlider().Forget();
                SetTime(gameTime + 3);
                _combo = 0;
                _comboText.GetInput(_combo);
                ++_wrongCount;
                FeverEffect(false);
                SoundManager.Instance.Play2DSound(SFX.Wrong);

                // 점수 감소
                score -= 5;
                UpdateScoreUI();
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    private void CheckDuplicate(int letterLen, int typeCount)
    {
        _nonDuplicateString = new StringBuilder(letterLen);
        StringBuilder temp = new StringBuilder(_appearLetterSearchChar.Count);
        List<int> randType = new List<int>();
        int randomNum;
        char curLetter = '!';
        short countDuplication = 1;

        foreach(var Key in _appearLetterSearchChar.Keys)
            temp.Append(Key);

        if (typeCount < temp.Length)
        {
            while (randType.Count < typeCount)
            {
                randomNum = _rand.Next(0, temp.Length);
                if (randType.IndexOf(randomNum) == -1)
                    randType.Add(randomNum);
            }
        }

        else
        {
            for (int i = 0; i < temp.Length; ++i)
                randType.Add(i);
        }

        for (int i = 0; i < letterLen; ++i)
        {
            randomNum = _rand.Next(0, randType.Count);
    
            if (curLetter == temp[randomNum])
            {
                if (countDuplication > 1)
                {
                    --i;
                    continue;
                }

                ++countDuplication;
            }

            else
            {
                countDuplication = 1;
                curLetter = temp[randomNum];
            }

            if (randomNum >= 0 && randomNum < temp.Length)
            {
                _nonDuplicateString.Append(curLetter);
            }
        }
    }

    private void Divide_Letter(string str)
    {
        StringBuilder stringBuilder = new StringBuilder(str);
        Divide_Letter(stringBuilder);
    }

    private void Divide_Letter(StringBuilder str)
    {
        int i;
        int letterStart, letterMid, letterEnd;
        ushort tempUnicode;
        //StringBuilder dividedLetter = new StringBuilder(str.Length);
        
        for (i = 0; i < str.Length; ++i)
        {
            tempUnicode = Convert.ToUInt16(str[i]);
            if (tempUnicode < _koreanStart || tempUnicode > _koreanEnd)
            {
                if ((tempUnicode >= _numberStart && tempUnicode <= _numberEnd))
                    _appearLetterSearchChar.TryAdd(str[i], _appearLetterSearchChar.Count);
                
                continue;
            }
            tempUnicode -= _koreanStart;
            letterStart = tempUnicode / (21 * 28);
            //dividedLetter.Append(_startconsonant[letterStart]);
            _appearLetterSearchChar.TryAdd(_startconsonant[letterStart], _appearLetterSearchChar.Count);

            tempUnicode %= (21 * 28);
            letterMid = tempUnicode / 28;
            //dividedLetter.Append(_vowel[letterMid]);
            _appearLetterSearchChar.TryAdd(_vowel[letterMid], _appearLetterSearchChar.Count);

            tempUnicode %= 28;
            letterEnd = tempUnicode;

            if (letterEnd != 0)
            {
                //dividedLetter.Append(_endconsonant[letterEnd - 1]);
                _appearLetterSearchChar.TryAdd(_endconsonant[letterEnd - 1], _appearLetterSearchChar.Count);
            }
        }

        for (i = 0; i < _excludedLetter.Length; ++i)
        {
            _appearLetterSearchChar.Remove(_excludedLetter[i]);
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
            }
        }
        catch (System.ObjectDisposedException)
        {
            // 이미 dispose된 경우 무시
        }
        _cts = new CancellationTokenSource();
        
        if (_area != null) _area.Clear();
        if (_comboText != null) _comboText.Clear();
        
        _checkInput?.Dispose();
        _checkFail?.Dispose();
    }

    private void MakeKeyCodeNCharTable()
    {
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.Q))
        {
            _keyCodeNCharPair.Add(KeyCode.Q, 'ㅂ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.W))
        {
            _keyCodeNCharPair.Add(KeyCode.W, 'ㅈ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.E))
        {
            _keyCodeNCharPair.Add(KeyCode.E, 'ㄷ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.R))
        {
            _keyCodeNCharPair.Add(KeyCode.R, 'ㄱ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.T))
        {
            _keyCodeNCharPair.Add(KeyCode.T, 'ㅅ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.Y))
        {
            _keyCodeNCharPair.Add(KeyCode.Y, 'ㅛ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.U))
        {
            _keyCodeNCharPair.Add(KeyCode.U, 'ㅕ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.I))
        {
            _keyCodeNCharPair.Add(KeyCode.I, 'ㅑ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.O))
        {
            _keyCodeNCharPair.Add(KeyCode.O, 'ㅐ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.P))
        {
            _keyCodeNCharPair.Add(KeyCode.P, 'ㅔ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.A))
        {
            _keyCodeNCharPair.Add(KeyCode.A, 'ㅁ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.S))
        {
            _keyCodeNCharPair.Add(KeyCode.S, 'ㄴ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.D))
        {
            _keyCodeNCharPair.Add(KeyCode.D, 'ㅇ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.F))
        {
            _keyCodeNCharPair.Add(KeyCode.F, 'ㄹ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.G))
        {
            _keyCodeNCharPair.Add(KeyCode.G, 'ㅎ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.H))
        {
            _keyCodeNCharPair.Add(KeyCode.H, 'ㅗ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.J))
        {
            _keyCodeNCharPair.Add(KeyCode.J, 'ㅓ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.K))
        {
            _keyCodeNCharPair.Add(KeyCode.K, 'ㅏ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.L))
        {
            _keyCodeNCharPair.Add(KeyCode.L, 'ㅣ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.Z))
        {
            _keyCodeNCharPair.Add(KeyCode.Z, 'ㅋ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.X))
        {
            _keyCodeNCharPair.Add(KeyCode.X, 'ㅌ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.C))
        {
            _keyCodeNCharPair.Add(KeyCode.C, 'ㅊ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.V))
        {
            _keyCodeNCharPair.Add(KeyCode.V, 'ㅍ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.B))
        {
            _keyCodeNCharPair.Add(KeyCode.B, 'ㅠ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.N))
        {
            _keyCodeNCharPair.Add(KeyCode.N, 'ㅜ');
        }
        if (!_keyCodeNCharPair.ContainsKey(KeyCode.M))
        {
            _keyCodeNCharPair.Add(KeyCode.M, 'ㅡ');
        }

    }

    public int GetScore()
    {
        return score;
    }

    public bool IsTimeOver()
    {
        return _isTimeOver;
    }

    public char GetCurrentChar()
    {
       return _nonDuplicateString[_stringPointer % _nonDuplicateString.Length];
    }
   
}