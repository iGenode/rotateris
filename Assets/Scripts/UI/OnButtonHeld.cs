using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// TODO: this works only with mouse clicks, look into other ways
public class OnButtonHeld : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private UnityEvent _action;

    bool _pressed = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
    }

    void Update()
    {
        if (_pressed)
        {
            //_action.Invoke();
        }
    }
}
