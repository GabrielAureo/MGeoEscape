﻿using System.Collections;
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
        selectionScreen.interactable = false;
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        var submit = new TMP_InputField.SubmitEvent();
        submit.AddListener(ConfirmName);
        inputField.onSubmit = submit;
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

    public void ConfirmName(string name){
        nameScreen.DOFade(0f, .2f).onComplete += ()=> nameScreen.interactable = false;
        selectionScreen.DOFade(1f, .2f).onComplete += ()=> selectionScreen.interactable = true;
        Connect();
    }

    void Connect(){
        #if UNITY_EDITOR
        NetworkManager.singleton.StartHost();
        #else
        NetworkDiscovery.onReceivedServerResponse += (info)=>{
            NetworkManager.singleton.networkAddress = info.EndPoint.Address.ToString();
            NetworkManager.singleton.StartClient();
            StopCoroutine(RefreshLAN());
        };
        StartCoroutine(RefreshLAN());
        #endif
        
    }
    IEnumerator RefreshLAN(){
        while(true){
            NetworkDiscovery.SendBroadcast();
            yield return new WaitForSeconds(1f);
        }
    }
}
