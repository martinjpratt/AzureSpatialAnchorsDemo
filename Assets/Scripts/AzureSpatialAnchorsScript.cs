using Microsoft.Azure.SpatialAnchors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzureSpatialAnchorsScript : MonoBehaviour
{


    /// <summary>
    /// Use the recognizer to detect air taps.
    /// </summary>
    //private GestureRecognizer recognizer;

    protected CloudSpatialAnchorSession cloudSpatialAnchorSession;

    /// <summary>
    /// The CloudSpatialAnchor that we either 1) placed and are saving or 2) just located.
    /// </summary>
    protected CloudSpatialAnchor currentCloudAnchor;

    /// <summary>
    /// True if we are 1) creating + saving an anchor or 2) looking for an anchor.
    /// </summary>
    protected bool tapExecuted = false;


}
