#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
//using Excel;
using System.Data;

#if  UNITY_EDITOR
public class ZpfEditorTool : Editor
{
    //===============================             Tool             ==============================================
    [MenuItem("CustomEditor/ZpfTool/切换物体显隐状态 &1")]
    public static void SetObjActive()
    {
        GameObject[] selectObjs = Selection.gameObjects;
        int objCtn = selectObjs.Length;
        string nameStr = "";
        for (int i = 0; i < objCtn; i++)
        {
            bool isAcitve = selectObjs[i].activeSelf;
            selectObjs[i].SetActive(!isAcitve);
            nameStr += selectObjs[i].name + "     ";
        }
        Debug.Log(string.Format("<color=#00009c>切换物体显隐状态!\n名称为:{0}</color>", nameStr));
    }

    [MenuItem("CustomEditor/ZpfTool/增加选中物体命名索引 &2")]
    public static void SetObjArrayName()
    {
        GameObject[] selectObjs = Selection.gameObjects;
        int objCtn = selectObjs.Length;
        string nameStr = "";
        for (int i = 0; i < objCtn; i++)
        {
            selectObjs[i].transform.name = selectObjs[i].transform.name + "_" + selectObjs[i].transform.GetSiblingIndex();
            nameStr += selectObjs[i].name + "     ";
        }
        Debug.Log(string.Format("<color=#00009c>增加选中物体命名索引!\n名称为:{0}</color>", nameStr));
    }

    [MenuItem("CustomEditor/ZpfTool/排列选中物体 &3")]
    public static void SetObjArrayTrans()
    {
        GameObject[] selectObjs = Selection.gameObjects;
        int objCtn = selectObjs.Length;
        string nameStr = "";
        for (int i = 0; i < objCtn; i++)
        {
            selectObjs[i].transform.localPosition = new Vector3(i * 2, 0, 0);
            selectObjs[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            selectObjs[i].transform.SetSiblingIndex(i);
            nameStr += selectObjs[i].name + "     ";
        }
        Debug.Log(string.Format("<color=#00009c>排列选中物体!\n名称为:{0}</color>", nameStr));
    }

    public enum RandomType
    {
        None = 0,
        UseNum = 1,
        UseLow = 2,
        UseUpp = 4,
        UseSpe = 8,
    }

    #region 冒泡排序从大到小 bubbleDown
    /// <summary>
    /// 冒泡排序从大到小
    /// </summary>
    /// <param name="Array"></param>
    /// <returns></returns>
    public static int[] bubbleDown(int[] Array)
    {
        for (int i = 0; i < Array.Length; i++)
        {
            for (int j = i + 1; j < Array.Length; j++)
            {
                if (Array[i] < Array[j])
                {
                    int temp = Array[i];
                    Array[i] = Array[j];
                    Array[j] = temp;
                }
            }
        }
        return Array;
    }
    #endregion

    #region GetRandomString 生成随机字符串 
    ///<summary>
    ///生成随机字符串 
    ///</summary>
    ///<param name="length">目标字符串的长度</param>
    ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
    ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
    ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
    ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
    ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
    ///<returns>指定长度的随机字符串</returns>
    public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
    {
        byte[] b = new byte[4];
        new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
        System.Random r = new System.Random(BitConverter.ToInt32(b, 0));
        string s = null, str = custom;
        if (useNum == true) { str += "0123456789"; }
        if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
        if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
        if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
        for (int i = 0; i < length; i++)
        {
            s += str.Substring(r.Next(0, str.Length - 1), 1);
        }
        return s;
    }
    #endregion

    public static void Log(string log)
    {
        Debug.Log("<color=#00009c>"+ log + "</color>");
    }
}

#endif
#endif