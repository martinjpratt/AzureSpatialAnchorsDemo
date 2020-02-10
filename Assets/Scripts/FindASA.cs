using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using TMPro;
using UnityEngine;

public class FindASA : MonoBehaviour
{
    public TextMeshProUGUI feedback;
    CloudSpatialAnchor currentCloudAnchor;
    AnchorExchanger anchorExchanger = new AnchorExchanger();
    public string anchorNumber;
    protected AnchorLocateCriteria anchorLocateCriteria = null;
    CloudSpatialAnchorWatcher currentWatcher;

    // Start is called before the first frame update
    async void Start()
    {
        anchorExchanger.WatchKeys("https://<YOUR_NAME>.azurewebsites.net/api/anchors");

        GetComponent<SpatialAnchorManager>().AnchorLocated += CloudAnchor_Located;
        anchorLocateCriteria = new AnchorLocateCriteria();

        await Initialize();
        
    }

    public async Task Initialize()
    {
        await GetComponent<SpatialAnchorManager>().CreateSessionAsync();
        feedback.text = "Created Session";
        await GetComponent<SpatialAnchorManager>().StartSessionAsync();
        feedback.text = "Started Session";

        string _anchorKeyToFind = await anchorExchanger.RetrieveAnchorKey(long.Parse(anchorNumber));
        if (_anchorKeyToFind == null)
        {
            feedback.text = "Anchor Number Not Found!";
        }

        List<string> anchorsToFind = new List<string>();
        List<string> anchorIdsToLocate = new List<string>();
        anchorsToFind.Add(_anchorKeyToFind);
        anchorIdsToLocate.AddRange(anchorsToFind);

        anchorLocateCriteria.Identifiers = new string[0];
        anchorLocateCriteria.Identifiers = anchorIdsToLocate.ToArray();

        feedback.text = "Anchor key to find: " + _anchorKeyToFind;

        GetComponent<SpatialAnchorManager>().Session.CreateWatcher(anchorLocateCriteria);
        feedback.text = "Watcher started...";

    }

    protected CloudSpatialAnchorWatcher CreateWatcher()
    {
        if ((GetComponent<SpatialAnchorManager>() != null) && (GetComponent<SpatialAnchorManager>().Session != null))
        {
            return GetComponent<SpatialAnchorManager>().Session.CreateWatcher(anchorLocateCriteria);
        }
        else
        {
            return null;
        }
    }


    private void CloudAnchor_Located(object sender, AnchorLocatedEventArgs args)
    {

        feedback.text = "Anchor " + anchorNumber + " located";
        currentCloudAnchor = args.Anchor;
        Pose anchorPose = currentCloudAnchor.GetPose();
        feedback.text = "Anchor position: " + anchorPose.position;
        this.transform.SetPositionAndRotation(anchorPose.position,anchorPose.rotation);
        this.GetComponentInChildren<Renderer>().enabled = true;
        this.GetComponentInChildren<Renderer>().material.color = Color.green;
        this.GetComponentInChildren<TextMeshPro>().text = anchorNumber;



        GetComponent<SpatialAnchorManager>().StopSession();
        feedback.text = "Stopped Session";
    }
}