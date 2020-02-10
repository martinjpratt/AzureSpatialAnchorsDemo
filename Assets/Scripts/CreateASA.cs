using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using System;
using System.Threading.Tasks;
using TMPro;

public class CreateASA : MonoBehaviour
{
    public TextMeshProUGUI feedback;
    CloudSpatialAnchor currentCloudAnchor;
    AnchorExchanger anchorExchanger = new AnchorExchanger();

    async void Start()
    {
        this.gameObject.AddComponent<CloudNativeAnchor>();
        anchorExchanger.WatchKeys("https://<YOUR_NAME>.azurewebsites.net/api/anchors");
        await Initialize();
    }

    public async Task Initialize()
    {
        await GetComponent<SpatialAnchorManager>().CreateSessionAsync();
        feedback.text = "Created Session";
        await GetComponent<SpatialAnchorManager>().StartSessionAsync();
        feedback.text = "Started Session";
        await SaveCurrentObjectAnchorToCloudAsync();

        long anchorNumber = -1;

        anchorNumber = (await anchorExchanger.StoreAnchorKey(currentCloudAnchor.Identifier));

        GetComponentInChildren<TextMeshPro>().text = anchorNumber.ToString();

        GetComponent<SpatialAnchorManager>().StopSession();
        feedback.text = "Stopped Session";

    }


    protected virtual async Task SaveCurrentObjectAnchorToCloudAsync()
    {
        CloudNativeAnchor nativeAnchor = this.GetComponent<CloudNativeAnchor>();
        nativeAnchor.SetPose(this.transform.position, this.transform.rotation);

        // If the cloud portion of the anchor hasn't been created yet, create it
        if (nativeAnchor.CloudAnchor == null) { nativeAnchor.NativeToCloud(); }
        
        CloudSpatialAnchor cloudAnchor = nativeAnchor.CloudAnchor;

        cloudAnchor.Expiration = DateTimeOffset.Now.AddDays(7);
        
        while (!GetComponent<SpatialAnchorManager>().IsReadyForCreate)
        {
            await Task.Delay(330);
            float createProgress = GetComponent<SpatialAnchorManager>().SessionStatus.RecommendedForCreateProgress;
            feedback.text = $"Move your device to capture more environment data: {createProgress:0%}";
        }

        Pose anchorPose = cloudAnchor.GetPose();
        feedback.text = "Anchor Position: " + anchorPose.position + " Rotation: " + anchorPose.rotation;

        
        try
        {
            // Actually save
            await GetComponent<SpatialAnchorManager>().CreateAnchorAsync(cloudAnchor);
            feedback.text = "Saved: " + cloudAnchor.Identifier;
            // Store
            currentCloudAnchor = cloudAnchor;

        //    // Success?
        //    success = currentCloudAnchor != null;

        //    if (success && !isErrorActive)
        //    {
        //        // Await override, which may perform additional tasks
        //        // such as storing the key in the AnchorExchanger
        //        await OnSaveCloudAnchorSuccessfulAsync();
        //    }
        //    else
        //    {
        //        OnSaveCloudAnchorFailed(new Exception("Failed to save, but no exception was thrown."));
        //    }
        }
        catch (Exception ex)
        {
            feedback.text = ex.ToString();
        //    OnSaveCloudAnchorFailed(ex);
        }
    }


//    protected override async Task OnSaveCloudAnchorSuccessfulAsync()
//#pragma warning restore CS1998

//    {
//        //await base.OnSaveCloudAnchorSuccessfulAsync();

//        long anchorNumber = -1;

//        localAnchorIds.Add(currentCloudAnchor.Identifier);


//        Pose anchorPose = Pose.identity;
//        anchorPose = currentCloudAnchor.GetPose();
        

        

//        //feedbackBox.text = $"Created anchor {anchorNumber}. Next: Stop cloud anchor session";
//    }
}