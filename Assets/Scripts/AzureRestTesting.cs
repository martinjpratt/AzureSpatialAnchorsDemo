using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class AzureRestTesting : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        StartCoroutine(GetMetadata());
        //ListBlobsAnonymously();
	}

    //async void ListBlobsAnonymously()
    //{
    //    // Get a reference to a container that's available for anonymous access.
    //    CloudBlobContainer container = new CloudBlobContainer(
    //        new Uri(@"https://haringerverdiag.blob.core.windows.net/ios/geoxplorer-outcrop"));

    //    var blobs = await container.ListBlobsSegmentedAsync();

    //    // Note this is only possible when the container supports full public read access.
    //    // List blobs in the container.
    //    foreach (IListBlobItem blobItem in )
    //    {
    //        print(blobItem.Uri);
    //    }
    //}

    IEnumerator GetMetadata()
    { 
        DateTime now = DateTime.UtcNow;
        


        var request = WebRequest.CreateHttp("https://haringerverdiag.blob.core.windows.net/ios/geoxplorer-outcrop?restype=container&comp=list");
        request.Method = "GET";
        request.Credentials = CredentialCache.DefaultCredentials;
        request.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
        //request.Headers.Add("x-ms-version", "2017-07-29");
        //request.Headers.Add("Authorization", "qNTIyFtXLm6W9+LV5fkVDeazdHvQVYJI+MYsngLN8kuUrlu8iqlQfTw52fKhTMExE4c2kR/eMS1k988j7pp5Xg==");

        //httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(storageAccountName, storageAccountKey, now, httpRequestMessage);

        // Get the response.  
        WebResponse response = request.GetResponse();
        yield return response;
        print(((HttpWebResponse)response).StatusDescription);
        response.Close();
    }
}
