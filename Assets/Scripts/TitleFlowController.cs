using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TitleStage
{
    Login,
    ProgramIntro,
    SurveyIntro,
    Analyzing
}

public class TitleFlowController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject panelProgramIntro;
    [SerializeField] private GameObject panelSurveyIntro;
    [SerializeField] private GameObject panelAnalyzing;

    [Header("Program Intro")]
    [SerializeField] private Toggle toggleAgree;

    [Header("Survey / Next Scene")]
    [SerializeField] private string loginSceneName = "Room_Login";
    [SerializeField] private float analyzingDelaySeconds = 2.0f;

    [Header("Recommended Track")]
    [SerializeField] private TrackType recommendedTrack = TrackType.A;

    private TitleStage currentStage = TitleStage.Login;

    private void Start()
    {
        SetStage(TitleStage.Login);
    }

    private void SetStage(TitleStage stage)
    {
        currentStage = stage;

        if (panelLogin != null) panelLogin.SetActive(stage == TitleStage.Login);
        if (panelProgramIntro != null) panelProgramIntro.SetActive(stage == TitleStage.ProgramIntro);
        if (panelSurveyIntro != null) panelSurveyIntro.SetActive(stage == TitleStage.SurveyIntro);
        if (panelAnalyzing != null) panelAnalyzing.SetActive(stage == TitleStage.Analyzing);
    }

    // --- UI 버튼용 메서드 ---

    public void OnClickLogin()
    {
        SetStage(TitleStage.ProgramIntro);
    }

    public void OnClickAgree()
    {
        if (toggleAgree != null && !toggleAgree.isOn)
        {
            Debug.Log("[TitleFlowController] 동의 토글이 체크되지 않았습니다.");
            return;
        }

        SetStage(TitleStage.SurveyIntro);
    }

    public void OnClickDecline()
    {
#if UNITY_EDITOR
        Debug.Log("[TitleFlowController] 동의하지 않고 종료 선택");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClickLoadSurveyResult()
    {
        SetStage(TitleStage.Analyzing);

        // 설문 결과를 통해 추천된 트랙을 FlowManager에 저장
        var flow = FlowManager.Instance;
        if (flow != null)
        {
            flow.currentTrack = recommendedTrack;
            flow.isGuest = false;
        }

        StartCoroutine(GoToLoginAfterDelay(analyzingDelaySeconds));
    }

    private IEnumerator GoToLoginAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(loginSceneName);
    }

    // --- XR 입력에서 호출할 "오른손/왼손 액션" ---

    public void OnRightAction()
    {
        switch (currentStage)
        {
            case TitleStage.Login:
                OnClickLogin();
                break;
            case TitleStage.ProgramIntro:
                OnClickAgree();
                break;
            case TitleStage.SurveyIntro:
                OnClickLoadSurveyResult();
                break;
            case TitleStage.Analyzing:
                // 분석 중에는 무시
                break;
        }
    }

    public void OnLeftAction()
    {
        switch (currentStage)
        {
            case TitleStage.Login:
            case TitleStage.ProgramIntro:
            case TitleStage.SurveyIntro:
                OnClickDecline();
                break;
            case TitleStage.Analyzing:
                // 분석 중에는 특별히 할 일 없음
                break;
        }
    }
}
