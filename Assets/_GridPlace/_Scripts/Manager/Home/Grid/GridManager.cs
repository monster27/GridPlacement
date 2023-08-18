
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region Variable
    public HomeManager homeManager;
    public TouchManager touchManager;
    public int xCount;
    public int yCount;
    public float floorLength = 10;
    public Transform container;
    public GameObject prefab;
    private Node[,] nodes = null;
    public List<Node> selectNodeLst = new List<Node>();
    private List<Vector3> touchPosLst = new List<Vector3>();
    private List<GameObject> touchObjLst = new List<GameObject>();
    bool isCanBuild;
    public Node selectNode;
    [SerializeField]
    Operate operate;
    public int m_selectXSize = 0;
    public int m_selectYSize = 0;

    #endregion

    #region Mono

    void Start()
    {
        if (container == null) container = transform;
        if (prefab == null) prefab = Resources.Load<GameObject>("_GridPlace/Prefabs/Node");
        touchManager.touchEvent_Up.AddListener(TouchUp);
        touchPosLst = touchManager.touchPosLst;
        touchObjLst = touchManager.touchObjLst;
        InitGridPlane();
    }
    void Update()
    {
        UpdateSetSelectNode();
    }

    #endregion

    #region GetStatus ��ȡ�÷���Ĳ���״̬
    /// <summary>
    /// ��ȡ�÷���Ĳ���״̬
    /// </summary>
    /// <returns></returns>
    HomeStatus GetStatus() { return homeManager.curStatus; }
    #endregion

    #region SetStatus ���ø÷���Ĳ���״̬
    /// <summary>
    /// ���ø÷���Ĳ���״̬
    /// </summary>
    /// <returns></returns>
    void SetStatus(HomeStatus status) { homeManager.curStatus = status; }
    #endregion

    #region PreBuildInit �ڽ���ǰ��ʼ��
    /// <summary>
    /// �ڽ���ǰ��ʼ��
    /// </summary>
    public void PreBuildInit(Node selectNode = null)
    {
        operate = Operate.Select;
        if (selectNode == null)
        {
            SetSelect(nodes[0, 0]);
        }
        else
        {
            SetSelect(selectNode);
        }
    }
    #endregion

    #region InitGridPlane ���ɵؿ�
    /// <summary>
    /// ���ɵؿ�
    /// </summary>
    public void InitGridPlane()
    {
        nodes = new Node[xCount, yCount];
        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < yCount; j++)
            {
                Node node = Instantiate(prefab, container).GetComponent<Node>();
                node.indexVec.x = i;
                node.indexVec.y = j;
                node.gameObject.name = $"Node[{i},{j}]";
                node.transform.position = new Vector3(container.position.x + i * floorLength, container.position.y, container.position.z - j * floorLength);
                nodes[i, j] = node;
            }
        }
    }
    #endregion

    #region AllNodeNotSelect ���еؿ�����Ϊ����ѡ��״̬
    /// <summary>
    /// ���еؿ�����Ϊ����ѡ��״̬
    /// </summary>
    void AllNodeNotSelect()
    {
        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < yCount; j++)
            {
                Node node = nodes[i, j];
                node.SetNoSelect();
            }
        }
    }
    #endregion

    #region AccordTouchGetSelectNode ���ݴ������ҵ����Ӧ�ĵؿ�
    /// <summary>
    /// ���ݴ������ҵ����Ӧ�ĵؿ�
    /// </summary>
    /// <returns></returns>
    Node AccordTouchGetSelectNode()
    {
        if (touchObjLst == null || touchObjLst.Count <= 0) return null;
        for (int i = 0; i < touchObjLst.Count; i++)
        {
            if (touchObjLst[i].tag == Tag.Node.ToString())
            {
                return touchObjLst[i].GetComponent<Node>();
            }
        }
        return null;
    }
    #endregion

    #region UpdateSetSelectNode ���ݴ����ؿ�ı�ؿ�״̬
    /// <summary>
    /// ���ݴ����ؿ�ı�ؿ�״̬
    /// </summary>
    void UpdateSetSelectNode()
    {
        if (GetStatus() == HomeStatus.None)
        {
            AllNodeNotSelect();
        }
        else if (GetStatus() == HomeStatus.Build || GetStatus() == HomeStatus.SelectBuildingStatus)
        {
            if (GetStatus() == HomeStatus.Build)
            {
                Node node = AccordTouchGetSelectNode();
                if (node != null) selectNode = node;
            }
            if (selectNode == null || m_selectXSize == 0 || m_selectYSize == 0)
            {
                AllNodeNotSelect();
                return;
            }
            isCanBuild = false;
            selectNodeLst.Clear();
            AllNodeNotSelect();
            switch (operate)
            {
                case Operate.None:
                    break;
                case Operate.Select:
                    Vector2 indexVec = GetSelectIndexVec();
                    int nodeX = (int)indexVec.x, nodeY = (int)indexVec.y;
                    selectNodeLst = GetNearbyNode(nodeX, nodeY, m_selectXSize, m_selectYSize);
                    bool isXFree = false;
                    bool isYFree = false;
                    int xSize = m_selectXSize;
                    int ySize = m_selectYSize;
                    if ((xSize % 2) == 0) { if (nodeX - (xSize / 2) + 1 >= 0 && nodeX + (xSize / 2 + 1) <= xCount) isXFree = true; }
                    else { if (nodeX - (xSize / 2) >= 0 && nodeX + (xSize / 2) < xCount) isXFree = true; }
                    if ((ySize % 2) == 0) { if (nodeY - (ySize / 2) + 1 >= 0 && nodeY + (ySize / 2 + 1) <= yCount) isYFree = true; }
                    else { if (nodeY - (ySize / 2) >= 0 && nodeY + (ySize / 2) < yCount) isYFree = true; }
                    if (isXFree && isYFree) isCanBuild = true;
                    break;
                default:
                    break;
            }
            for (int i = 0; i < selectNodeLst.Count; i++)
            {
                if (selectNodeLst[i].GetIsUsed()) { isCanBuild = false; break; }
            }
            for (int i = 0; i < selectNodeLst.Count; i++)
            {
                if (isCanBuild) { selectNodeLst[i].SetSelect(); }
                else { selectNodeLst[i].SetSelectErrot(); }
            }
            homeManager.SetCurCtrlNode(selectNode);
        }
    }
    #endregion

    #region GetNode ����������ȡ�ؿ�
    /// <summary>
    /// ����������ȡ�ؿ�
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Node GetNode(int x, int y)
    {
        return nodes[x, y];
    }
    #endregion

    #region GetSelectIndexVec ��ȡ��ѡ�еؿ������ֵ
    /// <summary>
    /// ��ȡ��ѡ�еؿ������ֵ
    /// </summary>
    /// <returns></returns>
    public Vector2 GetSelectIndexVec()
    {
        Vector2 v = Vector2.zero;
        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < yCount; j++)
            {
                if (nodes[i, j] == selectNode) { v.x = i; v.y = j; }
            }
        }
        return v;
    }
    #endregion

    #region GetClosedNode ���ݸ�������ֵ�ͷ�Χ��ȡ���ý����︽���ĵؿ���Ϣ
    /// <summary>
    /// ���ݸ�������ֵ�ͷ�Χ��ȡ���ý����︽���ĵؿ���Ϣ
    /// </summary>
    /// <param name="nodeX"></param>
    /// <param name="nodeY"></param>
    /// <param name="selectXSize"></param>
    /// <param name="selectYSize"></param>
    /// <returns></returns>
    public List<Node> GetNearbyNode(int nodeX, int nodeY, int selectXSize, int selectYSize)
    {
        List<Node> lst = new List<Node>();
        List<int> xlst = AccordSizeGetIndex(nodeX, selectXSize, 0, xCount);
        List<int> ylst = AccordSizeGetIndex(nodeY, selectYSize, 0, yCount);
        for (int i = 0; i < xlst.Count; i++)
        {
            for (int j = 0; j < ylst.Count; j++)
            {
                lst.Add(nodes[xlst[i], ylst[j]]);
            }
        }
        return lst;
    }
    #endregion

    #region AccordSizeGetIndex ���ݳ�ʼ������ø��������ҷ�Χ,��ȡ����С������ȡ�����
    /// <summary>
    /// ���ݳ�ʼ������ø��������ҷ�Χ,��ȡ����С������ȡ�����
    /// </summary>
    /// <param name="initIndex"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    List<int> AccordSizeGetIndex(int initIndex, int size, int mainValue, int maxValue)
    {
        List<int> indexLst = new List<int>();
        if ((size % 2) == 0)//ż��
        {
            int left = initIndex;
            int right = initIndex;
            for (int i = 0; i < size / 2 + 1; i++)
            {
                if (i != 0)
                {
                    left -= 1;
                    right += 1;
                }
                if (left >= mainValue)
                {
                    if (!indexLst.Contains(left))
                    {
                        if (i != size / 2)
                        {
                            indexLst.Add(left);
                        }
                    }
                }
                if (right < maxValue)
                {
                    if (!indexLst.Contains(right))
                    {
                        indexLst.Add(right);
                    }
                }
            }
        }
        else
        {
            int left = initIndex;
            int right = initIndex;
            for (int i = 0; i <= size / 2; i++)
            {
                if (i != 0)
                {
                    left -= 1;
                    right += 1;
                }
                if (left >= mainValue)
                {
                    if (!indexLst.Contains(left))
                    {
                        indexLst.Add(left);
                    }
                }
                if (right < maxValue)
                {
                    if (!indexLst.Contains(right))
                    {
                        indexLst.Add(right);
                    }
                }
            }
        }

        return indexLst;
    }
    #endregion

    #region TouchUp ����̧��
    /// <summary>
    /// ����̧��
    /// </summary>
    void TouchUp(Vector2 upPos)
    {
        if (GetStatus() == HomeStatus.Build)
        {
            Messenger.Broadcast(MessengerEvents.GridFingerTouchUp);
            SetStatus(HomeStatus.SelectBuildingStatus);
        }
    }
    #endregion

    #region SetSelect ����selectNode
    /// <summary>
    /// ����selectNode
    /// </summary>
    /// <param name="node"></param>
    public void SetSelect(Node node)    {        selectNode = node;    }
    #endregion

    #region PutDownBuild ��������
    /// <summary>
    /// ��������
    /// </summary>
    public void PutDownBuild()
    {
        if (selectNode != null)
        {
            if (isCanBuild)
            {
                homeManager.PutDownBuild(true);
                foreach (var item in selectNodeLst)
                {
                    item.SetIsUesd(true);
                }
            }
            else
            {
                homeManager.PutDownBuild(false);
            }
        }
        Init();
    }
    #endregion

    #region Init ��ʼ������
    /// <summary>
    /// ��ʼ������
    /// </summary>
    public void Init()
    {
        m_selectXSize = 0;
        m_selectYSize = 0;
        selectNode = null;
        selectNodeLst.Clear();
        touchPosLst.Clear();
        operate = Operate.None;
        homeManager.curBuildTag = BuildTag.None;
        if (homeManager.curBuilderCtrl != null)
        {
            Destroy(homeManager.curBuilderCtrl.gameObject);
            homeManager.curBuilderCtrl = null;
        }
    }
    #endregion

    enum Operate
    {
        None,
        Select
    }
}
