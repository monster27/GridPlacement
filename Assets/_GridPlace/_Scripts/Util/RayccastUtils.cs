using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class RayccastUtils
{

    /// <summary>
    /// ��ȡ��ǰ���λ�õ�UI����
    /// </summary>
    /// <returns></returns>
    public static int GetUINum()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //����
        eventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(eventData, raycastResults);
        //raycastResults.RemoveAll(p => p.gameObject.layer != 11 && p.gameObject.layer != 5);
        raycastResults.RemoveAll(p =>  p.gameObject.layer != 5);
        return raycastResults.Count;
    }
}