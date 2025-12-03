using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class TitleUIController : MonoBehaviour
{
    [Header("Next Scene Settings")]
    [SerializeField]
    private string loginSceneName = "Room_Login";

    [Header("FlowManager (Optional)")]
    [Tooltip("처음 실행 시 FlowManager 프리팹을 한 번만 생성하고 싶다면 여기에 넣어주세요.")]
    [SerializeField]
    private FlowManager flowManagerPrefab;

    // XR 컨트롤러 장치 목록
    private readonly List<InputDevice> leftHandDevices = new List<InputDevice>();
    private readonly List<InputDevice> rightHandDevices = new List<InputDevice>();

    // 한 번 시작/종료가 결정되면 중복 실행 방지 플래그
    private bool hasDecided = false;

    private void Awake()
    {
        // FlowManager가 아직 없고, 프리팹이 세팅되어 있으면 한 번만 생성
        if (FlowManager.Instance == null && flowManagerPrefab != null)
        {
            Instantiate(flowManagerPrefab);
        }
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

    // --- 매 프레임 버튼 감지 ---

    private void Update()
    {
        if (hasDecided) return;

        // 오른손 컨트롤러 버튼 → 시작
        if (CheckAnyButtonPressed(rightHandDevices))
        {
            hasDecided = true;
            OnClickStart();
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

    // --- 기존 버튼용 함수 (VR에서도 그대로 재사용) ---

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
