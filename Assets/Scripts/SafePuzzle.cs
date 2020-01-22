using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SafePuzzle : MonoBehaviour
{
    public List<PetrolItem> items;

    [Header("Components")]
    [SerializeField] Animator doorAnimator;
    [SerializeField] Transform buttonsObject;
    [SerializeField] TextMeshPro visorText;
    [SerializeField] AudioSource audioPlayer;
    [Header("Sound Effects")]
    [SerializeField] AudioClip keyPressSFX;
    [SerializeField] AudioClip lockOpenSFX;
    [SerializeField] AudioClip wrongPasswordSFX;
    string password;
    string input;
    // Start is called before the first frame update
    void Start()
    {
        GeneratePassword();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GeneratePassword(){
        password = "";
        for(int i =0; i < 3; i++){
            password += items[Random.Range(0, items.Count)].value.ToString();
        }
        Debug.Log("Safe Password: " + password);
    }

    public void Input(int value){
        input += value.ToString();
        visorText.text = input;
        audioPlayer.PlayOneShot(keyPressSFX);
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
