using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterButton : Button {
    public Character character;

    public bool selected = false;

    public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData){            
        Toggle();
    }
    public override void OnDeselect(BaseEventData eventData){
        base.OnDeselect(eventData);
        selected = false;
        
    }

    public void Toggle(){
        if(selected){
            //base.DoStateTransition(SelectionState.Normal, true);
            EventSystem.current.SetSelectedGameObject(null);
        }else{
            //base.DoStateTransition(SelectionState.Selected, true);
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
        
        selected = !selected;
        
    }

    
    
}