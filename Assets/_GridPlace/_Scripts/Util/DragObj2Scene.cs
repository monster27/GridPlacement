using UnityEngine;
using UnityEngine.EventSystems;

public class DragObj2Scene : MonoBehaviour, IPointerDownHandler
{
    public string dragName;
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(dragName);
        if (string.IsNullOrEmpty(dragName)) { dragName = gameObject.name; }
        Messenger.Broadcast(MessengerEvents.OnDragBegin, dragName);
    }
}
