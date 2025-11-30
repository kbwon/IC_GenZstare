using UnityEngine;

public enum TrackType { A, B }

public class FlowManager : MonoBehaviour
{
    public static FlowManager Instance { get; private set; }

    public string currentUserId;
    public bool isGuest;
    public TrackType currentTrack;
    public bool tutorialDone;

    //public SessionReport currentSessionReport;

    void Awake()
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
