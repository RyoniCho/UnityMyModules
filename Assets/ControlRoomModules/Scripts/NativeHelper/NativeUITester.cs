using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using UnityEngine.UI;

public class NativeUITester : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;
    public TMPro.TMP_Text logText;
    public TMPro.TMP_Text paramText;
    public TMPro.TMP_InputField inputField;
    public Button invokeButton;

    private Dictionary<string, MethodInfo> dicMethodInfo = new Dictionary<string, MethodInfo>();
    private MethodInfo currentSelectedMethod;

    private void Start()
    {
        GetMethod(); 

        dropdown?.AddOptions(dicMethodInfo.Keys.ToList());
        dropdown?.onValueChanged?.AddListener(OnDropdownChanged);

        Debug.Log($"First Selected Item : {dropdown.captionText.text}");

        if(dicMethodInfo.TryGetValue(dropdown.captionText.text,out currentSelectedMethod))
        {
            Debug.Log("Current Method setted");
            SetParamInfo(currentSelectedMethod);
        }

        invokeButton?.onClick.AddListener(OnClickInvokeButton);
        
    }

    void GetMethod()
    {
        var type = typeof(NativeHelper);

        var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

        foreach(MethodInfo info in methods)
        {
           
            if(info.GetParameters().Length>0)
            {
            
                Debug.Log($"{info.Name}/ {string.Join(",",info.GetParameters().ToList().Select(x=>x.ParameterType))}/ {info.ReturnParameter}");
            }
            else
                Debug.Log($"{info.Name}/ {info.ReturnParameter}");

            dicMethodInfo.Add(info.Name, info);

        }    


    }

    void OnDropdownChanged(int index)
    {
        Debug.Log($"Dropdown select:{dicMethodInfo.Keys.ToArray()[index]}");

        if (dicMethodInfo.TryGetValue(dropdown.captionText.text, out currentSelectedMethod))
        {
            Debug.Log("OnDropdownChanged: Current Method setted");
            SetParamInfo(currentSelectedMethod);
        }
    }

    void SetParamInfo(MethodInfo info)
    {
        var tmpText = inputField.placeholder.GetComponent<TMPro.TMP_Text>();

        if (info.GetParameters().Length>0)
        {
            inputField.enabled = true;

            if(tmpText!=null)
            {
                tmpText.text = $"{info.GetParameters().Length} parameter is required";


            }

            if(paramText!=null)
                paramText.text = $"Param:{string.Join(",", info.GetParameters().ToList().Select(x => $"{x.Name}({x.ParameterType})"))}";


        }
        else
        {
           
            if(tmpText!=null)
            {
                tmpText.text = "No Parameter in this Method";
            }

            if (paramText != null)
                paramText.text = tmpText.text;

            inputField.enabled = false;
        }
    }

    void OnClickInvokeButton()
    {
        object returnValue = null;
        if (currentSelectedMethod != null)
        {
            if(currentSelectedMethod.GetParameters().Length<=0)
                returnValue = currentSelectedMethod.Invoke(null, null);
            else
            {
                var parameters= GetParameterValueFromInputField();

                if(parameters!=null)
                    returnValue = currentSelectedMethod.Invoke(null, parameters);
                else
                {
                    Debug.Log("Input Parameter Count not matched");

                }
            }
        }

        if(returnValue!=null)
        {
            if(logText!=null)
                logText.text = $"Return Value : {returnValue.ToString()}";
        }
           
    }

    object[] GetParameterValueFromInputField()
    {
        var inputText = inputField.text;

        var splitedText = inputText.Split(",");

        if (splitedText.Length != currentSelectedMethod.GetParameters().Length)
            return null;

        var parameters = currentSelectedMethod.GetParameters();

        List<object> objectList = new List<object>();
        try
        {
            for (int i = 0; i < splitedText.Length; i++)
            {

                if (Object.ReferenceEquals(typeof(bool), parameters[i].ParameterType))
                {
                    objectList.Add(System.Convert.ToBoolean(splitedText[i]));
                }

                else if (Object.ReferenceEquals(typeof(string), parameters[i].ParameterType))
                {
                    objectList.Add(splitedText[i]);
                }

                else if (Object.ReferenceEquals(typeof(int), parameters[i].ParameterType))
                {
                    objectList.Add(int.Parse(splitedText[i]));
                }

                else if (Object.ReferenceEquals(typeof(float), parameters[i].ParameterType))
                {
                    objectList.Add(float.Parse(splitedText[i]));
                }
                else
                {
                    objectList.Add(null);
                }
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError($"Parse Parameter Failed:{e}");

            return null;
        }
        

        return objectList.ToArray();

    }
}
