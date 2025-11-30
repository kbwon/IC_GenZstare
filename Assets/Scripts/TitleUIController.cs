using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIController : MonoBehaviour
{
    [Header("Next Scene Settings")]
    [SerializeField]
    private string loginSceneName = "StartEndScene";

    [Header("AppFlowManager (Optional)")]
    [Tooltip("처음 실행 시 AppFlowManager 프리팹을 한 번만 생성하고 싶다면 여기에 넣어주세요.")]
    [SerializeField]
    private FlowManager flowManagerPrefab;

    private void Awake()
    {
        // FlowManager가 아직 없고, 프리팹이 세팅되어 있으면 한 번만 생성
        if (FlowManager.Instance == null && flowManagerPrefab != null)
        {
            Instantiate(flowManagerPrefab);
        }
    }

    public void OnClickStart()
    {
        if (string.IsNullOrEmpty(loginSceneName))
        {
            Debug.LogError("[TitleUIController] loginSceneName 이 설정되지 않았습니다.");
            return;
        }

        SceneManager.LoadScene(loginSceneName);
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        Debug.Log("[TitleUIController] Quit 눌림 (에디터에서는 종료 대신 로그만 출력)");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
