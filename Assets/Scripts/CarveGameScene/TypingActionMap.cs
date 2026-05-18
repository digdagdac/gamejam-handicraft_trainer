using Unity.MLAgents.Sensors;
using UnityEngine;

public struct TypingActionResult
{
    public readonly bool IsValidAction;
    public readonly bool IsCorrect;
    public readonly bool IsComplete;
    public readonly KeyCode Key;
    public readonly char TargetChar;
    public readonly char InputChar;
    public readonly int PreviousScore;
    public readonly int CurrentScore;
    public readonly int ScoreDelta;
    public readonly int ProgressIndex;
    public readonly int SequenceLength;
    public readonly int Combo;

    public TypingActionResult(
        bool isValidAction,
        bool isCorrect,
        bool isComplete,
        KeyCode key,
        char targetChar,
        char inputChar,
        int previousScore,
        int currentScore,
        int progressIndex,
        int sequenceLength,
        int combo)
    {
        IsValidAction = isValidAction;
        IsCorrect = isCorrect;
        IsComplete = isComplete;
        Key = key;
        TargetChar = targetChar;
        InputChar = inputChar;
        PreviousScore = previousScore;
        CurrentScore = currentScore;
        ScoreDelta = currentScore - previousScore;
        ProgressIndex = progressIndex;
        SequenceLength = sequenceLength;
        Combo = combo;
    }
}

public static class TypingActionMap
{
    public const int ActionCount = 26;

    private static readonly KeyCode[] Keys =
    {
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T,
        KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G,
        KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B,
        KeyCode.N, KeyCode.M
    };

    private static readonly char[] Chars =
    {
        'ㅂ', 'ㅈ', 'ㄷ', 'ㄱ', 'ㅅ',
        'ㅛ', 'ㅕ', 'ㅑ', 'ㅐ', 'ㅔ',
        'ㅁ', 'ㄴ', 'ㅇ', 'ㄹ', 'ㅎ',
        'ㅗ', 'ㅓ', 'ㅏ', 'ㅣ',
        'ㅋ', 'ㅌ', 'ㅊ', 'ㅍ', 'ㅠ',
        'ㅜ', 'ㅡ'
    };

    public static bool TryGetKey(int actionIndex, out KeyCode key)
    {
        if (actionIndex < 0 || actionIndex >= Keys.Length)
        {
            key = KeyCode.None;
            return false;
        }

        key = Keys[actionIndex];
        return true;
    }

    public static bool TryGetChar(int actionIndex, out char value)
    {
        if (actionIndex < 0 || actionIndex >= Chars.Length)
        {
            value = '\0';
            return false;
        }

        value = Chars[actionIndex];
        return true;
    }

    public static bool TryGetChar(KeyCode key, out char value)
    {
        for (int i = 0; i < Keys.Length; i++)
        {
            if (Keys[i] == key)
            {
                value = Chars[i];
                return true;
            }
        }

        value = '\0';
        return false;
    }

    public static bool TryGetAction(char value, out int actionIndex)
    {
        for (int i = 0; i < Chars.Length; i++)
        {
            if (Chars[i] == value)
            {
                actionIndex = i;
                return true;
            }
        }

        actionIndex = -1;
        return false;
    }

    public static bool TryGetPressedAction(out int actionIndex)
    {
        for (int i = 0; i < Keys.Length; i++)
        {
            if (Input.GetKeyDown(Keys[i]))
            {
                actionIndex = i;
                return true;
            }
        }

        actionIndex = 0;
        return false;
    }

    public static void AddOneHotObservation(VectorSensor sensor, char targetChar)
    {
        TryGetAction(targetChar, out int targetAction);

        for (int i = 0; i < ActionCount; i++)
        {
            sensor.AddObservation(i == targetAction ? 1f : 0f);
        }
    }
}
