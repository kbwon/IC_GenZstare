using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUIController : MonoBehaviour
{
    [Header("Next Scenes")]
    [SerializeField] private string tutorialSceneName = "Room_Tutorial";
    [SerializeField] private string checkSceneName = "Room_Check";

    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject tutorialChoicePanel;    // 튜토리얼 진행 여부 묻는 패널
    [SerializeField] private GameObject guestTrackChoicePanel;  // 게스트용 A/B 선택 패널

    private void Start()
    {
        if (tutorialChoicePanel != null)
            tutorialChoicePanel.SetActive(false);
        if (guestTrackChoicePanel != null)
            guestTrackChoicePanel.SetActive(false);
    }

    // --- 로그인 버튼들 ---

    public void OnClickUser1()
    {
        var app = FlowManager.Instance;
        app.currentUserId = "User1";
        app.isGuest = false;
        app.currentTrack = TrackType.A;   // 외부 설문 결과가 A라고 가정
        loginPanel.SetActive(false);
        ShowTutorialChoice();
    }

    public void OnClickUser2()
    {
        var app = FlowManager.Instance;
        app.currentUserId = "User2";
        app.isGuest = false;
        app.currentTrack = TrackType.B;   // 외부 설문 결과가 B라고 가정
        loginPanel.SetActive(false);
        ShowTutorialChoice();
    }

    public void OnClickGuest()
    {
        var app = FlowManager.Instance;
        app.currentUserId = "Guest";
        app.isGuest = true;

        loginPanel.SetActive(false);

        // 게스트는 먼저 A/B 중에서 체험할 트랙을 고르게
        if (guestTrackChoicePanel != null)
            guestTrackChoicePanel.SetActive(true);
    }

    // --- 게스트용 트랙 선택 ---

    public void OnClickGuestTrackA()
    {
        var app = FlowManager.Instance;
        app.currentTrack = TrackType.A;
        guestTrackChoicePanel.SetActive(false);
        ShowTutorialChoice();
    }

    public void OnClickGuestTrackB()
    {
        var app = FlowManager.Instance;
        app.currentTrack = TrackType.B;
        guestTrackChoicePanel.SetActive(false);
        ShowTutorialChoice();
    }

    // --- 튜토리얼 진행 여부 ---

    private void ShowTutorialChoice()
    {
        if (tutorialChoicePanel != null)
            tutorialChoicePanel.SetActive(true);
    }

    public void OnClickDoTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void OnClickSkipTutorial()
    {
        FlowManager.Instance.tutorialDone = true;
        SceneManager.LoadScene(checkSceneName);
    }
}
