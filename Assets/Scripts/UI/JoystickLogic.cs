using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class JoystickEventData
{
    public JoystickEvent OnDrag;
    public JoystickEvent OnTouchDown;
    public JoystickEvent OnTouchUp;
    public Action OnClick;
}

public struct JoystickData
{
    public Vector2 Direction;
    public float Horizontal;
    public float Vertical;
}

[Serializable]
public class JoystickEvent : UnityEvent<JoystickData>
{
}

[RequireComponent(typeof(Image))]
public class JoystickLogic : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] bool _alwaysVisible = true;
    [SerializeField] bool _fixedPosition = true;
    [SerializeField] bool _draggable = false;
    [SerializeField] bool _analog = true;
    [SerializeField] [Range(0f, 1f)] float _handleLimit = 0.5f;
    [SerializeField] [Range(0f, 1f)] float _deadZone = 0f;
    [SerializeField] Mode _joystickMode = Mode.AllAxes;

    [SerializeField] RectTransform _background;
    [SerializeField] RectTransform _handle;
    [SerializeField] bool _verbose = false;

    public JoystickEvent Drag;
    public JoystickEvent TouchDown;
    public JoystickEvent TouchUp;
    public Action Click;

    Canvas _canvas;
    Camera _camera;
    Image _touchZone;
    Vector3 _defaultPosition;
    Vector2 _inputVector;
    int _currentPointerId;

    enum Mode
    {
        AllAxes,
        Horizontal,
        Vertical
    }

    public float Horizontal => _inputVector.x;
    public float Vertical => _inputVector.y;
    public Vector2 Direction => _inputVector;

    void Awake()
    {
        _touchZone = gameObject.GetComponent<Image>();
    }

    void Start()
    {
        _canvas = GetComponentInParent<Canvas>();

        Setup();
    }

    void OnValidate()
    {
        Setup();
    }

    void Setup()
    {
        _alwaysVisible = _fixedPosition || _alwaysVisible;

        if(_touchZone != null)
        {
            _touchZone.enabled = !_fixedPosition;
        }

        if(_handle != null && _background != null)
        {
            Vector2 center = new Vector2(0.5f, 0.5f);
            _background.pivot = center;
            _defaultPosition = _background.position;
            _background.gameObject.SetActive(_alwaysVisible);
            _handle.anchorMin = center;
            _handle.anchorMax = center;
            _handle.pivot = center;
            _handle.anchoredPosition = Vector2.zero;
        }

        _currentPointerId = -1;
        _inputVector = Vector2.zero;
    }

    public void InitializeEvents(JoystickEventData eventData)
    {
        Drag = eventData.OnDrag;
        TouchDown = eventData.OnTouchDown;
        TouchUp = eventData.OnTouchUp;
        Click = eventData.OnClick;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_currentPointerId >= 0)
        {
            return;
        }

        _currentPointerId = eventData.pointerId;

        if(!_fixedPosition)
        {
            _background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        }

        _handle.anchoredPosition = Vector2.zero;
        _background.gameObject.SetActive(true);

        InvokeJoystickEvent(TouchDown);
        VerboseLog("OnPointerDown: " + eventData.ToString());
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(_currentPointerId != eventData.pointerId)
        {
            return;
        }

        _camera = _canvas.renderMode == RenderMode.ScreenSpaceCamera ? _canvas.worldCamera : null;
        Vector2 position = RectTransformUtility.WorldToScreenPoint(_camera, _background.position);
        Vector2 radius = _background.sizeDelta * 0.5f;
        Vector2 input = (eventData.position - position) / (radius * _canvas.scaleFactor);
        input = ClampJoystick(input, _joystickMode);

        if(_draggable)
        {
            float inputMagnitude = input.magnitude;
            const float moveThreshold = 1f;
            if(inputMagnitude > moveThreshold)
            {
                Vector2 difference = input.normalized * (inputMagnitude - moveThreshold) * radius;
                _background.anchoredPosition += difference;
            }
        }

        if(input.magnitude > 1f)
        {
            input.Normalize();
        }

        _handle.anchoredPosition = input * radius * _handleLimit;

        input = ClampJoystick(input, _joystickMode, _deadZone);
        _inputVector = _analog ? input : input.normalized;

        InvokeJoystickEvent(Drag);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_currentPointerId != eventData.pointerId)
        {
            return;
        }

        FinishEvent();
        VerboseLog("OnPointerUp: " + eventData.ToString());
    }

    private void OnDisable()
    {
        FinishEvent();
    }

    private void FinishEvent()
    {
        _currentPointerId = -1;

        _background.position = _defaultPosition;
        _handle.anchoredPosition = Vector2.zero;
        _background.gameObject.SetActive(_alwaysVisible);

        InvokeJoystickEvent(TouchUp);
        _inputVector = Vector2.zero;
    }

    static float RemapJoystickAxis(float axis, float deadZone)
    {
        return axis * (1.0f - deadZone) + (deadZone * Mathf.Sign(axis));
    }

    static Vector2 ClampJoystick(Vector2 input, Mode mode, float deadZone = 0f)
    {
        float xAxis = 0f;
        float yAxis = 0f;

        switch(mode)
        {
            case Mode.Horizontal:
                {
                    if(deadZone < Mathf.Abs(input.x))
                    {
                        xAxis = RemapJoystickAxis(input.x, deadZone);
                    }

                    break;
                }
            case Mode.Vertical:
                {
                    if(deadZone < Mathf.Abs(input.y))
                    {
                        yAxis = RemapJoystickAxis(input.y, deadZone);
                    }

                    break;
                }
            case Mode.AllAxes:
                {
                    if(deadZone < Mathf.Abs(input.x) || deadZone < Mathf.Abs(input.y))
                    {
                        xAxis = deadZone < Mathf.Abs(input.x) ? RemapJoystickAxis(input.x, deadZone) : input.x;
                        yAxis = deadZone < Mathf.Abs(input.y) ? RemapJoystickAxis(input.y, deadZone) : input.y;
                    }

                    break;
                }
        }

        return new Vector2(xAxis, yAxis);
    }

    Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint;
        var baseRect = _touchZone.rectTransform;
        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, _camera, out localPoint))
        {
            return Vector2.zero;
        }

        _defaultPosition = _background.position;

        _background.localPosition = Vector3.zero;
        var offset = _background.anchoredPosition;

        Vector2 sizeDelta = baseRect.sizeDelta;
        Vector2 pivotOffset = baseRect.pivot * sizeDelta;
        return localPoint - (_background.anchorMax * sizeDelta) + pivotOffset + offset;
    }

    void InvokeJoystickEvent(JoystickEvent joystickEvent)
    {
        joystickEvent?.Invoke(new JoystickData
        {
            Direction = Direction,
            Horizontal = Horizontal,
            Vertical = Vertical
        });
    }

    public void Scale(float factor)
    {
        _background.sizeDelta *= factor;
        _handle.sizeDelta *= factor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        VerboseLog("Joystick click! Data: " + eventData.ToString());
    }

    private void VerboseLog(string msg)
    {
        if (_verbose)
        {
            Debug.Log(msg);
        }
    }
}