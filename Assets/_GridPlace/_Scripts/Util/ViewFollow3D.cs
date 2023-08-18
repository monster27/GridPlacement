using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewFollow3D : MonoBehaviour
{
    public RectTransform rectTrans;
    public GameObject followObj;//这里是Slider需要跟随的对象，比如上图所示的马
    public Vector2 offset;

    private void Update()
    {
        if (followObj == null) return;
        Vector2 viewPosition = Camera.main.WorldToScreenPoint(followObj.transform.position);//使用此方法将跟随物件的3维位置转化为屏幕上的2维位置
        rectTrans.position = viewPosition + offset;//再将Slider的位置进行绑定
    }
}
