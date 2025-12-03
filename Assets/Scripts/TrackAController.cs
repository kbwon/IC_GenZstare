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

    private SessionReport report;
    private int step = 0;   // 0~3

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
        npcLineText.text = "카페 알바: 안녕하세요! 주문 어떻게 도와드릴까요?";

        SetOption(0, "안녕하세요!", true);
        SetOption(1, "...", true);
        SetOption(2, "아, 네.", true);

        feedbackText.text = "먼저 간단히 인사해 보세요.";
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
        npcLineText.text = "카페 알바: 오늘은 어떤 음료 준비해 드릴까요?";

        SetOption(0, "아이스 아메리카노 한 잔, 따뜻한 라떼 한 잔 포장해주세요.", true);
        SetOption(1, "따뜻한 아메리카노 한 잔, 아이스 라떼 한 잔 포장해주세요.", true);
        SetOption(2, "그.. 아.. 그냥 커피요.", true);
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
        npcLineText.text = "카페 알바: 아이스 아메리카노, 따뜻한 라떼 포장 나왔습니다! 맛있게 드세요~";

        SetOption(0, "감사합니다.", true);
        SetOption(1, "", false);
        SetOption(2, "", false);

        feedbackText.text = "감사 인사로 마무리해 보세요.";
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
    }
}
