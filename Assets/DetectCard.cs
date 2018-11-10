using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class DetectCard : MonoBehaviour, ITrackableEventHandler{
    protected TrackableBehaviour mTrackableNuke;

    protected virtual void Start()
    {
        mTrackableNuke = GetComponent<TrackableBehaviour>();
        if (mTrackableNuke)
            mTrackableNuke.RegisterTrackableEventHandler(this);
    }

    protected virtual void OnDestroy()
    {
        if (mTrackableNuke)
            mTrackableNuke.UnregisterTrackableEventHandler(this);
    }
	
	// Update is called once per frame
	void Update () {
	}


    // Use this for initialization
    //void Start()
    //{

    //}

    public void OnTrackableStateChanged(
      TrackableBehaviour.Status previousStatus,
      TrackableBehaviour.Status newStatus)
    {
     
    }


    protected virtual void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;
    }


    protected virtual void OnTrackingLost()
    {

    }
}
