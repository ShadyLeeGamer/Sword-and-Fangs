using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MobileControls : MonoBehaviour
{
    /*[DllImport("__Internal")]
    private static extern bool IsMobile();

    public bool isMobile()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
             return IsMobile();
#endif
        return false;
    }


    public void Start()
    {
        if (isMobile() == true)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }*/

    public DraculaController dracula;

    public static MobileControls Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void AttackButtonDown()
    {
        dracula.MobileAttackButton();
    }

    public void AttackButtonUp()
    {
        dracula.MobileAttackButtonUp();
    }

    public void DashButton()
    {
        dracula.DashMobileButton();
    }
}