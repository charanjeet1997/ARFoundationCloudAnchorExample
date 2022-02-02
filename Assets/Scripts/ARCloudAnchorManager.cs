using System.Collections;
using System.Collections.Generic;
using System;
using CommanTickManager;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCloudAnchorManager : MonoBehaviour,ITick
{
    public static ARCloudAnchorManager Instance { get; private set; } 
    [SerializeField] private Camera arCamera;
    [SerializeField] private float resolveAnchorTimeout;

    [SerializeField]private ARAnchorManager anchorManager;
    private ARAnchor pendingHostAnchor = null;
    private ARCloudAnchor cloudAnchor;
    private string anchorIDToResolve;
    private bool anchorHostInProgress;
    private bool anchorResolveInprogress;
    private float safeToAnchorResolve;
    public static event Action<Transform> onCreateCloudAnchor = delegate{};

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        ProcessingUpdate.Instance.Add(this);
    }

    private void Start()
    {
        if(!string.IsNullOrEmpty(PlayerPrefs.GetString("CloudAnchorID")))
        {
            anchorIDToResolve = PlayerPrefs.GetString("CloudAnchorID");
        }
    }

    private void OnDisable()
    {
        ProcessingUpdate.Instance.Remove(this);
    }

    private Pose GetCamerPose()
    {
        return new Pose(arCamera.transform.position,arCamera.transform.rotation);
    }

    #region CloudAnchorCycle
    
    public void QueueAnchor(ARAnchor anchor)
    {
        pendingHostAnchor = anchor;
    }

    public void HostAnchor()
    {
        ARDebugManager.Instance.LogInfo("Host call in progress");
        FeatureMapQuality quality = anchorManager.EstimateFeatureMapQualityForHosting(GetCamerPose());
        ARDebugManager.Instance.LogInfo($"Quality : {quality}");
        cloudAnchor = anchorManager.HostCloudAnchor(pendingHostAnchor,1);
        if (cloudAnchor == null)
        {
            ARDebugManager.Instance.LogInfo("Unable to host cloud manager");
        }
        else
        {
            anchorHostInProgress = true;
        }
    }

    public void Resolve()
    {
        ARDebugManager.Instance.LogInfo("Resolve call in progress");
        if (string.IsNullOrEmpty(anchorIDToResolve))
        {
            ARDebugManager.Instance.LogInfo("Anchor Id not found");  
        }

        cloudAnchor = anchorManager.ResolveCloudAnchorId(anchorIDToResolve);

        if (cloudAnchor == null)
        {
            ARDebugManager.Instance.LogInfo($"Unable to resolve cloud anchor: {anchorIDToResolve}");
        }
        else
        {
            anchorResolveInprogress = true;
        }
    }

    public void CheckHostingProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;

        switch (cloudAnchorState)
        {
            case CloudAnchorState.Success:
                ARDebugManager.Instance.LogInfo("Anchor Hosted Successfully");
                ARPlacementManager.Instance.RemovePlacedObject();
                anchorHostInProgress = false;
                anchorIDToResolve = cloudAnchor.cloudAnchorId;
                PlayerPrefs.SetString("CloudAnchorID",anchorIDToResolve);
                break;
            case CloudAnchorState.TaskInProgress:
                ARDebugManager.Instance.LogInfo($"{cloudAnchor.cloudAnchorState} ");
                break;
        }
        ARDebugManager.Instance.LogInfo($"{cloudAnchor.cloudAnchorState} ");
    }

    public void CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;
        switch (cloudAnchorState)
        {
            case CloudAnchorState.Success:
                ARDebugManager.Instance.LogInfo("Anchor Resolve Successfully");
                anchorResolveInprogress = false;
                onCreateCloudAnchor?.Invoke(cloudAnchor.transform);
                break;
            case CloudAnchorState.TaskInProgress:
                ARDebugManager.Instance.LogInfo($"{cloudAnchorState} ");
                break;
        }
        
    }
    #endregion

    public void Tick()
    {
        if (anchorHostInProgress)
        {
            CheckHostingProgress();
            return;
        }

        if (anchorResolveInprogress && safeToAnchorResolve <= 0)
        {
            safeToAnchorResolve = resolveAnchorTimeout;

            if (!string.IsNullOrEmpty(anchorIDToResolve))
            {
                ARDebugManager.Instance.LogInfo($"Error resolving anchor: {anchorIDToResolve}");
                CheckResolveProgress();
            }
        }
        else
        {
            safeToAnchorResolve -= Time.deltaTime * 1.0f;
        }
    }
}
