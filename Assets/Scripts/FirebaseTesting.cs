using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using UnityEngine.Networking;

public class FirebaseTesting : MonoBehaviour
{
    /// <summary>
    /// Gets current stored anchor information, downloads, checks for expiration.
    /// On creating a new ID, checks for conflicting name and uploads to Firebase.
    /// </summary>


    //Private variables
    List<AzureSpatialAnchorObject> anchorObjects = new List<AzureSpatialAnchorObject>();
    bool conflictFound;

    //Public variables
    public string anchorName { get; set; } //this is set by the UI Input Field

    // Initial settings
    void Start()
    {
        conflictFound = false;
        StartCoroutine(FetchCurrentAnchors());
    }

    //Script to put (overwrite) the anchor selection on Firebase
    public void PutAnchors()
    {
        AzureSpatialAnchorObject anchorObject = new AzureSpatialAnchorObject();
        anchorObject.name = anchorName;
        anchorObject.identifier = Guid.NewGuid().ToString(); //This will be the ASA identifier
        anchorObject.dateCreated = DateTime.Now;
        anchorObject.dateExpired = DateTime.Now.AddDays(7); //this is the time set by the ASA expiration

        //Check if name has already been taken by a stored anchor
        foreach (var anchor in anchorObjects)
        {
            if (anchor.name == anchorObject.name)
            {
                conflictFound = true;
                print("Anchor name is already used, please pick another");
            }
        }

        //Upload the information to Firebase using PUT
        if (!conflictFound)
        {
            anchorObjects.Add(anchorObject);
            var json = JsonConvert.SerializeObject(anchorObjects);

            var request = WebRequest.CreateHttp("https://<YOURDATABASENAME>.firebaseio.com/anchors.json");
            request.Method = "PUT";
            request.ContentType = "application/json";
            var buffer = Encoding.UTF8.GetBytes(json);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
        }

        conflictFound = false;
    }

    //Finds anchor name in stored anchor list
    public void FindAnchorByName()
    {
        foreach (var anchor in anchorObjects)
        {
            if (anchor.name == anchorName)
            {
                print("Found " + anchor.name + ": " + anchor.identifier);
            }
        }
    }

    //Fetches the current list of anchor information on Firebase
    public IEnumerator FetchCurrentAnchors()
    {
        conflictFound = false;
        var uwr = UnityWebRequest.Get("https://<YOURDATABASENAME>.firebaseio.com/anchors.json");
        yield return uwr.SendWebRequest();

        //Continue if there are anchors stored, otherwise there's no point doing any more
        if(uwr.downloadHandler.text != "null")
        { 
            List<AzureSpatialAnchorObject> downloadedAnchors = JsonConvert.DeserializeObject<List<AzureSpatialAnchorObject>>(uwr.downloadHandler.text);
            print(downloadedAnchors.Count + "Anchors Stored on Firebase");
            foreach (var anchor in downloadedAnchors)
            {
                //Check if anchor has expired - if it has it's not added to the anchorObjects list and so when a new list is uploaded it won't be included
                if (anchor.dateExpired > DateTime.Now)
                {
                    anchorObjects.Add(anchor);
                }
            }
        }
    }

    //Anchor class
    public class AzureSpatialAnchorObject
    {
        public string name { get; set; }
        public string identifier { get; set; }
        public DateTime dateCreated { get; set; }
        public DateTime dateExpired { get; set; }
    }
}
