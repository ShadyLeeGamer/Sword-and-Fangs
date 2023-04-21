using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class MainMenuUI : MonoBehaviour
{
    public Button[] MainMenuBtns;
    public GameObject FirstOption;
    public GameObject OptionsScreen;
    private bool _optionMenu;
    public Slider[] Volumes;


    public bool OptionMenu { get {
            return _optionMenu;
        }
        set {
            _optionMenu = value;
            OptionsScreen.SetActive(value);
            foreach (Button Btns in MainMenuBtns)
            {
                if(value == true)
                {
                    SetSelection(FirstOption);
                }
                else
                {
                    SetSelection(MainMenuBtns[0].gameObject);

                }
                Btns.interactable = !value;
            }
        } }


    public void Start()
    {
        SetSelection(MainMenuBtns[0].gameObject);
        OptionMenu = false;

    }
    public void SetOptionMenu(bool Active)
    {
        OptionMenu = Active;
        

    }
    public void SetSelection(GameObject Selection)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(Selection);
    }
   
}
