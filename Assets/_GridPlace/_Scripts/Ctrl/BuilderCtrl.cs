using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderCtrl : MonoBehaviour
{
    #region Variable

    public int selectXSize;
    public int selectYSize;
    public Node centerNode;
    public List<Node> nodeLst = new List<Node>();
    public BuildRotType rotType;
    public BuildTag buildTag;
    public Vector2 curIndexVec;

    public GameObject centerObj;
    public GameObject rightObj_90;
    public GameObject rightObj_180;
    public GameObject rightObj_270;
    public int selectCenterXNodeSize;
    public int selectCenterYNodeSize;

    public int selectCenterXNodeSize_Rot90;
    public int selectCenterYNodeSize_Rot90;

    #endregion

    #region BuilderInRoomData 获取到场景中该建筑的相关参数
    /// <summary>
    /// 获取到场景中该建筑的相关参数
    /// </summary>
    /// <returns></returns>
    public BuilderInRoomData GetBuildData() 
    {
        BuilderInRoomData data = new BuilderInRoomData();
        data.rotType = rotType;
        data.buildTag = buildTag;    
        data.selectYSize = selectYSize;
        data.selectXSize = selectXSize;
        data.nodeIndexX = (int)curIndexVec.x;
        data.nodeIndexY = (int)curIndexVec.y;
        return data;
    }
    #endregion

    #region SetData 设置该建筑物占地块的范围大小
    /// <summary>
    /// 设置该建筑物占地块的范围大小
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="centerXNodeSize"></param>
    /// <param name="centerYnNodeSize"></param>
    /// <param name="centerXNodeSize_90"></param>
    /// <param name="centerYnNodeSize_90"></param>
    public void SetData(BuildTag tag, int centerXNodeSize,int centerYnNodeSize, int centerXNodeSize_90, int centerYnNodeSize_90) 
    {
        buildTag = tag;
        selectCenterXNodeSize = centerXNodeSize;
        selectCenterYNodeSize = centerYnNodeSize;
        selectCenterXNodeSize_Rot90 = centerXNodeSize_90;
        selectCenterYNodeSize_Rot90 = centerYnNodeSize_90;
    }
    #endregion

    #region Init 设置tag参数,并根据旋转标签设置物体显示
    /// <summary>
    /// 设置tag参数,并根据旋转标签设置物体显示
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="type"></param>
    public void Init(BuildTag tag, BuildRotType type = BuildRotType.None)
    {
        buildTag = tag;
        SetRot(type);
    }
    #endregion

    #region SetCollider 设置该建筑物的碰撞体,用来检测射线,后续可能会同步跟人物动作碰撞等
    /// <summary>
    /// 设置该建筑物的碰撞体,用来检测射线,后续可能会同步跟人物动作碰撞等
    /// </summary>
    /// <param name="isShow"></param>
    public void SetCollider(bool isShow) 
    {
        centerObj.GetComponent<BoxCollider>().enabled = isShow;
        rightObj_90.GetComponent<BoxCollider>().enabled = isShow;
        rightObj_180.GetComponent<BoxCollider>().enabled = isShow;
        rightObj_270.GetComponent<BoxCollider>().enabled = isShow;
    }
    #endregion

    #region SetRot 根据Tag设置该旋转的物体显示
    /// <summary>
    /// 根据Tag设置该旋转的物体显示
    /// </summary>
    /// <param name="type"></param>
    void SetRot(BuildRotType type)
    {
        rotType = type;
        centerObj.SetActive(false);
        rightObj_90.SetActive(false);
        rightObj_180.SetActive(false);
        rightObj_270.SetActive(false);
        switch (rotType)
        {
            case BuildRotType.None:
                centerObj.SetActive(true);
                selectXSize = selectCenterXNodeSize;
                selectYSize = selectCenterYNodeSize;
                break;
            case BuildRotType.Right_90:
                rightObj_90.SetActive(true);
                selectXSize = selectCenterXNodeSize_Rot90;
                selectYSize = selectCenterYNodeSize_Rot90;
                break;
            case BuildRotType.Right_180:
                rightObj_180.SetActive(true);
                selectXSize = selectCenterXNodeSize;
                selectYSize = selectCenterYNodeSize;
                break;
            case BuildRotType.Right_270:
                rightObj_270.SetActive(true);
                selectXSize = selectCenterXNodeSize_Rot90;
                selectYSize = selectCenterYNodeSize_Rot90;
                break;
        }
    }
    #endregion

    #region SetNodeIsUsed 设置地块是否是使用状态
    /// <summary>
    /// 设置地块是否是使用状态
    /// </summary>
    /// <param name="isUsed"></param>
    public void SetNodeIsUsed(bool isUsed)
    {
        for (int i = 0; i < nodeLst.Count; i++)
        {
            nodeLst[i].SetIsUesd(isUsed);
        }
    }
    #endregion

    #region InitNodeLst 在建筑中储存地块信息
    /// <summary>
    /// 在建筑中储存地块信息
    /// </summary>
    /// <param name="center"></param>
    /// <param name="lst"></param>
    /// <param name="indexVec"></param>
    public void InitNodeLst(Node center,List<Node> lst,Vector2 indexVec)
    {
        //Debug.Log("lst count is " + lst.Count);
        curIndexVec = indexVec;
        centerNode = center;
        nodeLst.Clear();
        foreach (Node node in lst)
        {
            nodeLst.Add(node);
        }
    }
    #endregion
}

public enum BuildRotType
{
    None = 0,
    Right_90,
    Right_180,
    Right_270
}
