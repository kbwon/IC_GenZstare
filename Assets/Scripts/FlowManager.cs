using UnityEngine;

public enum TrackType { A, B }

public class FlowManager : MonoBehaviour
{
    public static FlowManager Instance { get; private set; }

    [Header("User Info")]
    public string currentUserId = "";
    public bool isGuest = false;
    public TrackType currentTrack = TrackType.A;
    public bool tutorialDone = false;

    [Header("Session")]
    public SessionReport currentSessionReport;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
