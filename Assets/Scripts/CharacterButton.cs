using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterButton : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler {
    public enum State{Selected, Unselected, Inactive, Pressed}

    [HideInInspector] State state;
    public Character character;

    public Graphic graphic;
    public Color baseColor;
    public Color selectedColor;
    public Color unselectedColor;
    public Color inactiveColor;
    public Color pressedColor;
    Tween transition;

    void Start(){
        graphic = GetComponent<Graphic>();
        state = State.Unselected;
    }

    void OnValidate(){
        if(graphic == null) graphic = GetComponent<Graphic>();
        graphic.canvasRenderer.SetColor(baseColor);
    }

    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData){   
        //print("Cliquei no "+ gameObject.name);         
        GameLobbyManager.characterSelection.CmdSelectCharacter((int)character);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void Select(){
        DoTransition(State.Selected);
    }

    public void Deselect(){
        DoTransition(State.Unselected);
    }

    public void Disable(){
        DoTransition(State.Inactive);
    }

    public void Toggle(bool toggle){
        print(toggle);
        if(!toggle){
            Select();
        }else{
            Disable();
        }
    }

    void DoTransition(State newState){
        //if(transition != null && transition.IsActive()) transition.Kill();
        Color newColor = GetColor(newState);
        graphic.CrossFadeColor(newColor, .1f, true, true);
        state = newState;
    }
    
    Color GetColor(State state){
        Color color = Color.white;
        switch(state){
            case State.Unselected:
                color = unselectedColor;
                break;
            case State.Selected:
                color =  selectedColor;
                break;
            case State.Pressed:
                color =  pressedColor;
                break;
            case State.Inactive:
                color =  inactiveColor;
                break;
            default:
                Debug.LogError("Invalid State");
                return Color.white;
        }

        return color;
    }

    
}