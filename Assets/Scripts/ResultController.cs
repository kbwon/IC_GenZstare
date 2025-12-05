using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using TMPro;

public class ResultController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text summaryText;

    [Header("Scenes")]
    [SerializeField] private string mainTitleSceneName = "Room_Title";

    // XR 컨트롤러 장치 목록
    private readonly List<InputDevice> leftHandDevices = new List<InputDevice>();
    private readonly List<InputDevice> rightHandDevices = new List<InputDevice>();

    // 한 번 돌아가거나 종료되면 중복 실행 방지
    private bool hasDecided = false;

    private void Start()
    {
        var flow = FlowManager.Instance;
        if (flow == null || flow.currentSessionReport == null)
        {
            summaryText.text = "세션 정보가 없습니다.\n\n왼손 컨트롤러 버튼: 종료\n오른손 컨트롤러 버튼: 메인으로";
            return;
        }

        var r = flow.currentSessionReport;

        // track은 string ("A" / "B") 기준
        string trackName;
        if (r.track == "A")
        {
            trackName = "A트랙 (카페 주문)";
        }
        else if (r.track == "B")
        {
            trackName = "B트랙 (사무실 스몰토크)";
        }
        else
        {
            trackName = $"알 수 없는 트랙 ({r.track})";
        }

        summaryText.text =
            $"유저: {r.userId}\n" +
            $"진행한 트랙: {trackName}\n\n" +
            $"권장 응답 선택: {r.goodChoices}회\n" +
            $"중립 응답 선택: {r.neutralChoices}회\n" +
            $"미흡한 응답 선택: {r.badChoices}회\n\n" +
            "※ 실제 서비스에서는 이 결과와 사전/사후 설문, 얼굴·음성 분석 정보를 함께 활용하여\n" +
            "   더 정교한 피드백 리포트를 제공할 예정입니다.\n\n" +
            "오른손 컨트롤러 버튼: 메인으로 돌아가기\n" +
            "왼손 컨트롤러 버튼: 프로그램 종료";
    }

    private void OnEnable()
    {
        RefreshDevices();
        InputDevices.deviceConnected += OnDeviceConnected;
        InputDevices.deviceDisconnected += OnDeviceDisconnected;
    }

    private void OnDisable()
    {
        InputDevices.deviceConnected -= OnDeviceConnected;
        InputDevices.deviceDisconnected -= OnDeviceDisconnected;
    }

    // --- XR Device 관리 ---

    private void RefreshDevices()
    {
        leftHandDevices.Clear();
        rightHandDevices.Clear();

        var allDevices = new List<InputDevice>();
        InputDevices.GetDevices(allDevices);

        foreach (var device in allDevices)
        {
            AddDevice(device);
        }
    }

    private void AddDevice(InputDevice device)
    {
        if ((device.characteristics & InputDeviceCharacteristics.Controller) == 0)
            return;

        if ((device.characteristics & InputDeviceCharacteristics.Right) != 0)
        {
            if (!rightHandDevices.Contains(device))
                rightHandDevices.Add(device);
        }
        else if ((device.characteristics & InputDeviceCharacteristics.Left) != 0)
        {
            if (!leftHandDevices.Contains(device))
                leftHandDevices.Add(device);
        }
    }

    private void OnDeviceConnected(InputDevice device)
    {
        AddDevice(device);
    }

    private void OnDeviceDisconnected(InputDevice device)
    {
        leftHandDevices.Remove(device);
        rightHandDevices.Remove(device);
    }

    private void Update()
    {
        if (hasDecided) return;

        // 오른손 컨트롤러 버튼 → 메인으로 돌아가기
        if (CheckAnyButtonPressed(rightHandDevices))
        {
            hasDecided = true;
            OnClickRestart();
            return;
        }

        // 왼손 컨트롤러 버튼 → 종료
        if (CheckAnyButtonPressed(leftHandDevices))
        {
            hasDecided = true;
            OnClickQuit();
            return;
        }
    }

    /// <summary>
    /// 주어진 컨트롤러 리스트에서 primary/secondary/trigger/grip 중
    /// 아무 버튼이나 눌렸는지 확인
    /// </summary>
    private bool CheckAnyButtonPressed(List<InputDevice> devices)
    {
        foreach (var device in devices)
        {
            bool primary, secondary, trigger, grip;

            if ((device.TryGetFeatureValue(CommonUsages.primaryButton, out primary) && primary) ||
                (device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondary) && secondary) ||
                (device.TryGetFeatureValue(CommonUsages.triggerButton, out trigger) && trigger) ||
                (device.TryGetFeatureValue(CommonUsages.gripButton, out grip) && grip))
            {
                return true;
            }
        }
        return false;
    }

    // --- UI 버튼용 함수 (타이틀/종료) ---

    public void OnClickRestart()
    {
        if (string.IsNullOrEmpty(mainTitleSceneName))
        {
            Debug.LogError("[ResultController] mainTitleSceneName 이 설정되지 않았습니다.");
            return;
        }

        SceneManager.LoadScene(mainTitleSceneName);
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        Debug.Log("[ResultController] Quit 호출됨 (에디터에서는 종료 대신 플레이 중지)");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
