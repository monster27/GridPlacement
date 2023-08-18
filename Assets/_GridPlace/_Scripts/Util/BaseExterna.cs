using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class BaseExterna
{
    public static T GetAndGetComponent<T>(this GameObject obj) where T : MonoBehaviour
    {
        if (obj.GetComponent<T>() == null) { obj.AddComponent<T>(); }
        return obj.GetComponent<T>();
    }
}
