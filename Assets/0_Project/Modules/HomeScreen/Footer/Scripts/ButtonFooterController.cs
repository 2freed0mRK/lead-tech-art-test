using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFooterController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Button _footerBtn;
    [SerializeField] private bool _lockOnAwake;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _image;
    [SerializeField] private Image _lock;

    public event Action<ButtonFooterController> OnButtonClickedEvent;

    private readonly Vector2 _defaultAnchor = new Vector2(0f, 0.5f);
    private readonly Vector2 _defaultPivot = new Vector2(0f, 0.5f);

    // Internal
    private bool _isSelected;
    private bool _isLocked;

    public RectTransform Rect => _rectTransform;
    public bool IsSelected => _isSelected;

    private void Awake()
    {
        SetLock(_lockOnAwake);
        SetDefaultRect();
    }

    private void OnEnable()
    {
        _footerBtn.onClick.AddListener(HandleClick);
    }

    private void OnDisable()
    {
        _footerBtn.onClick.RemoveListener(HandleClick);
    }

    private void HandleClick()
    {
        OnButtonClickedEvent?.Invoke(this);
    }

    public void SetLock(bool locked)
    {
        _isLocked = locked;
        _footerBtn.interactable = !_isLocked;
        _animator.SetBool("Locked", _isLocked);
        _image.gameObject.SetActive(!_isLocked);
        _lock.gameObject.SetActive(_isLocked);
    }

    public void SetDefaultRect()
    {
        _rectTransform.anchorMin = _defaultAnchor;
        _rectTransform.anchorMax = _defaultAnchor;
        _rectTransform.pivot = _defaultPivot;
    }

    public void SetSelect(bool selected)
    {
        _isSelected = selected;
        _animator.SetBool("Selected", _isSelected);
    }
}