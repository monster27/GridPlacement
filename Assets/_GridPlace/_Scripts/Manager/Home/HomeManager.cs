using BestHTTP.JSON.LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    #region Variable

    public GridManager gridManager;
    public TouchManager touchManager;
    BuilderExcelData[] excelDataLst;
    private List<GameObject> touchObjLst;
    public Transform buildContainer;
    public List<BuilderCtrl> buildCtrlLst = new List<BuilderCtrl>();
    public ViewFollow3D viewFollow3D;
    public HomeStatus curStatus;
    public BuildTag curBuildTag;
    public BuildRotType curBuildRotType;
    public BuilderCtrl curBuilderCtrl;

    public string loadScenePath;

    #endregion

    #region Mono
    void Start()
    {
        Init();
        Messenger.AddListener<BuildTag>(MessengerEvents.OnBuildClick, OnBuildClick);
        Messenger.AddListener(MessengerEvents.OnBuildMoveClick, OnBuildMoveClick);
        Messenger.AddListener(MessengerEvents.OnBuildRemoveClick, OnBuildRemoveClick);
        Messenger.AddListener(MessengerEvents.OnBuildRotClick, OnBuildRotClick);
        Messenger.AddListener(MessengerEvents.OnBuildMakeSureClick, OnBuildMakeSureClick);
        Messenger.AddListener(MessengerEvents.OnBuildSaveClick, OnBuildSaveClick);
        Messenger.AddListener(MessengerEvents.OnBuildReloadClick, OnBuildReloadClick);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<BuildTag>(MessengerEvents.OnBuildClick, OnBuildClick);
        Messenger.RemoveListener(MessengerEvents.OnBuildMoveClick, OnBuildMoveClick);
        Messenger.RemoveListener(MessengerEvents.OnBuildRemoveClick, OnBuildRemoveClick);
        Messenger.RemoveListener(MessengerEvents.OnBuildRotClick, OnBuildRotClick);
        Messenger.RemoveListener(MessengerEvents.OnBuildMakeSureClick, OnBuildMakeSureClick);
        Messenger.RemoveListener(MessengerEvents.OnBuildSaveClick, OnBuildSaveClick);
        Messenger.RemoveListener(MessengerEvents.OnBuildReloadClick, OnBuildReloadClick);
    }

    private void Update() { CheckoutBuildObj(); }

    #endregion

    #region BuildData 场景数据相关
    #region SetBuildData 根据数据重新加载场景
    /// <summary>
    /// 根据数据重新加载场景
    /// </summary>
    /// <param name="data"></param>
    public void SetBuildData(string data)
    {
        List<BuilderInRoomData> buildData = JsonMapper.ToObject<List<BuilderInRoomData>>(data);

        foreach (var builderData in buildData)
        {
            BuilderCtrl ctrl = CreatObj(builderData.buildTag);
            List<Node> closedNodeLst = gridManager.GetNearbyNode(builderData.nodeIndexX, builderData.nodeIndexY, builderData.selectXSize, builderData.selectYSize);
            Node centerNode = gridManager.GetNode(builderData.nodeIndexX, builderData.nodeIndexY);
            ctrl.Init(builderData.buildTag, builderData.rotType);
            ctrl.InitNodeLst(centerNode, closedNodeLst, new Vector2(builderData.nodeIndexX, builderData.nodeIndexY));
            ctrl.SetNodeIsUsed(true);
            SetCtrlNode(ctrl, centerNode);
            buildCtrlLst.Add(ctrl);
        }
    }
    #endregion

    #region GetBuildData 获取场景数据
    /// <summary>
    /// 获取场景数据
    /// </summary>
    /// <returns></returns>
    public string GetBuildData()
    {
        List<BuilderInRoomData> builderData = new List<BuilderInRoomData>();
        foreach (var item in buildCtrlLst)
        {
            builderData.Add(item.GetBuildData());
        }
        return JsonMapper.ToJson(builderData);
    }
    #endregion

    #endregion

    #region  OnClick

    #region OnBuildClick 建造按钮按下,生成建筑物
    /// <summary>
    /// 建造按钮按下,生成建筑物
    /// </summary>
    /// <param name="tag"></param>
    public void OnBuildClick(BuildTag tag)
    {
        Init();
        PutBuild(tag);
    }
    #endregion

    #region OnBuildMakeSureClick 确定按钮按下
    /// <summary>
    /// 确定按钮按下
    /// </summary>
    void OnBuildMakeSureClick()
    {
        gridManager.PutDownBuild();
        curStatus = HomeStatus.None;
    }
    #endregion

    #region OnBuildRemoveClick 删除按钮按下
    /// <summary>
    /// 删除按钮按下
    /// </summary>
    void OnBuildRemoveClick()
    {
        if (curBuilderCtrl == null) return;
        PutDownBuild(false);
        curStatus = HomeStatus.None;
    }
    #endregion

    #region OnBuildMoveClick 移动按钮按下
    /// <summary>
    /// 移动按钮按下
    /// </summary>
    void OnBuildMoveClick()
    {
        curStatus = HomeStatus.Build;
    }
    #endregion

    #region OnBuildRotClick 旋转按钮按下
    /// <summary>
    /// 旋转按钮按下
    /// </summary>
    public void OnBuildRotClick()
    {
        int type = ((int)curBuildRotType) + 1;
        bool isExits = false;
        foreach (BuildRotType t in Enum.GetValues(typeof(BuildRotType))) { if (type <= (int)t) isExits = true; }
        if (!isExits) type = 0;
        curBuildRotType = (BuildRotType)type;
        UpdateGridData();
    }
    #endregion

    #region OnBuildSaveClick 保存按钮按下
    /// <summary>
    /// 保存按钮按下
    /// </summary>
    private void OnBuildSaveClick()
    {
        string str = GetBuildData();
        //if (!File.Exists(loadScenePath)) { File.Create(loadScenePath); }
        File.WriteAllText(loadScenePath, str);
    }
    #endregion

    #region OnBuildReloadClick 加载场景按钮按下
    /// <summary>
    /// 加载场景按钮按下
    /// </summary>
    private void OnBuildReloadClick()
    {
        string str = File.ReadAllText(loadScenePath);
        SetBuildData(str);
    }
    #endregion

    #endregion

    #region Init 初始化场景以及更新数据
    /// <summary>
    /// 初始化场景以及更新数据
    /// </summary>
    void Init()
    {
        if (curBuilderCtrl != null) { PutDownBuild(false); }
        curStatus = HomeStatus.None;
        curBuildTag = BuildTag.None;
        curBuildRotType = BuildRotType.None;
        curBuilderCtrl = null;
        gridManager.Init();
        SetBuilingLstRayCollider(true);
        if (touchObjLst == null) touchObjLst = touchManager.touchObjLst;
        if (excelDataLst == null) excelDataLst = Resources.Load<BuilderExcelDataLst>(ExcelConfig.resourcesBuildExcelDataAssetPath).datas;
        if (loadScenePath == null || loadScenePath == "") loadScenePath = Application.streamingAssetsPath + "/SceneData.json";
    }
    #endregion

    #region CheckoutBuildObj 在update时检测是否能按到建筑物上
    /// <summary>
    /// 在update时检测是否能按到建筑物上
    /// </summary>
    void CheckoutBuildObj()
    {
        if (curStatus == HomeStatus.None)
        {
            for (int i = 0; i < touchObjLst.Count; i++)
            {
                if (touchObjLst[i].tag == Tag.Building.ToString())
                {
                    BuilderCtrl ctrl = touchObjLst[i].transform.parent.GetComponent<BuilderCtrl>();

                    if (ctrl != null)
                    {
                        if (ctrl != curBuilderCtrl)
                        {
                            PutUp(ctrl);
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region SetBuilingLstRayCollider 设置建筑物是否可以被射线检测到,区分与射线检测到地块的功能
    /// <summary>
    /// 设置建筑物是否可以被射线检测到,区分与射线检测到地块的功能
    /// </summary>
    /// <param name="isColldier"></param>
    void SetBuilingLstRayCollider(bool isColldier)
    {
        for (int i = 0; i < buildCtrlLst.Count; i++) { buildCtrlLst[i].SetCollider(isColldier); }
    }
    #endregion

    #region PutBuild 将建筑物放置到场景当中
    /// <summary>
    /// 将建筑物放置到场景当中
    /// </summary>
    /// <param name="tag"></param>
    void PutBuild(BuildTag tag)
    {
        SetBuilingLstRayCollider(false);
        curBuildTag = tag;
        gridManager.PreBuildInit();
        curBuilderCtrl = CreatObj(curBuildTag);
        UpdateGridData();
        viewFollow3D.followObj = curBuilderCtrl.gameObject;
        curStatus = HomeStatus.Build;
    }
    #endregion

    #region PutUp 将建筑物抬起,可以重新摆放(玩家点击到建筑物时)
    /// <summary>
    /// 将建筑物抬起,可以重新摆放(玩家点击到建筑物时)
    /// </summary>
    /// <param name="ctrl"></param>
    public void PutUp(BuilderCtrl ctrl)
    {
        curBuilderCtrl = ctrl;
        curBuildTag = curBuilderCtrl.buildTag;
        curBuildRotType = curBuilderCtrl.rotType;
        curBuilderCtrl.SetNodeIsUsed(false);
        gridManager.PreBuildInit(curBuilderCtrl.centerNode);
        UpdateGridData();
        viewFollow3D.followObj = curBuilderCtrl.gameObject;
        curStatus = HomeStatus.Build;
        for (int i = buildCtrlLst.Count - 1; i >= 0; i--)
        {
            if (buildCtrlLst[i] == ctrl)
            {
                buildCtrlLst.Remove(ctrl);
            }
        }
    }
    #endregion

    #region PutDownBuild 将建筑物摆放到场景中
    /// <summary>
    /// 将建筑物摆放到场景中
    /// </summary>
    /// <param name="isPutDown"></param>
    public void PutDownBuild(bool isPutDown)
    {
        SetBuilingLstRayCollider(true);
        if (isPutDown)
        {
            Vector2 indexVec = gridManager.GetSelectIndexVec();
            curBuilderCtrl.InitNodeLst(gridManager.selectNode, gridManager.selectNodeLst, indexVec);
            buildCtrlLst.Add(curBuilderCtrl);
            curBuilderCtrl = null;
        }
        else
        {
            Destroy(curBuilderCtrl.gameObject);
            gridManager.Init();
            Init();
        }
    }
    #endregion

    #region UpdateGridData 更新当前选中建筑物的信息,以及更新地块管理器中的数据,设置该建筑物的站地块范围
    /// <summary>
    /// 更新当前选中建筑物的信息,以及更新地块管理器中的数据,设置该建筑物的站地块范围
    /// </summary>
    void UpdateGridData()
    {
        if (curBuilderCtrl == null) return;
        curBuilderCtrl.Init(curBuildTag, curBuildRotType);
        gridManager.m_selectXSize = curBuilderCtrl.selectXSize;
        gridManager.m_selectYSize = curBuilderCtrl.selectYSize;
    }
    #endregion

    #region CreatObj 创建建筑物
    /// <summary>
    /// 创建建筑物
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    BuilderCtrl CreatObj(BuildTag tag)
    {
        if (tag == BuildTag.None) return null;
        string prefabPath = "";
        BuilderExcelData excelData = null;
        foreach (var item in excelDataLst)
        {
            if (tag.ToString().Equals(item.buildTag))
            {
                excelData = item;
            }
        }
        if (excelData == null) { return null; }
        prefabPath = excelData.loadPath;
        GameObject obj = Resources.Load<GameObject>(prefabPath);
        BuilderCtrl ctrl = Instantiate(obj, buildContainer).GetComponent<BuilderCtrl>();
        ctrl.SetData(tag, excelData.selectNodeX, excelData.selectNodeY, excelData.selectNodeX_90, excelData.selectNodeY_90);
        ctrl.gameObject.SetActive(true);
        ctrl.transform.SetPositionAndRotation(buildContainer.transform.position, buildContainer.transform.rotation);
        return ctrl;
    }
    #endregion

    #region SetCurCtrlNode 设置建筑物的位置,由地块管理器控制
    /// <summary>
    /// 设置建筑物的位置,由地块管理器控制
    /// </summary>
    /// <param name="node"></param>
    public void SetCurCtrlNode(Node node)
    {
        if (curBuilderCtrl != null) { SetCtrlNode(curBuilderCtrl, node); }
    }
    #endregion

    #region SetCtrlNode 根据地块位置设置建筑物的位置
    /// <summary>
    ///根据地块位置设置建筑物的位置
    /// </summary>
    /// <param name="ctrl"></param>
    /// <param name="node"></param>
    public void SetCtrlNode(BuilderCtrl ctrl, Node node)
    {
        ctrl.transform.position = node.buildPos;
    }
    #endregion
}
