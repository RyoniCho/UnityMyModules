using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

public class NativeHelper
{

    private delegate void AlertCallback();

    #region Common Function

    [DllImport("__Internal")]
    private static extern void showAlertPopup(string title, string message, bool setCancelButton,AlertCallback okCallback, AlertCallback cancelCallback);
    [DllImport("__Internal")]
    private static extern void showToast(string message);

    #endregion

    #region Request Permission

    [DllImport("__Internal")]
    private static extern void requestMicrophonePermission();

    #endregion

    #region Check Permission

    [DllImport("__Internal")]
    private static extern bool canShowRequestMicPermission();
    [DllImport("__Internal")]
    private static extern bool checkCameraPermissionAllowed();
    [DllImport("__Internal")]
    private static extern bool checkPhotoLibraryPermissionAllowed();

    #endregion

    public class iOSPluginCallback
    {
        private static System.Action okActionCallback;
        private static System.Action cancelActionCallback;

        public static void SetCallback(System.Action okCallback, System.Action cancelCallback)
        {
            okActionCallback = okCallback;
            cancelActionCallback = cancelCallback;
        }

        [MonoPInvokeCallback(typeof(AlertCallback))]
        public static void OnOkCallbackReceived()
        {
            if (okActionCallback != null)
                okActionCallback();
        }

        [MonoPInvokeCallback(typeof(AlertCallback))]
        public static void OnCancelCallbackReceived()
        {
            if (cancelActionCallback != null)
                cancelActionCallback();
        }
    }


    public static void ShowAlertPopup(string title, string message,System.Action okCallback,System.Action cancelCallback=null)
    {

        bool useCancelCallback = (cancelCallback != null);

        iOSPluginCallback.SetCallback(okCallback, cancelCallback);

        if(Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
        {
            showAlertPopup(title, message, useCancelCallback, iOSPluginCallback.OnOkCallbackReceived, iOSPluginCallback.OnCancelCallbackReceived);

        }

        UnityEngine.Debug.Log($"NativeHelper Call ShowAlertPopup: title: {title}, message {message}");

    }

    public static void ShowToast(string message)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
            showToast(message);

        UnityEngine.Debug.Log($"NativeHelper Call ShowToast: message {message}");
    }

    public static void RequestMicrophonePermission()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
            requestMicrophonePermission();

        UnityEngine.Debug.Log("NativeHelper Call RequestMicrophonePermission");
    }

    public static bool CanShowRequestMicPermission()
    {
        
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
            return canShowRequestMicPermission();

        UnityEngine.Debug.Log("NativeHelper Call CanShowRequestMicPermission");

        return false;
    }

    public static bool CheckCameraPermissionAllowed()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
            return checkCameraPermissionAllowed();

        UnityEngine.Debug.Log("NativeHelper Call CheckCameraPermissionAllowed");

        return false;
    }
  
    public static bool CheckPhotoLibraryPermissionAllowed()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
            return checkPhotoLibraryPermissionAllowed();

        UnityEngine.Debug.Log("NativeHelper Call CheckCameraPermissionAllowed");

        return false;
    }

    public static void Test(bool testBool, int intTest,float floatTest)
    {
        UnityEngine.Debug.Log($"NativeHelper Call Test {testBool}/{intTest}/{floatTest}");
    }


}
