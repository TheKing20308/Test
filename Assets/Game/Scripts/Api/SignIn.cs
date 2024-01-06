using System.Collections;
using System.Collections.Generic;
using System.Text;
using Game.Scripts.Api;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using ZxLog;

public class SignIn : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputEmail;
    [SerializeField] private TMP_InputField inputPass;
    [SerializeField] private Button signInBtt;
    [SerializeField] private Button signUpPageBtt;
    
    private void OnEnable()
    {
        signInBtt.onClick.AddListener(OnSignIn);
        signUpPageBtt.onClick.AddListener(ChangePage);
    }
    private void OnDisable()
    {
        signInBtt.onClick.RemoveListener(OnSignIn);
        signUpPageBtt.onClick.RemoveListener(ChangePage);
    }

    private void ChangePage()
    {
        UIManager.ChangeScreen(UIManager.Screen.Signup);
    }
    
    private void OnValidate()
    {
        if (inputEmail == null)
        {
            inputEmail = GameObject.Find("L Email field").GetComponent<TMP_InputField>();
        }
        if (inputPass == null)
        {
            inputPass = GameObject.Find("L Password field").GetComponent<TMP_InputField>();
        }
        if (signInBtt == null)
        {
            signInBtt = GameObject.Find("L Log-In Button").GetComponent<Button>();
        }
        if (signUpPageBtt == null)
        {
            signUpPageBtt = GameObject.Find("L Sign-Up Btt").GetComponent<Button>();
        }
    }
    
    private void OnSignIn()
    {
        SignInRequest signinRequest = new SignInRequest() {
            email = inputEmail.text, password = inputPass.text
        };
        StartCoroutine(SendSignInRequest(Api.BaseUrl + Api.SignInUrl, signinRequest));
    }
    
    private IEnumerator SendSignInRequest(string url, SignInRequest data)
    {
        string jsonRequest = JsonConvert.SerializeObject(data);
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            SignInResponse loginResponse = JsonConvert.DeserializeObject<SignInResponse>(request.downloadHandler.text);
            Print.CustomLog(loginResponse.ResponseMessage, 15, LogColor.Red);
            UIManager.ChangeScreen(UIManager.Screen.Profile);

            Data.permToken = loginResponse.ResponseData.permtoken;
        }
        else
        {
            Print.CustomLog(request.error, 15, LogColor.Red);
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
    public SignInData ResponseData;
}

public class SignInData
{
    public string id;
    public string name;
    public string email;
    public string permtoken;
}
