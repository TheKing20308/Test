using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class SignUp : MonoBehaviour
{
    private void OnClick()
    {
        StartCoroutine(SendSignInRequest());
    }

    private IEnumerator SendSignInRequest()
    {
        string apiUrl = (Api.BaseUrl + Api.SignInUrl);

        SignInRequest signInRequest = new SignInRequest
        {
            email = "example@email.com",
            password = "yourpassword"
        };

        string jsonRequest = JsonConvert.SerializeObject(signInRequest);

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                SignInResponse signInResponse = JsonConvert.DeserializeObject<SignInResponse>(jsonResponse);

                Debug.Log($"Response Code: {signInResponse.ResponseCode}");
                Debug.Log($"Response Message: {signInResponse.ResponseMessage}");
                Debug.Log($"Success: {signInResponse.success}");

                if (signInResponse.success)
                {
                    SignInData signInData = JsonConvert.DeserializeObject<SignInData>(jsonResponse);
                    Debug.Log($"ID: {signInData.id}");
                    Debug.Log($"Name: {signInData.name}");
                    Debug.Log($"Email: {signInData.email}");
                    Debug.Log($"Permission Token: {signInData.permtoken}");
                }
            }
            else
            {
                Debug.LogError($"Request failed with error: {request.error}");
            }
        } 
    }
}

public class SignInRequest
{
    public string email;
    public string password;
}

public class SignInResponse
{
    public int ResponseCode;
    public string ResponseMessage;
    public bool success;
}

public class SignInData
{
    public string id;
    public string name;
    public string email;
    public string permtoken;
}
