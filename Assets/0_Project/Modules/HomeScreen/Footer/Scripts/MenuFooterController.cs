using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MenuFooterController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform _indicator;
    [SerializeField] private CanvasGroup _indicatorCanvasGroup;
    [SerializeField] private ButtonFooterController _startSelected;
    [SerializeField] private List<ButtonFooterController> _footerButtons;
    [SerializeField] private Ease _animationEase = Ease.OutSine;
    [SerializeField] private float _animationDuration = 0.25f;

    [Header("Components")]
    [SerializeField] private RectTransform _layoutGroup;
    [SerializeField] private float _selectedDelta = 1.25f;
    
    //Internal
    private ButtonFooterController _buttonSelected;
    private RectTransform _currentSlot;
    private float _sizeEqually;
    private float _sizeUnselected;
    private float _selectedSize;
    private bool _hasSelection = true;

    void Start()
    {
        Inizilize();
    }

    private void Inizilize()
    {
        CalculateSize();
        
        _indicator.anchorMin = new Vector2(0, 0.5f);
        _indicator.anchorMax = new Vector2(0, 0.5f);

        if (_startSelected != null)
        {
            _buttonSelected = _startSelected;
            _startSelected.SetSelect(true);
            _hasSelection = true;
            
            MoveIndicator(0f);
        }
        else
        {
            _hasSelection = false;
            _indicatorCanvasGroup.alpha = 0f;
        }
        
        UpdateLayout(0f);
    }

    void OnEnable()
    {
        foreach (var btn in _footerButtons)
        {
            btn.OnButtonClickedEvent += OnButtonClickedEvent;
        }
    }

    void OnDisable()
    {
        foreach (var btn in _footerButtons)
        {
            btn.OnButtonClickedEvent -= OnButtonClickedEvent;
        }
    }

    private void CalculateSize()
    {
        if (_footerButtons.Count <= 2)
        {
            Debug.LogWarning("Icons in footer is less than 3");
             return;
        }
        _sizeEqually = _layoutGroup.rect.size.x / _footerButtons.Count;
        _selectedSize = _sizeEqually * _selectedDelta;
        _sizeUnselected = (_layoutGroup.rect.size.x - _selectedSize) / (_footerButtons.Count - 1);
    }

    private void UpdateLayout(float duration)
    {
        var targetSize = _hasSelection ? _sizeUnselected : _sizeEqually;   
        float currentXOffset = 0f;
        for (int i = 0; i < _footerButtons.Count; i++)
        {
            var btn = _footerButtons[i];
            float elementWidth = btn.IsSelected ? _selectedSize : targetSize;

            btn.Rect.DOKill();

            if (duration > 0f)
            {
                btn.Rect.DOSizeDelta(new Vector2(elementWidth, btn.Rect.sizeDelta.y), duration).SetEase(_animationEase);
                btn.Rect.DOAnchorPosX(currentXOffset, duration).SetEase(_animationEase);
            }
            else
            {
                btn.Rect.sizeDelta = new Vector2(elementWidth, btn.Rect.sizeDelta.y);
                btn.Rect.anchoredPosition = new Vector2(currentXOffset, 0f);
            }

            currentXOffset += elementWidth;
        }
    }

    private void OnButtonClickedEvent(ButtonFooterController buttonClicked)
    {
        if (_footerButtons.Contains(buttonClicked))
        {
            if (_buttonSelected == buttonClicked)
            {
                _buttonSelected = null;
                _currentSlot = null;
                foreach (var btn in _footerButtons)
                {
                    btn.SetSelect(false);
                }

                _indicatorCanvasGroup.alpha = 0f;
                _hasSelection = false;
                UpdateLayout(_animationDuration);
                return;
            }

            _buttonSelected = buttonClicked;

            foreach (var btn in _footerButtons)
            {
                btn.SetSelect(_buttonSelected == btn);
            }
            _hasSelection = true;
            UpdateLayout(_animationDuration);
            MoveIndicator(_animationDuration);
        }
    }

    private void MoveIndicator(float duration)
    {
        if (_buttonSelected == null) return;
        if (_currentSlot == _buttonSelected.Rect) return;
        _currentSlot = _buttonSelected.Rect;
        
        _indicatorCanvasGroup.alpha = 1f;
        _hasSelection = true;

        float targetXPosition = 0f;

        for (int i = 0; i < _footerButtons.Count; i++)
        {
            if (_footerButtons[i] == _buttonSelected)
            {
                break;
            }
            targetXPosition += _sizeUnselected;
        }

        if (_indicator.pivot.x == 0.5f)
        {
            targetXPosition += _selectedSize * 0.5f;
        }
        
        _indicator.DOKill();
        _indicator.DOAnchorPosX(targetXPosition, duration).SetEase(_animationEase).OnComplete(() =>
        {
            _indicator.anchoredPosition = new Vector2(targetXPosition, _indicator.anchoredPosition.y);
        });
    }
}