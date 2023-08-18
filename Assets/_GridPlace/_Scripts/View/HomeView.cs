using UnityEngine;
using UnityEngine.UI;

public class HomeView : ViewBase
{
    public GameObject buildSetObj;
    public GameObject buildSelectObj;

    protected override void OnAwake()
    {
        base.OnAwake();
        buildSetObj.SetActive(false);
        Messenger.AddListener(MessengerEvents.GridFingerTouchUp, OnGridFingerTouchUp);
        Messenger.AddListener<string>(MessengerEvents.OnDragBegin, OnDragBegin);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(MessengerEvents.GridFingerTouchUp, OnGridFingerTouchUp);
        Messenger.RemoveListener<string>(MessengerEvents.OnDragBegin, OnDragBegin);
    }

    void OnGridFingerTouchUp() { buildSetObj.SetActive(true); }

    void OnDragBegin(string uiName) 
    {
        switch (uiName)
        {
            case "BedDoubleBtn":
                Messenger.Broadcast(MessengerEvents.OnBuildClick, BuildTag.BedDouble);
                break;
            case "ChairBtn":
                Messenger.Broadcast(MessengerEvents.OnBuildClick, BuildTag.Chair);
                break;
            case "TableBtn":
                Messenger.Broadcast(MessengerEvents.OnBuildClick, BuildTag.Table);
                break;
            default:
                break;
        }
    }
    protected override void OnClick(Button btn)
    {
        base.OnClick(btn);
        switch (btn.name)
        {
            case "MoveBtn":
                Messenger.Broadcast(MessengerEvents.OnBuildMoveClick);
                buildSetObj.SetActive(false);
                break;
            case "RemoveBtn":
                Messenger.Broadcast(MessengerEvents.OnBuildRemoveClick);
                buildSetObj.SetActive(false);
                break;
            case "RotBtn":
                Messenger.Broadcast(MessengerEvents.OnBuildRotClick);
                break;
            case "BuildMakeSureBtn":
                Debug.Log("queding");
                Messenger.Broadcast(MessengerEvents.OnBuildMakeSureClick);
                buildSetObj.SetActive(false);
                break;

            case "SaveBtn":
                Messenger.Broadcast(MessengerEvents.OnBuildSaveClick);
                break;
            case "ReLoadBtn":
                Messenger.Broadcast(MessengerEvents.OnBuildReloadClick);
                break;
            case "SetBuildPlaneCloseBtn":
                buildSetObj.SetActive(false);
                break;
            case "BuildPlaneCloseBtn":
                break;
            default:
                break;
        }
    }
}
