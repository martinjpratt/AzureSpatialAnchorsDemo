using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.XR.ARFoundation;
using Microsoft.Azure.SpatialAnchors.Unity;

public class RoomManager : MonoBehaviour, IMixedRealityPointerHandler
{
#if UNITY_WSA
    public TextMeshPro directionText;
#elif UNITY_IOS
    public TextMeshProUGUI directionText;
#endif
    //public LobbyManager lobbyManager;
    public GameObject createAnchorButton;
    public GameObject findAnchorButton;
    public GameObject continueButton;
    public GameObject inputTextBox;
    public GameObject startFindingAnchorButton;

    public GameObject anchorObject;
    public Pose anchorPose;
    public string anchorNumber { get; set; }

    ARRaycastManager raycastManager;
    GameObject newAnchorObject;

    bool isPlacing;
    bool anchorValid;


    public void ListenForClicks()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
    }

    public void StopListenForClicks()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
    }


    // Start is called before the first frame update
    void Start()
    {
#if UNITY_IOS
        raycastManager = FindObjectOfType<ARRaycastManager>();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlacing)
        {
#if UNITY_IOS
            var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            var hits = new List<ARRaycastHit>();
            raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);

            anchorValid = hits.Count > 0;
            if (anchorValid)
            {
                anchorPose = hits[0].pose;
                
                newAnchorObject.GetComponentInChildren<Renderer>().enabled = true;

                var cameraForward = Camera.current.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                anchorPose.rotation = Quaternion.LookRotation(cameraBearing);

                newAnchorObject.transform.SetPositionAndRotation(anchorPose.position, anchorPose.rotation);

                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    isPlacing = false;
                    newAnchorObject.GetComponentInChildren<Renderer>().material.color = Color.yellow;
                    newAnchorObject.GetComponent<SpatialAnchorManager>().enabled = true;
                    newAnchorObject.AddComponent<CreateASA>();
                    newAnchorObject.GetComponent<CreateASA>().feedback = directionText;

                    //set the table anchor instance to it's new position and rotation
                    //TableAnchor.instance.transform.SetPositionAndRotation(newAnchorObject.transform.position, newAnchorObject.transform.rotation);
                    //lobbyManager.OnAnchorSuccessful();
                }
            }
            else
            {
                newAnchorObject.GetComponentInChildren<Renderer>().enabled = false;
            }

            

#elif UNITY_WSA
            if (Camera.main.GetComponent<GazeProvider>().HitPosition.magnitude > 0)
            {
                ListenForClicks();

                anchorValid = true;
                newAnchorObject.GetComponentInChildren<Renderer>().enabled = true;

                var cameraForward = Camera.main.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                anchorPose.rotation = Quaternion.LookRotation(cameraBearing);
                anchorPose.position = Camera.main.GetComponent<GazeProvider>().HitPosition;

                newAnchorObject.transform.SetPositionAndRotation(anchorPose.position, anchorPose.rotation);
            }
            else
            {
                StopListenForClicks();
                anchorValid = false;
                newAnchorObject.GetComponentInChildren<Renderer>().enabled = false;
            }
#endif

        }
    }

    public void OnCreateSelected()
    {
        createAnchorButton.SetActive(false);
        findAnchorButton.SetActive(false);
        continueButton.SetActive(false);
        directionText.text = "Place anchor location...";
        isPlacing = true;
        anchorValid = false;

        newAnchorObject = Instantiate(anchorObject);
    }

    public void OnFindSelected()
    {
        createAnchorButton.SetActive(false);
        findAnchorButton.SetActive(false);
        continueButton.SetActive(false);

        directionText.text = "Enter anchor number:";
        inputTextBox.SetActive(true);
        startFindingAnchorButton.SetActive(true);
    }

    public void OnStartFindingSelected()
    {
        inputTextBox.SetActive(false);
        startFindingAnchorButton.SetActive(false);
        print("finding anchor #: " + anchorNumber);

        newAnchorObject = Instantiate(anchorObject);
        newAnchorObject.GetComponentInChildren<Renderer>().enabled = false;
        newAnchorObject.GetComponent<SpatialAnchorManager>().enabled = true;
        newAnchorObject.AddComponent<FindASA>();
        newAnchorObject.GetComponent<FindASA>().anchorNumber = anchorNumber;
        newAnchorObject.GetComponent<FindASA>().feedback = directionText;
    }

    public void OnContinueSelected()
    {
        createAnchorButton.SetActive(false);
        findAnchorButton.SetActive(false);
        continueButton.SetActive(false);
        directionText.text = "Place menu location...";
        isPlacing = true;
        anchorValid = false;

        newAnchorObject = Instantiate(anchorObject);
        
    }



    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {

    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {

    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {

    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
#if UNITY_WSA
        if (anchorValid)
        {
            isPlacing = false;

            newAnchorObject.GetComponentInChildren<Renderer>().material.color = Color.green;

            //set the table anchor instance to it's new position and rotation
            TableAnchor.instance.transform.SetPositionAndRotation(newAnchorObject.transform.position, newAnchorObject.transform.rotation);
            StopListenForClicks();
            lobbyManager.OnAnchorSuccessful();

        }
#endif
    }

}