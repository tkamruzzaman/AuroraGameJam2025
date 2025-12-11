using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;
using UnityEngine.Timeline;

public class InitialTimeline : MonoBehaviour
{
    public Player playerScript;
    public GameObject rotatePoint;
    public PlayableDirector playableDirectorController;
    public CinemachineCamera cinemachineCamera;

    TimelineAsset timeline;
    bool hasEnded = false;

    void Awake()
    {
        playableDirectorController.stopped += OnTimelineStopped;
        SwitchOnOffObjects(false);
        playableDirectorController.time = 0;
        playableDirectorController.Play();
        timeline = playableDirectorController.playableAsset as TimelineAsset;
    }

    void SwitchOnOffObjects(bool state)
    {
        rotatePoint.SetActive(state);
        playerScript.enabled = state;
    }

    void Update()
    {
        // Debug.Log("Time: " + playableDirectorController.time + 
        //   " / Duration: " + playableDirectorController.playableAsset.duration);

        // if (!hasEnded && playableDirectorController.time >= playableDirectorController.playableAsset.duration)
        // {
        //     hasEnded = true;

        //     SwitchOnOffObjects(true);
        //     cinemachineCamera.Follow = playerScript.transform;

        //     print("TimelineEnded");
        // }
    }
    void OnTimelineStopped(PlayableDirector d)
{
    print("TimelineEnded");
    SwitchOnOffObjects(true);
    cinemachineCamera.Follow = playerScript.transform;
    this.gameObject.SetActive(false);
     print("TimelineEnded");
}
}
