using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ControlRoom
{
    public class SingletonBase<T> : MonoBehaviour where T:Component
    {

      
        private static T _instance;
        private static bool hasApplicationQuited = false;
       
        public static T Instance
        {
            get
            {
                if (hasApplicationQuited)
                    return null;
                
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();//FindObjectOfType<T>();
                    string gameObjectName = typeof(T).ToString();

                    if (_instance == null)
                    {

                        GameObject obj = GameObject.Find(gameObjectName);
                    
                        if(obj==null)
                            obj= new GameObject(gameObjectName);
                    
                        _instance = obj.AddComponent<T>();
                    
                    }
                }
                return _instance;
            }
        }


        protected virtual void Awake()
        {
           
            //if(_instance==null)
            //_instance = this as T;
            
        }

        protected void OnApplicationQuit()
        {
            hasApplicationQuited = true;
        }
    }
}