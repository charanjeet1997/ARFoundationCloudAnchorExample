using System;
using System.Collections;
using System.Collections.Generic;
using Games.Services;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementManager : MonoBehaviour,IMouseDown
{
    public static ARPlacementManager Instance { get; private set; } 
    public ARPlaneManager planeManager;
    public ARRaycastManager raycastManager;
    public ARAnchorManager anchorManager;
    public float scaleFactor;
    private static List<ARRaycastHit> hits;
    [SerializeField]private GameObject placedObjectPrefab;
    private GameObject placedObject;
    [SerializeField] private Slider arObjectScaler;
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        ARCloudAnchorManager.onCreateCloudAnchor += ReCreatePlacement;
        TouchService.instance.Add(this);
    }

    private void OnDisable()
    {
        ARCloudAnchorManager.onCreateCloudAnchor -= ReCreatePlacement;
        TouchService.instance.Remove(this);
    }

    public void OnMouseDown(Vector3 position)
    {
        if (hits == null)
        {
            hits = new List<ARRaycastHit>();
        }

        if (raycastManager.Raycast(position, hits, TrackableType.All))
        {
            if (placedObject == null && !PointIsOverUI(position.x,position.y))
            {
                var hitPose = hits[0].pose;
                placedObject = Instantiate(placedObjectPrefab, hitPose.position, hitPose.rotation);
                placedObject.transform.localScale = Vector3.one * scaleFactor;
                var anchor = anchorManager.AttachAnchor(hits[0].trackable.GetComponent<ARPlane>(), hitPose);
                placedObject.transform.parent = anchor.transform;
                ARCloudAnchorManager.Instance.QueueAnchor(anchor);
            }
        }
    }

    public void ReCreatePlacement(Transform transform)
    {
        if (placedObject == null)
        {
            Debug.Log("recreate placement");
            placedObject = Instantiate(placedObjectPrefab, transform.position, transform.rotation);
            placedObject.transform.localScale = Vector3.one * scaleFactor;
            placedObject.transform.parent = transform;
        }
    }

    public void RemovePlacedObject()
    {
        Destroy(placedObject);
        placedObject = null;
    }

    public void ScaleArObject()
    {
        Debug.Log(arObjectScaler.value);
        scaleFactor = arObjectScaler.value;
        if(placedObject != null)
            placedObject.transform.localScale = Vector3.one * scaleFactor;
    }
    
    private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>();

    public bool PointIsOverUI(float x, float y)
    {
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = new Vector2(x, y);

        tempRaycastResults.Clear();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, tempRaycastResults);

        return tempRaycastResults.Count > 0;
    }
}