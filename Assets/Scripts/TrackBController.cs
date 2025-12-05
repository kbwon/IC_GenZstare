using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TrackBController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text npcLineText;
    [SerializeField] private TMP_Text feedbackText;
    //[SerializeField] private TMP_Text tipsText;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TMP_Text[] optionTexts;

    [Header("Result Scene")]
    [SerializeField] private string resultSceneName = "Room_Result";

    [Header("Audio")]
    [SerializeField] private AudioSource npcAudioSource;
    [Tooltip("각 스텝별 NPC 대사 음성 (0: 점심 어땠어요, 1: 평소 점심, 2: 오후 일정, 3: 마무리)")]
    [SerializeField] private AudioClip[] npcLineClips;

    private SessionReport report;
    private int step = 0;   // 0~3

    private void Start()
    {
        var flow = FlowManager.Instance;
        report = new SessionReport
        {
            userId = flow != null ? flow.currentUserId : "Guest",
            track = "B"
        };
        if (flow != null)
            flow.currentSessionReport = report;

        feedbackText.text = "";
        //tipsText.text = "표정, 시선, 응답 속도를 의식해 보세요.";
        ShowStep0();
    }

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

    // --- STEP 0: 점심 어땠어요? ---

    private void ShowStep0()
    {
        step = 0;
        npcLineText.text = "박주임: 점심 어땠어요? 오늘 구내식당 신메뉴 먹어봤어요?";

        SetOption(0, "네, 꽤 괜찮았어요. 소스가 생각보다 달달하더라고요. 대리님은 어땠어요?", true);
        SetOption(1, "먹어봤는데… 저는 그냥 그렇다라구요.", true);
        SetOption(2, "… 아 넵. 먹어봤어요.", true);

        feedbackText.text = "공감과 되묻기를 섞어서 대답해 볼까요?";
        //tipsText.text = "미소 + 상대에게 되묻기 한 번 정도면 충분합니다.";

        StartCoroutine(PlayNpcLineAudioDelayed(3.0f, 0));
    }

    private void HandleStep0(int index)
    {
        if (index == 0)
        {
            report.goodChoices++;
            feedbackText.text = "공감과 되묻기로 대화를 자연스럽게 이어갔어요.";
        }
        else if (index == 1)
        {
            report.neutralChoices++;
            feedbackText.text = "사실만 말하면 조금 건조하게 느껴질 수 있어요.";
        }
        else
        {
            report.badChoices++;
            feedbackText.text = "짧은 대답과 긴 침묵은 서로를 어색하게 만들 수 있어요.";
        }

        ShowStep1();
    }

    // --- STEP 1: 평소 점심은? ---

    private void ShowStep1()
    {
        step = 1;
        npcLineText.text = "박주임: 보통 점심은 뭐 자주 드세요? 구내식당 자주 가세요?";

        SetOption(0, "평일엔 가볍게 먹는 편이에요. 가끔 회사 앞 덮밥집 가는데, 다음엔 같이 식사하실래요?", true);
        SetOption(1, "그냥 가까운 데서 먹어요.", true);
        SetOption(2, "네, 구내식당 가요.", true);

        feedbackText.text = "조금 더 이야기를 넓혀볼 수도 있어요.";
        //tipsText.text = "초대를 곁들이면 관계가 조금 더 가까워질 수 있습니다.";

        PlayNpcLineAudio(1);
    }

    private void HandleStep1(int index)
    {
        if (index == 0)
        {
            report.goodChoices++;
            feedbackText.text = "초대를 곁들여서 분위기가 더 따뜻해졌어요.";
        }
        else if (index == 1)
        {
            report.neutralChoices++;
            feedbackText.text = "정보는 전달되지만, 대화가 쉽게 끊길 수 있어요.";
        }
        else
        {
            report.badChoices++;
            feedbackText.text = "너무 짧게 말하면 상대가 더 물어보기가 어렵습니다.";
        }

        ShowStep2();
    }

    // --- STEP 2: 오후 일정 ---

    private void ShowStep2()
    {
        step = 2;
        npcLineText.text = "박주임: 그나저나 요즘 마감 때문에 일 바쁘죠? 오후 일정은 괜찮으세요?";

        SetOption(0, "네, 조금 바쁘지만 괜찮아요. 대리님은요? 오후에 회의 많으세요?", true);
        SetOption(1, "그냥 보통인 것 같아요.", true);
        SetOption(2, "… 어… 잘 모르겠습니다.", true);

        feedbackText.text = "상대의 일정도 함께 물어볼 수 있어요.";
        //tipsText.text = "상대에게도 되묻는 한 문장이 관계에 큰 차이를 만듭니다.";

        PlayNpcLineAudio(2);
    }

    private void HandleStep2(int index)
    {
        if (index == 0)
        {
            report.goodChoices++;
            feedbackText.text = "상대의 일정도 물어보며 관심을 표현했어요.";
        }
        else if (index == 1)
        {
            report.neutralChoices++;
            feedbackText.text = "문제는 없지만, 조금 덜 따뜻하게 느껴질 수 있어요.";
        }
        else
        {
            report.badChoices++;
            feedbackText.text = "눈을 잘 마주치지 않고 애매한 말로 넘기면 상대가 걱정할 수 있어요.";
        }

        ShowStep3();
    }

    // --- STEP 3: 마무리 ---

    private void ShowStep3()
    {
        step = 3;
        npcLineText.text = "박주임: 그래도 점심 같이 얘기하니까 좋네요. 오후에도 파이팅해요!";

        SetOption(0, "네, 감사합니다. 대리님도 파이팅하세요!", true);
        SetOption(1, "어… 네.", true);
        SetOption(2, "", false);

        feedbackText.text = "간단한 응원과 감사 인사로 마무리해 봅니다.";
        //tipsText.text = "마무리 인사는 길지 않아도 괜찮아요.";

        PlayNpcLineAudio(3);
    }

    private void HandleStep3(int index)
    {
        if (index == 0)
        {
            report.goodChoices++;
            feedbackText.text = "상대도 기분 좋게 대화를 마무리할 수 있습니다.";
        }
        else
        {
            report.neutralChoices++;
            feedbackText.text = "대화는 끝나지만, 조금 아쉬운 느낌일 수 있어요.";
        }

        if (!string.IsNullOrEmpty(resultSceneName))
        {
            SceneManager.LoadScene(resultSceneName);
        }
        else
        {
            Debug.LogError("[TrackBController] resultSceneName 이 설정되지 않았습니다.");
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

    private IEnumerator PlayNpcLineAudioDelayed(float delay, int clipIndex)
    {
        // 지정한 시간만큼 기다렸다가
        yield return new WaitForSeconds(delay);

        // 아직도 같은 스텝이면 (이미 다음 스텝으로 넘어간 상태가 아니면) 재생
        if (step == 0) // 필요하면 원하는 step 번호로 조건 바꾸기
        {
            PlayNpcLineAudio(clipIndex);
        }
    }
}