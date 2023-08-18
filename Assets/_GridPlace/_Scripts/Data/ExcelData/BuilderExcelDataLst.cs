using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderExcelDataLst : ScriptableObject
{
    public BuilderExcelData[] datas;
}



[System.Serializable]
public class BuilderExcelData
{ 
    public string id;
    public string loadPath;
    public string dataName;
    public int selectNodeX;
    public int selectNodeY;
    public int selectNodeX_90;
    public int selectNodeY_90;
    public string buildTag;
}