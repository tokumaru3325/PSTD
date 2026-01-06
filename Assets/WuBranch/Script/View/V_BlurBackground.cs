using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class V_BlurBackground : MonoBehaviour, IPointerClickHandler
{
    [Serializable]
    public class ButtonClickedEvent : UnityEvent
    {
    }

    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    public ButtonClickedEvent onClick
    {
        get
        {
            return m_OnClick;
        }
        set
        {
            m_OnClick = value;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_OnClick?.Invoke();
    }
}
