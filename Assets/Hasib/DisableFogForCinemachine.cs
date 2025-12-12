using UnityEngine;

using Unity.Cinemachine;

[ExecuteAlways]
public class DisableFogForCinemachine : CinemachineExtension
{
    bool previousFog;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Finalize) return;

        var cam = vcam.GetComponent<Camera>();
        if (cam == null) return;

        // Save fog state only once per frame
        previousFog = RenderSettings.fog;

        // Disable fog for this shot
        RenderSettings.fog = false;

        // Re-enable fog after rendering
        // (Cinemachine calls Finalize after rendering)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            RenderSettings.fog = previousFog;
        };
#endif

    }
}