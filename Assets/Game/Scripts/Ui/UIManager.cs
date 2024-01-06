using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZxLog;
public class UIManager : MonoBehaviour
{

    public enum Screen
    {
        SignIn,
        Otp,
        Signup,
        Image,
        Profile,
    }
    [SerializeField] private GameObject[] doNotDestroy;
    [Space]
    public List<GameObject> singletons;
    public static UIManager instance;
    //public LoadingAnimator loading;
    [SerializeField] private List<ScreenPair> screens;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        foreach (var screen in singletons)
        {
            screen.SetActive(true);
            screen.SetActive(false);
        }
    }


    private void OnApplicationQuit()
    {
        //App quit called
        Debug.Log("App Quit Called");
    }

    private void Start()
    {
        ChangeScreen(Screen.SignIn);
    }

    private void OnApplicationPause(bool hasFocus)
    {
        if (!hasFocus)
        {
            PlayerPrefs.SetString("isLoggedIn", "no");
        }
    }

    [ContextMenu("Call Debugs")]
    private void DoSoemthing()
    {
        Print.Separator();
        Print.CustomLog("World", 12, LogColor.Cyan);
        Print.Separator(LogColor.Red);
        Print.BigWhiteLog("Hahaha");
    }

    public static void ChangeScreen(Screen screenName)
    {
        foreach (ScreenPair pair in instance.screens)
        {
            if (pair.screenType == screenName)
            {
                pair.AnimateIn();
            }
            else if (pair.screenObject.activeSelf)
            {
                pair.AnimateOut();
            }
        }
    }

    public static void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    // public static void ShowLoading(bool value)
    // {
    //     instance.loading.Show(value);
    // }

    [Serializable]
    public struct ScreenPair
    {
        public Screen screenType;
        public GameObject screenObject;
        public float transitionDuration;

        public void AnimateIn()
        {
            LeanTween.cancel(screenObject);
            screenObject.SetActive(true);
            // Start from transparent and fade in
            CanvasGroup canvasGroup = screenObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = screenObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            LeanTween.alphaCanvas(canvasGroup, 1, transitionDuration).setEase(LeanTweenType.easeInOutQuad);
        }

        public void AnimateOut()
        {
            GameObject pp = screenObject;
            LeanTween.cancel(screenObject);
            // Start from opaque and fade out
            CanvasGroup canvasGroup = screenObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = screenObject.AddComponent<CanvasGroup>();
            LeanTween.alphaCanvas(canvasGroup, 0, transitionDuration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() => pp.SetActive(false)); 
        }
    }
}
