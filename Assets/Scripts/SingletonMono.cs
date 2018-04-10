using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{


    protected static T s_Instance = null;


    public static T Instance
    {
        get
        {
            if (null == s_Instance)
            {
                //寻找是否存在当前单例类对象  
                GameObject go = GameObject.Find(typeof(T).Name);
                //不存在的话  
                if (go == null)
                {
                    //new一个并添加一个单例脚本  
                    go = new GameObject();
                    s_Instance = go.AddComponent<T>();
                }
                else
                {
                    if (go.GetComponent<T>() == null)
                    {
                        go.AddComponent<T>();
                    }
                    s_Instance = go.GetComponent<T>();
                }
                //在切换场景的时候不要释放这个对象  
                DontDestroyOnLoad(go);
            }
            return s_Instance;
        }
    }


}