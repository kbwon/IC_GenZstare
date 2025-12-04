using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TrackAController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text npcLineText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TMP_Text[] optionTexts;

    [Header("Result Scene")]
    [SerializeField] private string resultSceneName = "ResultScene";

    [Header("Audio")]
    [SerializeField] private AudioSource npcAudioSource;
    [Tooltip("각 스텝별 NPC 대사 음성 (0: 인사, 1: 주문, 2: 포장/매장 질문, 3: 음료 나왔습니다)")]
    [SerializeField] private AudioClip[] npcLineClips;

    private SessionReport report;
    private int step = 0;

    private int cases = 0;
    private string step1Answer;
    private string step2Answer;

    private void Start()
    {
        var flow = FlowManager.Instance;
        report = new SessionReport
        {
            userId = flow != null ? flow.currentUserId : "Guest",
            track = "A"
        };
        if (flow != null)
            flow.currentSessionReport = report;

        feedbackText.text = "";
        ShowStep0();
    }

    // 버튼 OnClick에 index 0,1,2 연결
    public void OnClickOption(int index)
    {
        switch (step)
        {
            case 0:
                HandleStep0(index);
                break;
            case 1:
                HandleStep1(index);
                break;
            case 2:
                HandleStep2(index);
                break;
            case 3:
                HandleStep3(index);
                break;
        }
    }

    // --- STEP 0: 점원 인사 ---

    private void ShowStep0()
    {
        step = 0;
        npcLineText.text = "카페 알바: 어서오세요~ \n 목표: 아이스 아메리카노 1잔, 따뜻한 라떼 한잔 포장하기";

        SetOption(0, "안녕하세요!", true);
        SetOption(1, "...", true);
        SetOption(2, "아, 네.", true);

        feedbackText.text = "먼저 간단히 인사해 보세요.";

        PlayNpcLineAudio(0);
    }

    private void HandleStep0(int index)
    {
        if (index == 0)
        {
            report.goodChoices++;
            feedbackText.text = "먼저 인사로 시작하면 분위기가 부드러워집니다.";
        }
        else if (index == 1)
        {
            report.badChoices++;
            feedbackText.text = "인사를 하지 않으면 조금 어색하게 느껴질 수 있어요.";
        }
        else
        {
            report.neutralChoices++;
            feedbackText.text = "말은 했지만, 조금 애매하게 느껴질 수 있어요.";
        }

        ShowStep1();
    }

    // --- STEP 1: 주문 내용 ---

    private void ShowStep1()
    {
        step = 1;
        npcLineText.text = "안녕하세요! 주문 어떻게 도와드릴까요?";

        SetOption(0, "아이스 아메리카노 한 잔, 따뜻한 라떼 한 잔 포장해주세요.", true);
        SetOption(1, "따뜻한 아메리카노 한 잔, 아이스 라떼 한 잔 포장해주세요.", true);
        SetOption(2, "그.. 아.. 그냥 커피요.", true);

        PlayNpcLineAudio(1);
    }

    private void HandleStep1(int index)
    {
        if (index == 0)
        {
            report.goodChoices++;
            feedbackText.text = "구체적으로 말해서 주문이 정확하게 전달됩니다.";
        }
        else if (index == 1)
        {
            report.neutralChoices++;
            feedbackText.text = "의도와 조금 다른 조합으로 전달될 수 있어요.";
        }
        else
        {
            report.badChoices++;
            feedbackText.text = "정보가 부족해서 상대가 다시 물어봐야 할 수 있어요.";
        }

        ShowStep2();
    }

    // --- STEP 2: 포장 or 매장 ---

    private void ShowStep2()
    {
        step = 2;
        npcLineText.text = "카페 알바: 포장해 드릴까요, 매장에서 드실까요?";

        SetOption(0, "포장해서 갈게요.", true);
        SetOption(1, "매장에서 먹고 갈게요.", true);
        SetOption(2, "", false); // 필요 없으면 숨기기

        PlayNpcLineAudio(2);
    }

    private void HandleStep2(int index)
    {
        if (index == 0)
        {
            report.goodChoices++;
            feedbackText.text = "포장을 선택했습니다. 다음 대사로 넘어갑니다.";
        }
        else
        {
            report.neutralChoices++;
            feedbackText.text = "매장에서 먹고 가는 선택도 괜찮습니다.";
        }

        ShowStep3();
    }

    // --- STEP 3: 음료 수령 & 감사 인사 ---

    private void ShowStep3()
    {
        step = 3;
        npcLineText.text = "카페 알바:" + step1Answer + step2Answer + "나왔습니다! 맛있게 드세요~";

        SetOption(0, "감사합니다.", true);
        SetOption(1, "", false);
        SetOption(2, "", false);

        feedbackText.text = "감사 인사로 마무리해 보세요.";
        switch(cases)
        {
            case 0:
                //아이스 아메리카노, 따뜻한 라떼 포장
                PlayNpcLineAudio(3);
                break;
            case 1:
                //따뜻한 아메리카노, 아이스 라떼 포장
                PlayNpcLineAudio(4);
                break;
            case 2:
                //아메리카노 포장
                PlayNpcLineAudio(5);
                break;
            case 3:
                //아이스 아메리카노, 따뜻한 라떼 매장
                PlayNpcLineAudio(6);
                break;
            case 4:
                //따뜻한 아메리카노, 아이스 라떼 매장
                PlayNpcLineAudio(7);
                break;
            case 5:
                //아메리카노 매장
                PlayNpcLineAudio(8);
                break;

        }
    }

    private void HandleStep3(int index)
    {
        if (index == 0)
        {
            report.goodChoices++;
            feedbackText.text = "감사 인사를 하면 상대에게 좋은 인상을 남길 수 있습니다.";
        }

        // 리포트 씬으로 이동
        if (!string.IsNullOrEmpty(resultSceneName))
        {
            SceneManager.LoadScene(resultSceneName);
        }
        else
        {
            Debug.LogError("[TrackAController] resultSceneName 이 설정되지 않았습니다.");
        }
    }

    // --- 공용 유틸리티 ---

    private void SetOption(int index, string text, bool active)
    {
        if (index < 0 || index >= optionButtons.Length) return;

        optionButtons[index].gameObject.SetActive(active);
        if (active)
        {
            optionTexts[index].text = text;
        }

        if(step == 1)
        {
            if(index == 0)
            {
                step1Answer = "아이스 아메리카노, 따뜻한 라떼";
            }
            else if(index == 1)
            {
                step1Answer = "따뜻한 아메리카노, 아이스 라떼";
                cases++;
            }
            else if(index == 2)
            {
                step1Answer = "아메리카노";
                cases += 2;
            }
        }
        else if(step == 2)
        {
            if (index == 0)
            {
                step2Answer = "포장";
            }
            else if (index == 1)
            {
                step2Answer = "";
                cases += 3;
            }
            else if (index == 2)
            {
                
            }
        }
    }

    private void PlayNpcLineAudio(int clipIndex)
    {
        if (npcAudioSource == null) return;
        if (npcLineClips == null) return;
        if (clipIndex < 0 || clipIndex >= npcLineClips.Length) return;
        if (npcLineClips[clipIndex] == null) return;

        npcAudioSource.Stop();
        npcAudioSource.clip = npcLineClips[clipIndex];
        npcAudioSource.Play();
    }

}
