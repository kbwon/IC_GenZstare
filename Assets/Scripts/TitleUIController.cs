using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TitleUIController : MonoBehaviour
{
    [Header("FlowManager (Optional)")]
    [SerializeField]
    private FlowManager flowManagerPrefab;

    [Header("Input Delay")]
    [Tooltip("씬이 로드된 후 이 시간(초) 동안은 XR 입력을 무시합니다.")]
    [SerializeField]
    private float inputDelaySeconds = 0.5f;

    [Header("Title Flow Controller")]
    [SerializeField]
    private TitleFlowController titleFlowController;

    // XR 컨트롤러 장치 목록
    private readonly List<InputDevice> leftHandDevices = new List<InputDevice>();
    private readonly List<InputDevice> rightHandDevices = new List<InputDevice>();

    private float elapsedSinceLoad = 0f;

    // 바로 전 프레임에 버튼이 눌려 있었는지 여부 (엣지 검출용)
    private bool lastRightPressed = false;
    private bool lastLeftPressed = false;

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
        elapsedSinceLoad = 0f;

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
        if (titleFlowController == null) return;

        elapsedSinceLoad += Time.unscaledDeltaTime;
        if (elapsedSinceLoad < inputDelaySeconds)
            return;

        // 현재 프레임에서 버튼이 눌려 있는지
        bool rightNow = IsAnyButtonPressed(rightHandDevices);
        bool leftNow = IsAnyButtonPressed(leftHandDevices);

        // "false → true"로 바뀐 순간만 처리 (Button Down)
        if (rightNow && !lastRightPressed)
        {
            // 오른손: 현재 Stage의 "진행" 액션
            titleFlowController.OnRightAction();
        }

        if (leftNow && !lastLeftPressed)
        {
            // 왼손: 현재 Stage의 "뒤로/종료" 액션
            titleFlowController.OnLeftAction();
        }

        // 다음 프레임 비교를 위해 상태 저장
        lastRightPressed = rightNow;
        lastLeftPressed = leftNow;
    }

    /// <summary>
    /// 주어진 컨트롤러 리스트에서
    /// primary / secondary / trigger / grip 중 아무 버튼이나 눌려 있는지
    /// (현재 프레임 기준)
    /// </summary>
    private bool IsAnyButtonPressed(List<InputDevice> devices)
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
}
