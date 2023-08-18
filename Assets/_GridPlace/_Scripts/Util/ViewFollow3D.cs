using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewFollow3D : MonoBehaviour
{
    public RectTransform rectTrans;
    public GameObject followObj;//������Slider��Ҫ����Ķ��󣬱�����ͼ��ʾ����
    public Vector2 offset;

    private void Update()
    {
        if (followObj == null) return;
        Vector2 viewPosition = Camera.main.WorldToScreenPoint(followObj.transform.position);//ʹ�ô˷��������������3άλ��ת��Ϊ��Ļ�ϵ�2άλ��
        rectTrans.position = viewPosition + offset;//�ٽ�Slider��λ�ý��а�
    }
}
