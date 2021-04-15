using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRoom
{
    public class SingletonBase<T> : MonoBehaviour where T:Component
    {

        protected static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
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
    }
}