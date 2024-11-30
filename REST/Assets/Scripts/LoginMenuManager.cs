using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LoginMenuScript : MonoBehaviour
{
    // Serialized private TMP input fields and buttons
    [SerializeField] private TMP_Text _enterYourName;
    [SerializeField] private TMP_InputField _nicknameInput;
    [SerializeField] private GameObject _checkNicknameButton;

    [SerializeField] private TMP_Text _nicknameExist;
    [SerializeField] private TMP_Text _enterPasswordLogin;
    [SerializeField] private TMP_InputField _passwordLoginInput;
    [SerializeField] private GameObject _loginButton;

    [SerializeField] private TMP_Text _nicknameNoExist;
    [SerializeField] private TMP_Text _enterPasswordRegister;
    [SerializeField] private TMP_InputField _passwordRegisterInput;
    [SerializeField] private GameObject _registerButton;

    [SerializeField] private int _id;
    [SerializeField] private string _nickName;
    [SerializeField] private string _password;

    void Start()
    {
        // Initially hide all UI elements
        HideAllUI();

        // Check PlayerPrefs for nickname and password
    if (string.IsNullOrEmpty(PlayerPrefs.GetString("Nickname")) || string.IsNullOrEmpty(PlayerPrefs.GetString("Password")) || string.IsNullOrEmpty(PlayerPrefs.GetString("id")))
        {
            // Show UI for entering nickname
            ShowEnterNicknameUI();
        }
        else
        {
            //Prehodime sa do druheheo canvasu

        }
    }

    void Update()
    {
    }

    private void HideAllUI()
    {
        _enterYourName.gameObject.SetActive(false);
        _nicknameInput.gameObject.SetActive(false);
        _checkNicknameButton.SetActive(false);

        _nicknameExist.gameObject.SetActive(false);
        _enterPasswordLogin.gameObject.SetActive(false);
        _passwordLoginInput.gameObject.SetActive(false);
        _loginButton.SetActive(false);

        _nicknameNoExist.gameObject.SetActive(false);
        _enterPasswordRegister.gameObject.SetActive(false);
        _passwordRegisterInput.gameObject.SetActive(false);
        _registerButton.SetActive(false);
    }

    private void ShowEnterNicknameUI()
    {
        _enterYourName.gameObject.SetActive(true);
        _nicknameInput.gameObject.SetActive(true);
        _checkNicknameButton.SetActive(true);
    }

    private void ShowEnterPasswordLoginUI()
    {
        _nicknameExist.gameObject.SetActive(true);
        _enterPasswordLogin.gameObject.SetActive(true);
        _passwordLoginInput.gameObject.SetActive(true);
        _loginButton.SetActive(true);
    }

    private void ShowEnterPasswordRegisterUI()
    {
        _nicknameNoExist.gameObject.SetActive(true);
        _enterPasswordRegister.gameObject.SetActive(true);
        _passwordRegisterInput.gameObject.SetActive(true);
        _registerButton.SetActive(true);
    }

    public void OnCheckNicknameButtonClicked()
    {
        _nickName = _nicknameInput.text;

        // Check if nickname exists in the database (pseudo-code)
        bool nicknameExists = CheckNicknameInDatabase(_nickName);

        if (nicknameExists)
        {
            ShowEnterPasswordLoginUI();
        }
        else
        {
            ShowEnterPasswordRegisterUI();
        }
    }

    public void OnLoginButtonClicked()
    {
        _password = _passwordLoginInput.text;

        // Validate login credentials (pseudo-code)
        bool loginSuccessful = ValidateLogin(_nickName, _password);

        if (loginSuccessful)
        {
            // Proceed to the game
            Debug.Log("Login successful!");
        }
        else
        {
            Debug.Log("Invalid login credentials.");
        }
    }

    public void OnRegisterButtonClicked()
    {
        _password = _passwordRegisterInput.text;

        // Register new user (pseudo-code)
        bool registrationSuccessful = RegisterNewUser(_nickName, _password);

        if (registrationSuccessful)
        {
            // Proceed to the game
            Debug.Log("Registration successful!");
        }
        else
        {
            Debug.Log("Registration failed.");
        }
    }

    private bool CheckNicknameInDatabase(string nickname)
    {
        // Implement database check logic here
        return false; // Placeholder
    }

    private bool ValidateLogin(string nickname, string password)
    {
        // Implement login validation logic here
        return false; // Placeholder
    }

    private bool RegisterNewUser(string nickname, string password)
    {
        // Implement user registration logic here
        return false; // Placeholder
    }

    private void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteKey("Nickname");
        PlayerPrefs.DeleteKey("Password");
    }
}