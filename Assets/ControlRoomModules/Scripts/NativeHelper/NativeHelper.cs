using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

public class NativeHelper
{

    private delegate void AlertCallback(int id);
    private delegate void BooleanCallback(int id, bool status);

    private static bool isShowingAlertPopup = false;
    private static AndroidJavaObject _activity;
    private static DateTime _activity_expireTime;

    public static AndroidJavaObject Activity
    {
        get
        {
            if (_activity == null ||_activity_expireTime < DateTime.Now )
            {
                _activity?.Dispose();
                _activity = getCurrentActivity();
            }
            return _activity;
        }
    }

    private static AndroidJavaObject getCurrentActivity()
    {
        using (var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            _activity_expireTime = DateTime.Now.AddSeconds(60);
            return jc.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }

#if UNITY_IOS
#region Common Function

    [DllImport("__Internal")]
    private static extern void showAlertPopup(int methodId,string title, string message, bool setCancelButton,AlertCallback okCallback, AlertCallback cancelCallback);
    [DllImport("__Internal")]
    private static extern void showToast(string message);
    
    [DllImport("__Internal")]
    private static extern void addObserverGCKeyboard(int methodId,BooleanCallback onBooleanResultCallback);
    
    [DllImport("__Internal")]
    private static extern void addObserverGCController(int methodId,BooleanCallback onBooleanResultCallback);
    
    [DllImport("__Internal")]
    private static extern void addObserverGCMouse(int methodId,BooleanCallback onBooleanResultCallback);
    
    [DllImport("__Internal")]
    private static extern bool isAnyExternalKeyboardConnected();
    
    [DllImport("__Internal")]
    private static extern bool isAnyExternalMouseConnected();

    [DllImport("__Internal")]
    private static extern bool isAnyExternalGamepadConnected();
    
    [DllImport("__Internal")]
    private static extern float getGCMouseScrollYAxisValue();
    
    [DllImport("__Internal")]
    private static extern float getGCMouseScrollXAxisValue();

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
    
    [DllImport("__Internal")]
    private static extern MousePosition GetMousePosition();

     #endregion
    
#else
    private static void showAlertPopup(int methodId,string title, string message, bool setCancelButton,AlertCallback okCallback, AlertCallback cancelCallback){}
    private static void showToast(string message){}
    private static  void addObserverGCKeyboard(int methodId,BooleanCallback onBooleanResultCallback){}
    private static  void addObserverGCController(int methodId,BooleanCallback onBooleanResultCallback){}
    private static  void addObserverGCMouse(int methodId,BooleanCallback onBooleanResultCallback){}
    private static  void requestMicrophonePermission(){}
    private static bool canShowRequestMicPermission() { return false;}
    private static bool checkCameraPermissionAllowed() { return false;}
    private static bool checkPhotoLibraryPermissionAllowed() { return false;}
    private static  bool isAnyExternalKeyboardConnected() {return false; }
    private static  bool isAnyExternalMouseConnected() { return false; }
    private static  bool isAnyExternalGamepadConnected() { return false; }
    private static  float getGCMouseScrollYAxisValue() { return 0f; }
    private static  float getGCMouseScrollXAxisValue() { return 0f; }
    private static  MousePosition GetMousePosition(){ return new MousePosition();}
    
#endif
    public class AndroidPluginCallback : AndroidJavaProxy
    {
        private System.Action okButtonCallback;
        private System.Action cancelButtonCallback;
        private System.Action<bool> onCallback;
        public AndroidPluginCallback() :base("com.controlroom.androidmodule.PluginCallback"){}

        public AndroidPluginCallback(System.Action<bool> onCallback) : base("com.controlroom.androidmodule.PluginCallback")
        {
            this.onCallback = onCallback;
        }
        
        public AndroidPluginCallback(System.Action okButtonCallback) : base("com.controlroom.androidmodule.PluginCallback")
        {
            this.okButtonCallback = okButtonCallback;
        }
        
        public AndroidPluginCallback(System.Action okButtonCallback, System.Action cancelButtonCallback) : base("com.controlroom.androidmodule.PluginCallback")
        {
            this.okButtonCallback = okButtonCallback;
            this.cancelButtonCallback = cancelButtonCallback;
        }

        public void OnDefaultCallback(bool success)
        {
            this.onCallback?.Invoke(success);
        }

        public void OnOkButtonTouched()
        {
            this.okButtonCallback?.Invoke();
        }
        
        public void OnCancelButtonTouched()
        {
            this.cancelButtonCallback?.Invoke();
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct MousePosition
    {
        public float x;
        public float y;
    }

    public class iOSPluginCallback
    {
        public enum MethodId : int
        {
            ShowAlertPopup,
            CheckKeyboardConnected,
            CheckGamepadConnected,
            CheckMouseConnected,
            
        }
     

        private static Dictionary<int, System.Action> dicOKCallback = new Dictionary<int, Action>();
        private static Dictionary<int, System.Action> dicCancelCallback = new Dictionary<int, Action>();
        private static Dictionary<int, System.Action<bool>> dicResultCallback = new Dictionary<int, Action<bool>>();

        public static void SetCallback(MethodId methodId, System.Action okCallback, System.Action cancelCallback,Action<bool> resultCallback=null)
        {
            if (okCallback != null)
                dicOKCallback[(int)methodId] = okCallback;

            if (cancelCallback != null)
                dicCancelCallback[(int)methodId] = cancelCallback;
            if (resultCallback != null)
                dicResultCallback[(int)methodId] = resultCallback;
        }

        [MonoPInvokeCallback(typeof(AlertCallback))]
        public static void OnOkCallbackReceived(int id)
        {
            if (dicOKCallback.TryGetValue(id, out var callback))
            {
                callback?.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(AlertCallback))]
        public static void OnCancelCallbackReceived(int id)
        {
            if (dicCancelCallback.TryGetValue(id, out var callback))
            {
                callback?.Invoke();
            }
        }

        [MonoPInvokeCallback(typeof(BooleanCallback))]
        public static void OnBooleanResultCallback(int id, bool result)
        {
            if (dicResultCallback.TryGetValue(id, out var callback))
            {
                callback?.Invoke(result);
            }
        }
        
    }


    public static void ShowAlertPopup(string title, string message,System.Action okCallback,System.Action cancelCallback=null)
    {

        if (isShowingAlertPopup)
            return;

        okCallback += () => { isShowingAlertPopup = false; };
        
        if(cancelCallback!=null)
            cancelCallback += () => { isShowingAlertPopup = false; };
        
        bool useCancelCallback = (cancelCallback != null);

        if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
        {
            var androidPluginCallback = new AndroidPluginCallback(okCallback,cancelCallback);
            Activity.Call("ShowAlertPopup", title, message, "ok", "cancel", useCancelCallback, androidPluginCallback);
        }

        if(Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
        {
            iOSPluginCallback.SetCallback(iOSPluginCallback.MethodId.ShowAlertPopup, okCallback, cancelCallback);
            showAlertPopup((int)iOSPluginCallback.MethodId.ShowAlertPopup,title, message, useCancelCallback, iOSPluginCallback.OnOkCallbackReceived, iOSPluginCallback.OnCancelCallbackReceived);
          
        }

        UnityEngine.Debug.Log($"NativeHelper Call ShowAlertPopup: title: {title}, message {message}");
        isShowingAlertPopup = true;

    }
    

    public static void ShowToast(string message)
    {
        if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
        {
            Activity.Call("ShowToast", message);
        }
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

    public static bool IsAnyExternalKeyboardConnected()
    {
        if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
        {
            return Activity.Call<bool>("IsAnyKeyboardDeviceConnected");
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
        {
            return isAnyExternalKeyboardConnected();
        }
        
        return false;
    }
    public static bool IsAnyExternalMouseConnected()
    {
        if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
        {
            return Activity.Call<bool>("IsAnyMouseDeviceConnected");
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
        {
            return isAnyExternalMouseConnected();
        }
        
        return false;
    }

    public static bool IsAnyExternalGamepadConnected()
    {
        if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
        {
            return Activity.Call<bool>("IsAnyGamepadDeviceConnected");
        }
        
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
        {
            return isAnyExternalGamepadConnected();
        }
        return false;
    }

    public static void CheckKeyboardConnected(System.Action<bool> resultCallback)
    {
        if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
        {
            var androidPluginCallback = new AndroidPluginCallback(resultCallback);
            Activity.Call("AddObserverPhysicalKeyboard", androidPluginCallback);
        }
        
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
        {
            iOSPluginCallback.SetCallback(iOSPluginCallback.MethodId.CheckKeyboardConnected, null, null,resultCallback);
            addObserverGCKeyboard((int)iOSPluginCallback.MethodId.CheckKeyboardConnected,iOSPluginCallback.OnBooleanResultCallback);
        }
    }
    public static void CheckGamepadConnected(System.Action<bool> resultCallback)
    {
        if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
        {
            var androidPluginCallback = new AndroidPluginCallback(resultCallback);
            Activity.Call("AddObserverGamepad", androidPluginCallback);
        }
        
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
        {
            iOSPluginCallback.SetCallback(iOSPluginCallback.MethodId.CheckGamepadConnected,null, null,resultCallback);
            addObserverGCController((int)iOSPluginCallback.MethodId.CheckGamepadConnected,iOSPluginCallback.OnBooleanResultCallback);
        }
    }
    
    public static void CheckMouseConnected(System.Action<bool> resultCallback)
    {
        if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
        {
            var androidPluginCallback = new AndroidPluginCallback(resultCallback);
            Activity.Call("AddObserverMouse", androidPluginCallback);
        }
        
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
        {
            iOSPluginCallback.SetCallback(iOSPluginCallback.MethodId.CheckMouseConnected, null, null,resultCallback);
            addObserverGCMouse((int)iOSPluginCallback.MethodId.CheckMouseConnected,iOSPluginCallback.OnBooleanResultCallback);
        }
    }

    public static float GetGCMouseScrollYAxisValue()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
        {
            return getGCMouseScrollYAxisValue();
        }

        return 0;

    }
    public static float GetGCMouseScrollXAxisValue()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
        {
            return getGCMouseScrollXAxisValue();
        }

        return 0;

    }

    public static Vector2 GetCurrentGCMousePosition()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer && !Application.isEditor)
        {
            var mousePosition = GetMousePosition();
            return new Vector2(mousePosition.x, mousePosition.y);
        }
        
        return Vector2.zero;
        
    }


}
