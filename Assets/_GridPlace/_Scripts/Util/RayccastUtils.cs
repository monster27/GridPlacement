using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class RayccastUtils
{

    /// <summary>
    /// 获取当前点击位置的UI数量
    /// </summary>
    /// <returns></returns>
    public static int GetUINum()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //监听
        eventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(eventData, raycastResults);
        //raycastResults.RemoveAll(p => p.gameObject.layer != 11 && p.gameObject.layer != 5);
        raycastResults.RemoveAll(p =>  p.gameObject.layer != 5);
        return raycastResults.Count;
    }
}