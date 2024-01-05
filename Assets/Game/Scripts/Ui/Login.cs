using Newtonsoft.Json;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ZxLog;
public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField login;
    [SerializeField] private TMP_InputField pass;
    [SerializeField] private Button LoginBtt;

    private void OnEnable()
    {
        LoginBtt.onClick.AddListener(OnLogin);
    }
    private void OnDisable()
    {
        LoginBtt.onClick.RemoveListener(OnLogin);
    }

    private void OnValidate()
    {
        if (login == null)
        {
            login = GameObject.Find("Login Field").GetComponent<TMP_InputField>();
        }
        if (pass == null)
        {
            pass = GameObject.Find("Pass Field").GetComponent<TMP_InputField>();
        }
        if (LoginBtt == null)
        {
            LoginBtt = GameObject.Find("LoginBtt").GetComponent<Button>();
        }
    }
    private void OnLogin()
    {
        LoginRequest loginRequest = new LoginRequest {
            email = login.text, password = pass.text
        };
        StartCoroutine(LoginCoroutine(ServiceUrl.BaseUrl + ServiceUrl.LoginUrl, loginRequest));
    }

    IEnumerator LoginCoroutine(string url, LoginRequest data)
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
            LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);
            Print.CustomLog(loginResponse.ResponseMessage, 15, LogColor.Red);
            UIManager.ChangeScreen(UIManager.Screen.Home);
        }
        else
        {
            Print.CustomLog(request.error, 15, LogColor.Red);
        }
    }
}

public class LoginRequest
{
    public string email;
    public string password;
}

public class LoginResponse
{
    public int ResponseCode;
    public string ResponseMessage;
    public bool success;
    public MainData ResponseMainData;
}

public class MainData
{
    public string id;
    public string name;
    public string email;
    public string permtoken;
}
