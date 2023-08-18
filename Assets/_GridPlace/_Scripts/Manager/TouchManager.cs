using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Touch的事件，只有判断单指
/// </summary>
public class TouchManager : MonoBehaviour
{
    public FingerTouchEvent_Up touchEvent_Up = new FingerTouchEvent_Up();
    public FingerTouchEvent_Touch touchEvent = new FingerTouchEvent_Touch();
    public List<Vector3> touchPosLst = new List<Vector3>();
    public List<GameObject> touchObjLst = new List<GameObject>();
    private void Update()
    {
        Touch();
    }
    void Touch()
    {
        touchPosLst.Clear();
#if !UNITY_EDITOR
        if (Input.touchCount > 0)
        {
            if (RayccastUtils.GetUINum() > 0) return;
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    touchEvent_Up.Invoke(Input.GetTouch(0).position);
                }
                else
                {
                    touchEvent.Invoke(Input.GetTouch(0).position);
                }
            }
            for (int i = 0; i < Input.touchCount; i++) { touchPosLst.Add(Input.GetTouch(i).position); }
        }
#endif
        if (Input.GetMouseButtonUp(0))
        {
            if (RayccastUtils.GetUINum() > 0) return;
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            touchEvent_Up.Invoke(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonUp(0)) return;
            if (RayccastUtils.GetUINum() > 0) return;
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            touchEvent.Invoke(Input.mousePosition);
            touchPosLst.Add(Input.mousePosition);
        }

        touchObjLst.Clear();
        RaycastHit hit;
        for (int i = 0; i < touchPosLst.Count; i++)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosLst[i]);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.collider.gameObject != null)
                {
                    touchObjLst.Add(hit.collider.gameObject);
                }
            }
        }
    }
}

public class FingerTouchEvent_Up : UnityEvent<Vector2> { }
public class FingerTouchEvent_Touch : UnityEvent<Vector2> { }