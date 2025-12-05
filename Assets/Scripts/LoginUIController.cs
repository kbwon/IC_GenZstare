using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelTrackChoice;      // A/B/Guest 처음 선택 패널
    [SerializeField] private GameObject panelInfoTrackA;       // A 안내 패널
    [SerializeField] private GameObject panelInfoTrackB;       // B 안내 패널
    [SerializeField] private GameObject panelInfoGuest;        // Guest 안내 패널
    [SerializeField] private GameObject panelGuestTrackChoice; // Guest용 A/B 선택 패널
    [SerializeField] private GameObject panelTutorialChoice;   // 튜토리얼 여부 패널

    [Header("Next Scenes")]
    [SerializeField] private string tutorialSceneName = "Room_Tutorial";
    [SerializeField] private string checkSceneName = "Room_Check";   // 거울 씬

    private TrackType selectedTrack = TrackType.A;
    private bool isGuestMode = false;

    private void Start()
    {
        ShowPanelTrackChoice();
    }

    // ---------- Panel 전환 유틸 ----------

    private void ShowPanelTrackChoice()
    {
        SetAllPanelsInactive();
        if (panelTrackChoice != null) panelTrackChoice.SetActive(true);
    }

    private void ShowPanelInfoTrackA()
    {
        SetAllPanelsInactive();
        if (panelInfoTrackA != null) panelInfoTrackA.SetActive(true);
    }

    private void ShowPanelInfoTrackB()
    {
        SetAllPanelsInactive();
        if (panelInfoTrackB != null) panelInfoTrackB.SetActive(true);
    }

    private void ShowPanelInfoGuest()
    {
        SetAllPanelsInactive();
        if (panelInfoGuest != null) panelInfoGuest.SetActive(true);
    }

    private void ShowPanelGuestTrackChoice()
    {
        SetAllPanelsInactive();
        if (panelGuestTrackChoice != null) panelGuestTrackChoice.SetActive(true);
    }

    private void ShowPanelTutorialChoice()
    {
        SetAllPanelsInactive();
        if (panelTutorialChoice != null) panelTutorialChoice.SetActive(true);
    }

    private void SetAllPanelsInactive()
    {
        if (panelTrackChoice != null) panelTrackChoice.SetActive(false);
        if (panelInfoTrackA != null) panelInfoTrackA.SetActive(false);
        if (panelInfoTrackB != null) panelInfoTrackB.SetActive(false);
        if (panelInfoGuest != null) panelInfoGuest.SetActive(false);
        if (panelGuestTrackChoice != null) panelGuestTrackChoice.SetActive(false);
        if (panelTutorialChoice != null) panelTutorialChoice.SetActive(false);
    }

    // ---------- 1단계: A / B / Guest 선택 ----------

    public void OnClickTrackA()
    {
        selectedTrack = TrackType.A;
        isGuestMode = false;

        var flow = FlowManager.Instance;
        if (flow != null)
        {
            flow.currentTrack = TrackType.A;
            flow.isGuest = false;
        }

        // 바로 튜토리얼로 가지 않고, A 트랙 안내 패널 먼저
        ShowPanelInfoTrackA();
    }

    public void OnClickTrackB()
    {
        selectedTrack = TrackType.B;
        isGuestMode = false;

        var flow = FlowManager.Instance;
        if (flow != null)
        {
            flow.currentTrack = TrackType.B;
            flow.isGuest = false;
        }

        // B 트랙 안내 패널 먼저
        ShowPanelInfoTrackB();
    }

    public void OnClickGuest()
    {
        isGuestMode = true;

        var flow = FlowManager.Instance;
        if (flow != null)
        {
            flow.isGuest = true;
            // Guest 모드에서는 아직 트랙을 확정하지 않음 (나중에 선택)
        }

        // Guest 모드 안내 패널 먼저
        ShowPanelInfoGuest();
    }

    // ---------- 2단계: 안내 패널에서 "다음" 버튼 ----------

    // A 안내 패널의 "다음" 버튼
    public void OnClickInfoTrackANext()
    {
        // A 트랙은 바로 튜토리얼 여부로 이동
        ShowPanelTutorialChoice();
    }

    // B 안내 패널의 "다음" 버튼
    public void OnClickInfoTrackBNext()
    {
        // B 트랙도 바로 튜토리얼 여부로 이동
        ShowPanelTutorialChoice();
    }

    // Guest 안내 패널의 "다음" 버튼
    public void OnClickInfoGuestNext()
    {
        // Guest인 경우에는 이제 A/B 중 어떤 상황을 체험할지 선택
        ShowPanelGuestTrackChoice();
    }

    // ---------- 3단계: Guest 모드에서 A/B 선택 ----------

    public void OnClickGuestChooseTrackA()
    {
        selectedTrack = TrackType.A;

        var flow = FlowManager.Instance;
        if (flow != null)
        {
            flow.currentTrack = TrackType.A;
            // isGuest는 이미 true로 세팅되어 있음
        }

        ShowPanelTutorialChoice();
    }

    public void OnClickGuestChooseTrackB()
    {
        selectedTrack = TrackType.B;

        var flow = FlowManager.Instance;
        if (flow != null)
        {
            flow.currentTrack = TrackType.B;
        }

        ShowPanelTutorialChoice();
    }

    // ---------- 4단계: 튜토리얼 여부 ----------

    public void OnClickDoTutorial()
    {
        var flow = FlowManager.Instance;
        if (flow != null)
        {
            flow.tutorialDone = false; // 아직 안 했고, 이제 하러 감
        }

        SceneManager.LoadScene(tutorialSceneName);
    }

    public void OnClickSkipTutorial()
    {
        var flow = FlowManager.Instance;
        if (flow != null)
        {
            flow.tutorialDone = true;
        }

        // 튜토리얼을 건너뛰고 바로 거울(페이셜/보이스 체크) 씬으로
        SceneManager.LoadScene(checkSceneName);
    }
}
