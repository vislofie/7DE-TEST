using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FP_InventoryIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;

    public bool Interactable => _interactable;
    private bool _interactable;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();

        Activate();
    }

    public void Deactivate()
    {
        _interactable = false;
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    public void Activate()
    {
        _interactable = true;
        _canvasGroup.alpha = 1.0f;
        _canvasGroup.blocksRaycasts = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_interactable)
        {
            _canvasGroup.alpha = 0.6f;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_interactable)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_interactable)
        {
            _canvasGroup.alpha = 1.0f;
            _canvasGroup.blocksRaycasts = true;

            eventData.pointerDrag.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            eventData.pointerDrag.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        }
    }
}
