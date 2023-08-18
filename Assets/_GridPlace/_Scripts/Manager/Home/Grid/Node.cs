using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    Material mat;
    public Vector3 buildPos;
    public Vector2 indexVec;
    [SerializeField]
    private bool isUsed;
    //[SerializeField]
    //private bool isSelectCanBuild; //当选中状态是否可以放置，临时变量
    private void Start()
    {
        mat = GetComponent<Renderer>().material;
        buildPos = transform.position /*+ new Vector3(0, 0.5f, 0)*/;
        //isSelectCanBuild = true;
        isUsed = false;
    }
    public void SetSelect()
    {
        //SetIsSelectCanBuild(true);
        mat.color = Color.green;
    }
    public void SetSelectErrot()
    {
        //SetIsSelectCanBuild(false);
        mat.color = Color.red;
    }
    public void SetIsUesd(bool used) { isUsed = used; }
    //public void SetIsSelectCanBuild(bool build) { isSelectCanBuild = build; }
    public bool GetIsUsed() { return isUsed; }
    //public bool GetIsSelectCanBuild() 
    //{
    //    if (!isUsed)
    //    {
    //        return isSelectCanBuild;
    //    }
    //    return false;
    //}

    public void SetNoSelect()
    {
        //SetIsSelectCanBuild(true);
        if (mat.color != Color.gray)
        {
            mat.color = Color.gray;
        }
    }
}
