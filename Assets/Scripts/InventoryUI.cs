using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryUI : MonoBehaviour
{
    public RectTransform button;
    public RectTransform items;
    public void OpenAnimation(){
        button.DOMoveY(-button.rect.height/2, .5f);
        items.DOMoveY(items.rect.height/2, 0.5f);
    }

    public void CloseAnimation(){
        button.DOMoveY(button.rect.height/2, .5f);
        items.DOMoveY(-items.rect.height/2, 0.5f);

    }
}
