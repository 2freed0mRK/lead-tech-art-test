using System.Collections.Generic;
using System.Data.Common;
using DG.Tweening;
using UnityEngine;

public class MenuFooterController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform indicator;
    [SerializeField] private CanvasGroup indicatorCanvasGroup;
    [SerializeField] private ButtonFooterController startSelected;
    [SerializeField] private List<ButtonFooterController> footerButtons;
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
        CalculateSize();

        if (startSelected != null)
        {
            _buttonSelected = startSelected;
            startSelected.SetSelect(true);
            _hasSelection = true;
            
            MoveIndicator(0f);
        }
        else
        {
            _hasSelection = false;
            indicatorCanvasGroup.alpha = 0f;
        }
        
        UpdateLayout(0f);
    }

    void OnEnable()
    {
        foreach (var btn in footerButtons)
        {
            btn.OnButtonClickedEvent.AddListener(OnButtonClickedEvent);
        }
    }

    void OnDisable()
    {
        foreach (var btn in footerButtons)
        {
            btn.OnButtonClickedEvent.RemoveListener(OnButtonClickedEvent);
        }
    }

    private void CalculateSize()
    {
        _sizeEqually = _layoutGroup.rect.size.x / footerButtons.Count;
        _selectedSize = _sizeEqually * _selectedDelta;
        Debug.LogFormat("{0}", _selectedSize);
        _sizeUnselected = (_layoutGroup.rect.size.x - _selectedSize) / (footerButtons.Count - 1);
    }

    private void UpdateLayout(float duration)
    {
        var targetSize = _hasSelection ? _sizeUnselected : _sizeEqually;   
        float currentXOffset = 0f;
        for (int i = 0; i < footerButtons.Count; i++)
        {
            var btn = footerButtons[i];
            float elementWidth = btn.IsSelected ? _selectedSize : targetSize;
            btn.Rect.anchorMin = new Vector2(0, 0.5f);
            btn.Rect.anchorMax = new Vector2(0, 0.5f);
            btn.Rect.pivot = new Vector2(0, 0.5f);

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
        if (footerButtons.Contains(buttonClicked))
        {
            if (_buttonSelected == buttonClicked)
            {
                _buttonSelected = null;
                _currentSlot = null;
                foreach (var btn in footerButtons)
                {
                    btn.SetSelect(false);
                }

                indicatorCanvasGroup.alpha = 0f;
                _hasSelection = false;
                UpdateLayout(_animationDuration);
                return;
            }

            _buttonSelected = buttonClicked;

            foreach (var btn in footerButtons)
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
        
        indicatorCanvasGroup.alpha = 1f;
        _hasSelection = true;

        float targetXPosition = 0f;

        for (int i = 0; i < footerButtons.Count; i++)
        {
            if (footerButtons[i] == _buttonSelected)
            {
                break;
            }
            targetXPosition += _sizeUnselected;
        }

        if (indicator.pivot.x == 0.5f)
        {
            targetXPosition += _selectedSize * 0.5f;
        }
        indicator.anchorMin = new Vector2(0, 0.5f);
        indicator.anchorMax = new Vector2(0, 0.5f);
        
        indicator.DOKill();
        Debug.Log(targetXPosition);
        indicator.DOAnchorPosX(targetXPosition, duration).SetEase(_animationEase).OnComplete(() =>
        {
            indicator.anchoredPosition = new Vector2(targetXPosition, indicator.anchoredPosition.y);
        });
    }
}