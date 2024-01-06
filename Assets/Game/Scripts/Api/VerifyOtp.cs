using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using ZxLog;

public class VerifyOtp : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputOtp;
    [SerializeField] private Button VerifyBtt;
    [SerializeField] private GameObject wrongtext;

    private void Start()
    {
        wrongtext.SetActive(false);
    }

    private void OnEnable()
    {
        VerifyBtt.onClick.AddListener(OnVerify);
    }
    private void OnDisable()
    {
        VerifyBtt.onClick.RemoveListener(OnVerify);
    }
    
    private void OnValidate()
    {
        if (inputOtp == null)
        {
            inputOtp = GameObject.Find("OTP field").GetComponent<TMP_InputField>();
        }
        if (VerifyBtt == null)
        {
            VerifyBtt = GameObject.Find("Verify Button").GetComponent<Button>();
        }
        
        if (wrongtext == null)
        {
            wrongtext = GameObject.Find("Wrong Otp Text").GetComponent<GameObject>();
        }
    }
    
    private void OnVerify()
    {
        VerifyOtpRequest verifyotpRequest = new VerifyOtpRequest() {
            authorization = ("Bearer " + Data.tempToken),otp = inputOtp.text
        };
        StartCoroutine(SendVerifyOtpRequest(Api.BaseUrl + Api.VerifyOtpUrl, verifyotpRequest));
    }
    
    private IEnumerator SendVerifyOtpRequest(string url, VerifyOtpRequest data)
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
            VerifyOtpResponse otpVerificationResponse = JsonConvert.DeserializeObject<VerifyOtpResponse>(request.downloadHandler.text);

            if (otpVerificationResponse.success == true)
            {
                UIManager.ChangeScreen(UIManager.Screen.Profile);
            }
            else
            {
                wrongtext.SetActive(true);
            }
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
        }
    }
}

public class VerifyOtpRequest
{
    public string authorization;
    public string otp;
}

public class VerifyOtpResponse
{
    public int ResponseCode;
    public string ResponseMessage;
    public bool success;
}


