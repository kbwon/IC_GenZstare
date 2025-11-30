using UnityEngine;
using UnityEngine.SceneManagement;

public class VRTutorialController : MonoBehaviour
{
    [Header("Next Scene")]
    [SerializeField] private string checkSceneName = "Room_Check";

    private bool movedOnce = false;
    private bool pressedButtonOnce = false;

    private void Start()
    {

    }

    public void OnClickGoToCheck()
    {
        FlowManager.Instance.tutorialDone = true;
        SceneManager.LoadScene(checkSceneName);
    }
}
