using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

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

    [SerializeField] private int _temporaryId;

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

        _nicknameNoExist.gameObject.SetActive(false);
        _enterPasswordRegister.gameObject.SetActive(false);
        _passwordRegisterInput.gameObject.SetActive(false);
        _registerButton.SetActive(false);
    }

    private void ShowEnterPasswordRegisterUI()
    {
        _nicknameNoExist.gameObject.SetActive(true);
        _enterPasswordRegister.gameObject.SetActive(true);
        _passwordRegisterInput.gameObject.SetActive(true);
        _registerButton.SetActive(true);

        _nicknameExist.gameObject.SetActive(false);
        _enterPasswordLogin.gameObject.SetActive(false);
        _passwordLoginInput.gameObject.SetActive(false);
        _loginButton.SetActive(false);
    }

    public void OnLoginButtonClicked()
    {

    }

    public void OnRegisterButtonClicked()
    {

    }



    public void ResetPlayerPrefs()
    {
        Debug.Log("Nickname: " + PlayerPrefs.GetString("Nickname"));
        Debug.Log("Password: " + PlayerPrefs.GetString("Password"));
        Debug.Log("id: " + PlayerPrefs.GetInt("_id"));        
        PlayerPrefs.DeleteKey("Nickname");
        PlayerPrefs.DeleteKey("Password");
        Debug.Log("Nickname: " + PlayerPrefs.GetString("Nickname"));
        Debug.Log("Password: " + PlayerPrefs.GetString("Password"));
        Debug.Log("id: " + PlayerPrefs.GetInt("_id"));
    }

////////////////////////////REST/////////////////////////
    public void OnCheckNicknameButtonClicked()
    {
        _nickName = _nicknameInput.text;
        StartCoroutine(CheckNicknameCoroutine(_nickName));
    }

    private IEnumerator CheckNicknameCoroutine(string nickname)
    {
        string url = "https://ukfig2.sk/ukfIG2_Piskvorky/nickNameExist.php"; // Replace with your backend URL
        string json = "{\"nickname\": \"" + nickname + "\"}";

        // Debug log for the data being sent to the backend
        Debug.Log("Sending to backend: URL = " + url + ", Data = " + json);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                // Parse the response
                string responseText = www.downloadHandler.text;
                Debug.Log("Response Text: " + responseText); // Debug log for response text
                HandleNicknameResponse(responseText);
            }
        }
    }

    private void HandleNicknameResponse(string responseText)
    {
        // Assuming the response is in JSON format
        // Example response: {"status": "Nothing"} or {"status": "idFound", "id": 123}
        try
        {
            var response = JsonUtility.FromJson<NicknameResponse>(responseText);
            Debug.Log("Parsed Response: " + response.status); // Debug log for parsed response

            if (response.status == "Nothing")
            {
                ShowEnterPasswordRegisterUI();
            }
            else if (response.status == "idFound")
            {
                _id = response.id;
                _temporaryId = response.id; // Set the _temporaryId
                ShowEnterPasswordLoginUI();
            }
        }
        catch (ArgumentException e)
        {
            Debug.LogError("JSON parse error: " + e.Message);
        }
    }

    [System.Serializable]
    private class NicknameResponse
    {
        public string status;
        public int id;
    }
}