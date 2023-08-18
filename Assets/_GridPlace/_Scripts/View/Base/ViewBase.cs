using UnityEngine;
using UnityEngine.UI;

public class ViewBase : MonoBehaviour
{
    public GameObject obj;
    private Button[] btns;
     void Awake()
    {
        if (obj == null) { obj = gameObject; }
        btns = GetComponentsInChildren<Button>();
        for (int i = 0; i < btns.Length; i++)
        {
            Button btn = btns[i];
            btn.onClick.AddListener(() => { OnClick(btn); });
        }
        OnAwake();
    }

    protected virtual void OnClick(Button btn) 
    {
        Debug.Log(btn.name + " Click !");
        switch (btn.name)
        {
            case "CloseBtn":
                CloseView();
                break;
            default:
                break;
        }
    }

    protected virtual void OnAwake() { }

    public void OpenView() { if (obj != null) { obj.SetActive(true); } }
    public void CloseView() { if (obj != null) { obj.SetActive(false); } }
}
