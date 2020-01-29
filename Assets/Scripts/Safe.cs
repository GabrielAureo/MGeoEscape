using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Safe : MonoBehaviour
{
    public List<PetrolItem> items;

    [Header("Components")]
    [SerializeField] Animator doorAnimator = null;
    [SerializeField] Transform buttonsObject = null;
    [SerializeField] TextMeshPro visorText = null;
    [SerializeField] AudioSource audioPlayer = null;
    [Header("Sound Effects")]
    [SerializeField] AudioClip keyPressSFX = null;
    [SerializeField] AudioClip lockOpenSFX = null;
    [SerializeField] AudioClip wrongPasswordSFX = null;
    [HideInInspector] public string password;
    string input;
    
    public void Input(int value){
        input += value.ToString();
        visorText.text = input;
        audioPlayer.PlayOneShot(keyPressSFX);
        Debug.Log("input: " + input + " password: " + password);
        if(input.Length == password.Length) ParseInput();
    }

    private void ParseInput(){
        if(input == password){
            Unlock();
            audioPlayer.PlayOneShot(lockOpenSFX);
        }else{
            audioPlayer.PlayOneShot(wrongPasswordSFX);
            input = "";
            visorText.text = input;
        }
    }

    private void Unlock(){
        doorAnimator.SetTrigger("Open");
        var triggers = buttonsObject.GetComponentsInChildren<Collider>();
        foreach(var trigger in triggers) trigger.enabled = false;
    }
}
