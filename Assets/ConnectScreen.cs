using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Mirror;
using DG.Tweening;

public class ConnectScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup nameScreen;
    [SerializeField] CanvasGroup selectionScreen;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button acceptButton;

    Tweener fade;
    bool reveal;


    void Start()
    {
        reveal = false;
        selectionScreen.alpha = 0;
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        //inputField.OnPointerClick (null);
    }

    // Update is called once per frame
    public void RevealButton(){
        if(!reveal){
            if(fade != null) fade.Kill();
            fade = acceptButton.GetComponent<CanvasGroup>().DOFade(0f, .2f);
            acceptButton.interactable = false;
            return;
        }
        if(fade != null) fade.Kill();
        fade = acceptButton.GetComponent<CanvasGroup>().DOFade(1f, 1f);
        acceptButton.interactable = true;
    }

    public void CheckInput(string input){
        if(input != "" && !reveal){
            reveal = true;
            RevealButton();
        }else if(input == "" && reveal){
            reveal = false;
            RevealButton();
        }
    }
}
