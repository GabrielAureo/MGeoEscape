using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Events;

public class CharacterButton : UIBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler {
    public enum SelectionState{Selected, Normal, Disabled, Pressed}
    public bool interactable;
    [HideInInspector] SelectionState state;
    public Character character;


    public Graphic targetGraphic;
    [HideInInspector]
    public CharacterSelection selection;
    public ColorBlock colors = ColorBlock.defaultColorBlock;
    /*public Color baseColor;
    public Color selectedColor;
    public Color unselectedColor;
    public Color inactiveColor;
    public Color pressedColor;*/
    //Tween transition;
    /// <summary>
    /// Interaction state of the parent Canvas Group
    /// </summary>
    private bool m_groupInteraction;
    
    public UnityAction<int> clickAction;

    protected override void Start(){
        targetGraphic = GetComponent<Graphic>();
        state = SelectionState.Normal;
    }

    void OnValidate(){
        if(targetGraphic == null) targetGraphic = GetComponent<Graphic>();
        targetGraphic.canvasRenderer.SetColor(colors.normalColor);
    }
    //https://bitbucket.org/Unity-Technologies/ui/src/2019.1/UnityEngine.UI/UI/Core/Selectable.cs
    protected override void OnCanvasGroupChanged()
    {
        //Depth Search of parent that contains Canvas Group
        //Note that it stops as soon as it finds the first Canvas Group, if there are more parent Groups
        //this behaviour needs to change as to find all of them. Check the link above.
        var t = transform.parent;
        CanvasGroup group = null;
        //if t is null, no canvas group has been found
        while(group == null && t != null){
            group = t.GetComponent<CanvasGroup>();
            t = t.parent;
        }

        m_groupInteraction = group.interactable;

    }

    public bool IsInteractable(){
        return interactable && m_groupInteraction;
    }

    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData){   
        Debug.LogError("Cliquei no "+ gameObject.name);
        if(IsInteractable()) selection.CmdSelectCharacter((int)character);

    }
    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void Select(){
        DoTransition(SelectionState.Selected);
    }

    public void Deselect(){
        DoTransition(SelectionState.Normal);
    }

    public void Disable(){
        DoTransition(SelectionState.Disabled);
        interactable = false;
    }

    public void Enable(){
        DoTransition(SelectionState.Normal);
        interactable = true;
    }
    public void Toggle(bool toggle){
        print(toggle);
        if(toggle){
            Enable();
        }else{
            Disable();
        }
    }

    void DoTransition(SelectionState newState){
        //if(transition != null && transition.IsActive()) transition.Kill();
        Color newColor = GetColor(newState);
        targetGraphic.CrossFadeColor(newColor, .1f, true, true);
        state = newState;
    }
    
    Color GetColor(SelectionState state){
        Color color = Color.white;
        switch(state){
            case SelectionState.Normal:
                color = colors.normalColor;
                break;
            case SelectionState.Selected:
                color =  colors.selectedColor;
                break;
            case SelectionState.Pressed:
                color =  colors.pressedColor;
                break;
            case SelectionState.Disabled:
                color =  colors.disabledColor;
                break;
            default:
                Debug.LogError("Invalid State");
                return Color.white;
        }

        return color;
    }

    
}