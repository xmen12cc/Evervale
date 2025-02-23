using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleCrafting : MonoBehaviour
{
    [SerializeField] private Transform CrafttUI;
    private bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I pressed! Opening Crafting menu...");
            if(isOpen == false){
                CrafttUI.gameObject.SetActive(true);
                isOpen = true;
            }
            else{
                CrafttUI.gameObject.SetActive(false);
                isOpen = false;
                }
        }
    }
}
