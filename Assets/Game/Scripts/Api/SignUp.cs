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

namespace Game.Scripts.Api
{
    public class SignUp: MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputName;
        [SerializeField] private TMP_InputField inputEmail;
        [SerializeField] private TMP_InputField inputPass;
        [SerializeField] private Button signUpBtt;
        [SerializeField] private Button signInPageBtt;

        

        

        private void OnEnable()
        {
            signUpBtt.onClick.AddListener(OnSignUp);
            signInPageBtt.onClick.AddListener(ChangePage);
        }
        private void OnDisable()
        {
            signUpBtt.onClick.RemoveListener(OnSignUp);
            signInPageBtt.onClick.RemoveListener(ChangePage);
        }
        
        private void ChangePage()
        {
            UIManager.ChangeScreen(UIManager.Screen.SignIn);
        }
        
        private void OnValidate()
        {
            if (inputName == null)
            {
                inputName = GameObject.Find("S Name field").GetComponent<TMP_InputField>();
            }
            if (inputEmail == null)
            {
                inputEmail = GameObject.Find("S Email field").GetComponent<TMP_InputField>();
            }
            if (inputPass == null)
            {
                inputPass = GameObject.Find("S Password field").GetComponent<TMP_InputField>();
            }
            if (signUpBtt == null)
            {
                signUpBtt = GameObject.Find("S Sign-Up Button").GetComponent<Button>();
            }
            if (signInPageBtt == null)
            {
                signInPageBtt = GameObject.Find("S Log-In Btt").GetComponent<Button>();
            }
        }
        
        private void OnSignUp()
        {
            SignUpRequest signuprequest = new SignUpRequest() {
                name = inputName.text,email = inputEmail.text, password = inputPass.text
            };
            StartCoroutine(SendSignUpRequest(global::Api.BaseUrl + global::Api.SignUpUrl, signuprequest));
        }
        private IEnumerator SendSignUpRequest(string url, SignUpRequest data)
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
                SignUpResponse signUpResponse =
                    JsonConvert.DeserializeObject<SignUpResponse>(request.downloadHandler.text);
                Print.CustomLog(signUpResponse.ResponseMessage, 15, LogColor.Red);
                Debug.Log($"Received OTP: {signUpResponse.ResponseData.otp}");
                UIManager.ChangeScreen(UIManager.Screen.Otp);

                Data.OTP = signUpResponse.ResponseData.otp;
                Data.tempToken = signUpResponse.ResponseData.temptoken;

            }
            else
            {
                Debug.LogError($"Error: {request.error}");
            }
        }
    }

    public class SignUpRequest
    {
        public string name;
        public string email;
        public string password;
    }

    public class SignUpResponse
    {
        public int ResponseCode;
        public string ResponseMessage;
        public bool success;
        public SignUpData ResponseData;
    }

    public class SignUpData
    {
        public string id;
        public string name;
        public string email;
        public string temptoken;
        public string permtoken;
        public int otp;
    }
}