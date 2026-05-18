using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class MinigameSetting : MonoBehaviour
{
    public string targetLetter; // 화면에 표시될 목표 글자
    public GameObject letterDisplay; // UI 텍스트로 출력할 오브젝트
    public int maxSteps = 1000; // 에피소드 최대 길이
    private int currentStep;

    // 학습 환경 초기화
    public void InitEnvironment()
    {
        // 랜덤으로 한글 문자 설정
        targetLetter = GetRandomLetter();
        UpdateLetterDisplay(targetLetter);
        currentStep = 0;
    }

    public string GetRandomLetter()
    {
        // Unicode 범위에서 랜덤으로 한글자 선택
        int unicodeStart = 0xAC00; // 한글 시작
        int unicodeEnd = 0xD7A3; // 한글 끝
        int randomUnicode = Random.Range(unicodeStart, unicodeEnd + 1);
        return char.ConvertFromUtf32(randomUnicode);
    }

    public void UpdateLetterDisplay(string letter)
    {
        // 화면 UI에 글자 업데이트
        letterDisplay.GetComponent<UnityEngine.UI.Text>().text = letter;
    }

    public bool CheckInput(string input)
    {
        // 입력이 올바른지 확인
        return input == targetLetter;
    }

    public bool IsEpisodeDone()
    {
        // 에피소드 종료 조건
        return currentStep >= maxSteps;
    }

    public void Step()
    {
        currentStep++;
    }
}
