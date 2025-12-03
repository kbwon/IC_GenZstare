using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class PressAnyVRButton : MonoBehaviour
{
    [Header("Next Scene Name")]
    [SerializeField]
    private string nextSceneName = "Room_Login";   // 여기다 이동할 씬 이름 적기

    [Header("Debug Options")]
    [SerializeField]
    private bool allowKeyboardForDebug = true;     // 에디터에서 키보드로도 넘길지 여부

    private readonly List<InputDevice> controllerDevices = new List<InputDevice>();
    private bool hasLoaded = false;

    private void Start()
    {
        // 처음에 한 번 컨트롤러 목록 채우기
        RefreshDevices();

        // 나중에 컨트롤러 연결/해제될 때도 리스트 갱신
        InputDevices.deviceConnected += OnDeviceConnected;
        InputDevices.deviceDisconnected += OnDeviceDisconnected;
    }

    private void OnDestroy()
    {
        InputDevices.deviceConnected -= OnDeviceConnected;
        InputDevices.deviceDisconnected -= OnDeviceDisconnected;
    }

    private void OnDeviceConnected(InputDevice device)
    {
        if ((device.characteristics & InputDeviceCharacteristics.Controller) != 0)
        {
            if (!controllerDevices.Contains(device))
                controllerDevices.Add(device);
        }
    }

    private void OnDeviceDisconnected(InputDevice device)
    {
        controllerDevices.Remove(device);
    }

    private void RefreshDevices()
    {
        controllerDevices.Clear();
        InputDevices.GetDevices(controllerDevices);
    }

    private void Update()
    {
        if (hasLoaded) return;

        // 1) 에디터/PC 테스트용: 아무 키나 눌러도 시작
        if (allowKeyboardForDebug && Input.anyKeyDown)
        {
            LoadNextScene();
            return;
        }

        // 2) VR 컨트롤러 버튼 감지
        foreach (var device in controllerDevices)
        {
            bool primaryButton, secondaryButton, triggerButton, gripButton;

            // ABXY (primary/secondary), 트리거, 그립 등 여러 버튼 중 하나라도 눌리면 시작
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButton) && primaryButton ||
                device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButton) && secondaryButton ||
                device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButton) && triggerButton ||
                device.TryGetFeatureValue(CommonUsages.gripButton, out gripButton) && gripButton)
            {
                LoadNextScene();
                break;
            }
        }
    }

    private void LoadNextScene()
    {
        if (hasLoaded) return;
        hasLoaded = true;

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("[PressAnyVRButtonToStart] nextSceneName이 설정되지 않았습니다.");
            return;
        }

        Debug.Log("[PressAnyVRButtonToStart] 입력 감지됨 → 씬 로드: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}
