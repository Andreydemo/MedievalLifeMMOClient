using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LoginCanvas : MonoBehaviour
{
    public UnityEngine.UI.InputField loginInputField;

    public UnityEngine.UI.InputField passwordInputField;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Show()
    {
        this.gameObject.SetActive(true);
    }


    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Login()
    {
        string login = loginInputField.text;
        string password = passwordInputField.text;


        EditorUtility.DisplayDialog("Debug", "Login: " + login + "\nPassword: " + password, "Не ну ок", "Похер");
    }
}
