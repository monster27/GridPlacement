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

    #region BuildData �����������
    #region SetBuildData �����������¼��س���
    /// <summary>
    /// �����������¼��س���
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

    #region GetBuildData ��ȡ��������
    /// <summary>
    /// ��ȡ��������
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

    #region OnBuildClick ���찴ť����,���ɽ�����
    /// <summary>
    /// ���찴ť����,���ɽ�����
    /// </summary>
    /// <param name="tag"></param>
    public void OnBuildClick(BuildTag tag)
    {
        Init();
        PutBuild(tag);
    }
    #endregion

    #region OnBuildMakeSureClick ȷ����ť����
    /// <summary>
    /// ȷ����ť����
    /// </summary>
    void OnBuildMakeSureClick()
    {
        gridManager.PutDownBuild();
        curStatus = HomeStatus.None;
    }
    #endregion

    #region OnBuildRemoveClick ɾ����ť����
    /// <summary>
    /// ɾ����ť����
    /// </summary>
    void OnBuildRemoveClick()
    {
        if (curBuilderCtrl == null) return;
        PutDownBuild(false);
        curStatus = HomeStatus.None;
    }
    #endregion

    #region OnBuildMoveClick �ƶ���ť����
    /// <summary>
    /// �ƶ���ť����
    /// </summary>
    void OnBuildMoveClick()
    {
        curStatus = HomeStatus.Build;
    }
    #endregion

    #region OnBuildRotClick ��ת��ť����
    /// <summary>
    /// ��ת��ť����
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

    #region OnBuildSaveClick ���水ť����
    /// <summary>
    /// ���水ť����
    /// </summary>
    private void OnBuildSaveClick()
    {
        string str = GetBuildData();
        //if (!File.Exists(loadScenePath)) { File.Create(loadScenePath); }
        File.WriteAllText(loadScenePath, str);
    }
    #endregion

    #region OnBuildReloadClick ���س�����ť����
    /// <summary>
    /// ���س�����ť����
    /// </summary>
    private void OnBuildReloadClick()
    {
        string str = File.ReadAllText(loadScenePath);
        SetBuildData(str);
    }
    #endregion

    #endregion

    #region Init ��ʼ�������Լ���������
    /// <summary>
    /// ��ʼ�������Լ���������
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

    #region CheckoutBuildObj ��updateʱ����Ƿ��ܰ�����������
    /// <summary>
    /// ��updateʱ����Ƿ��ܰ�����������
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

    #region SetBuilingLstRayCollider ���ý������Ƿ���Ա����߼�⵽,���������߼�⵽�ؿ�Ĺ���
    /// <summary>
    /// ���ý������Ƿ���Ա����߼�⵽,���������߼�⵽�ؿ�Ĺ���
    /// </summary>
    /// <param name="isColldier"></param>
    void SetBuilingLstRayCollider(bool isColldier)
    {
        for (int i = 0; i < buildCtrlLst.Count; i++) { buildCtrlLst[i].SetCollider(isColldier); }
    }
    #endregion

    #region PutBuild ����������õ���������
    /// <summary>
    /// ����������õ���������
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

    #region PutUp ��������̧��,�������°ڷ�(��ҵ����������ʱ)
    /// <summary>
    /// ��������̧��,�������°ڷ�(��ҵ����������ʱ)
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

    #region PutDownBuild ��������ڷŵ�������
    /// <summary>
    /// ��������ڷŵ�������
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

    #region UpdateGridData ���µ�ǰѡ�н��������Ϣ,�Լ����µؿ�������е�����,���øý������վ�ؿ鷶Χ
    /// <summary>
    /// ���µ�ǰѡ�н��������Ϣ,�Լ����µؿ�������е�����,���øý������վ�ؿ鷶Χ
    /// </summary>
    void UpdateGridData()
    {
        if (curBuilderCtrl == null) return;
        curBuilderCtrl.Init(curBuildTag, curBuildRotType);
        gridManager.m_selectXSize = curBuilderCtrl.selectXSize;
        gridManager.m_selectYSize = curBuilderCtrl.selectYSize;
    }
    #endregion

    #region CreatObj ����������
    /// <summary>
    /// ����������
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

    #region SetCurCtrlNode ���ý������λ��,�ɵؿ����������
    /// <summary>
    /// ���ý������λ��,�ɵؿ����������
    /// </summary>
    /// <param name="node"></param>
    public void SetCurCtrlNode(Node node)
    {
        if (curBuilderCtrl != null) { SetCtrlNode(curBuilderCtrl, node); }
    }
    #endregion

    #region SetCtrlNode ���ݵؿ�λ�����ý������λ��
    /// <summary>
    ///���ݵؿ�λ�����ý������λ��
    /// </summary>
    /// <param name="ctrl"></param>
    /// <param name="node"></param>
    public void SetCtrlNode(BuilderCtrl ctrl, Node node)
    {
        ctrl.transform.position = node.buildPos;
    }
    #endregion
}
