using UnityEngine;
using UnityEngine.SceneManagement;

public class BathroomCheckController : MonoBehaviour
{
    [Header("Track Scene Names")]
    [SerializeField] private string trackASceneName = "TrackA";
    [SerializeField] private string trackBSceneName = "TrackB";

    public void OnClickStartTrack()
    {
        var app = FlowManager.Instance;
        if (app == null)
        {
            Debug.LogError("[BathroomCheckController] AppFlowManager 가 없습니다.");
            return;
        }

        string nextScene;
        if (app.currentTrack == TrackType.A)
            nextScene = trackASceneName;
        else
            nextScene = trackBSceneName;

        SceneManager.LoadScene(nextScene);
    }
}
