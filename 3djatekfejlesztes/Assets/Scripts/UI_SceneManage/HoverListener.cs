using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverListener : MonoBehaviour, IPointerEnterHandler
{
    private MainMenuAudio mma;

    private void Start()
    {
        mma = FindObjectOfType<MainMenuAudio>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mma.HoverSound();
    }
}
