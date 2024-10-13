using GMVC.Utls;
using UnityEngine;
using UnityEngine.Events;

public class JoyStick : MonoBehaviour
{
    [SerializeField] RectTransform joyStick;
    [SerializeField] RectTransform handle;
    public float mRadius = 0f;
    public Vector3 mousePosition, dragPosition;
    //player透过注册事件来移动
    public readonly UnityEvent<Vector3> OnMoveEvent = new();

    public bool IsDrag;
    public Vector3 pos;
    public bool IsInit { get; private set; }
    void Start() => Init();
    public void Init()
    {
        if (IsInit) return;
        IsInit = true;
        if (!joyStick) joyStick = transform.GetChild(0).GetComponent<RectTransform>();
        if (!handle) handle = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        mRadius = joyStick.rect.width * 0.45f;
        joyStick.Display(false);
    }
    void Update()
    {
        if (!IsInit) return;
        //点击
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag(Input.mousePosition);
            return;
        }
        //抬起
        if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
            return;
        }
        //拖拽
        if (IsDrag && Input.GetMouseButton(0))
        {
            Dragging(Input.mousePosition);
            return;
        }
    }

    void Dragging(Vector3 mousePos)
    {
        dragPosition = mousePos;
        pos = dragPosition - mousePosition;
        if (pos.magnitude > mRadius)
            pos = pos.normalized * mRadius;
        handle.localPosition = pos;
        OnMoveEvent.Invoke(pos.normalized);
    }

    void EndDrag()
    {
        var zero = Vector3.zero;
        IsDrag = false;
        mousePosition = zero;
        handle.localPosition = zero;
        joyStick.Display(false);
        OnMoveEvent.Invoke(zero);
    }

    void StartDrag(Vector3 mousePos)
    {
        mousePosition = mousePos;
        joyStick.transform.position = mousePos;
        joyStick.Display(true);
        OnMoveEvent.Invoke(Vector3.zero);
        IsDrag = true;
    }
}