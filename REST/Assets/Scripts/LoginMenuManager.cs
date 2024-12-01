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

    [SerializeField] private TMP_Text _wrongPassword;

    [SerializeField] private int _id;
    [SerializeField] private string _nickName;
    [SerializeField] private string _password;
    [SerializeField] private int _temporaryId;

    [SerializeField] private GameObject _notLoggedInCanvas;
    [SerializeField] private GameObject _loggedInCanvas;

    [SerializeField] private GameObject _startSearchingForOponentButton;
    [SerializeField] private GameObject _buttonPrefab; // Assign the button prefab in the Inspector
    [SerializeField] private Transform _contentParent; // Assign the Content GameObject in the Inspector    

    private void Start()
    {
        // Initially hide all UI elements
        HideAllUI();
        _loggedInCanvas.gameObject.SetActive(false);
        _notLoggedInCanvas.gameObject.SetActive(true);
        _id = PlayerPrefs.GetInt("Id");
        _nickName = PlayerPrefs.GetString("Nickname");
        _password = PlayerPrefs.GetString("Password");
        Debug.Log(_id);
        Debug.Log(_nickName);
        Debug.Log(_password);
        // Check PlayerPrefs for nickname and password
        if (_id == 0 && string.IsNullOrEmpty(_nickName) && string.IsNullOrEmpty(_password))
        {
            // Show UI for entering nickname
            ShowEnterNicknameUI();
        }
        else
        {
            //Prehodime sa do druheheo canvasu
            _loggedInCanvas.gameObject.SetActive(true);
            _notLoggedInCanvas.gameObject.SetActive(false);

            // Display start searching for opponent button
            _startSearchingForOponentButton.gameObject.SetActive(true);
            OnFetchOpponentsButtonClicked();
        }
    }

    private void Update()
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

        _wrongPassword.gameObject.SetActive(false);
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

    private void ResetPlayerPrefs()
    {
        Debug.Log("Nickname: " + PlayerPrefs.GetString("Nickname"));
        Debug.Log("Password: " + PlayerPrefs.GetString("Password"));
        Debug.Log("id: " + PlayerPrefs.GetInt("Id"));        
        PlayerPrefs.DeleteKey("Nickname");
        PlayerPrefs.DeleteKey("Password");
        PlayerPrefs.DeleteKey("Id");
        Debug.Log("Nickname: " + PlayerPrefs.GetString("Nickname"));
        Debug.Log("Password: " + PlayerPrefs.GetString("Password"));
        Debug.Log("id: " + PlayerPrefs.GetInt("Id"));
    }

////////////////////////////REST    CheckNickname/////////////////////////
    private void OnCheckNicknameButtonClicked()
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
///////////////////////////////REST CheckNickname////////////////////////////////
////////////////////////////REST Login Button/////////////////////////
private void OnLoginButtonClicked()
{
    _password = _passwordLoginInput.text;
    StartCoroutine(LoginCoroutine(_temporaryId, _password));
}

private IEnumerator LoginCoroutine(int id, string password)
{
    string url = "https://ukfig2.sk/ukfIG2_Piskvorky/login.php"; // Replace with your backend URL
    string json = "{\"id\": " + id + ", \"password\": \"" + password + "\"}";

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
            HandleLoginResponse(responseText);
        }
    }
}

private void HandleLoginResponse(string responseText)
{
    // Assuming the response is in JSON format
    // Example response: {"status": "success", "id": 123, "nickname": "ivan", "password": "password123"}
    try
    {
        var response = JsonUtility.FromJson<LoginResponse>(responseText);
        Debug.Log("Parsed Response: " + response.status); // Debug log for parsed response

        if (response.status == "success")
        {
            _id = response.id;
            PlayerPrefs.SetInt("Id", _id);
            _nickName = response.nickname;
            PlayerPrefs.SetString("Nickname", _nickName);
            _password = response.password;
            PlayerPrefs.SetString("Password", _password);

            // Hide all UI elements
            _notLoggedInCanvas.gameObject.SetActive(false);
            _loggedInCanvas.gameObject.SetActive(true);
        }
        else
        {
            _wrongPassword.gameObject.SetActive(true);
        }
    }
    catch (ArgumentException e)
    {
        Debug.LogError("JSON parse error: " + e.Message);
    }
}

[System.Serializable]
private class LoginResponse
{
    public string status;
    public int id;
    public string nickname;
    public string password;
}
///////////////////////////////REST Login Button////////////////////////////////
///////////////////////////////REST Register Button////////////////////////////////
private void OnRegisterButtonClicked()
{
    _nickName = _nicknameInput.text;
    _password = _passwordRegisterInput.text;
    StartCoroutine(RegisterCoroutine(_nickName, _password));
}

private IEnumerator RegisterCoroutine(string nickname, string password)
{
    string url = "https://ukfig2.sk/ukfIG2_Piskvorky/register.php"; // Replace with your backend URL
    string json = "{\"nickname\": \"" + nickname + "\", \"password\": \"" + password + "\"}";

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
            HandleRegisterResponse(responseText);
        }
    }
}

private void HandleRegisterResponse(string responseText)
{
    // Assuming the response is in JSON format
    // Example response: {"status": "success", "id": 123, "nickname": "ivan", "password": "password123"}
    try
    {
        var response = JsonUtility.FromJson<RegisterResponse>(responseText);
        Debug.Log("Parsed Response: " + response.status); // Debug log for parsed response

        if (response.status == "success")
        {
            _id = response.id;
            PlayerPrefs.SetInt("Id", _id);
            _nickName = response.nickname;
            PlayerPrefs.SetString("Nickname", _nickName);
            _password = response.password;
            PlayerPrefs.SetString("Password", _password);

            // Hide all UI elements
            _notLoggedInCanvas.gameObject.SetActive(false);
            _loggedInCanvas.gameObject.SetActive(true);
        }
        else
        {
            _wrongPassword.text = "Registration failed";
            _wrongPassword.gameObject.SetActive(true);
        }
    }
    catch (ArgumentException e)
    {
        Debug.LogError("JSON parse error: " + e.Message);
    }
}

[System.Serializable]
private class RegisterResponse
{
    public string status;
    public int id;
    public string nickname;
    public string password;
}
///////////////////////////////REST Register Button////////////////////////////////
//////////////////////////////REST StartSearching for oponent//////////////////////
public void OnStartSearchingForOponentButtonClicked()
{
    _id = PlayerPrefs.GetInt("Id");
    _nickName = PlayerPrefs.GetString("Nickname");
    _password = PlayerPrefs.GetString("Password");
    StartCoroutine(StartSearchingForOponentCoroutine(_id, _nickName, _password));
}

private IEnumerator StartSearchingForOponentCoroutine(int id, string nickname, string password)
{
    string url = "https://ukfig2.sk/ukfIG2_Piskvorky/start_searching.php"; // Replace with your backend URL
    string json = "{\"id\": " + id + ", \"nickname\": \"" + nickname + "\", \"password\": \"" + password + "\"}";

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
            HandleStartSearchingResponse(responseText);
        }
    }
}

private void HandleStartSearchingResponse(string responseText)
{
    // Assuming the response is in JSON format
    // Example response: {"status": "success"} or {"status": "unsuccessful"}
    try
    {
        var response = JsonUtility.FromJson<StartSearchingResponse>(responseText);
        Debug.Log("Parsed Response: " + response.status); // Debug log for parsed response

        if (response.status == "success")
        {
            Debug.Log("Successfully started searching for an opponent.");
        }
        else
        {
            Debug.LogError("Failed to start searching for an opponent.");
        }
    }
    catch (ArgumentException e)
    {
        Debug.LogError("JSON parse error: " + e.Message);
    }
}

[System.Serializable]
private class StartSearchingResponse
{
    public string status;
}
//////////////////////////////REST StartSearching for oponent//////////////////////
//////////////////////////////REST Fetch Opponents//////////////////////
    public void OnFetchOpponentsButtonClicked()
    {
        _id = PlayerPrefs.GetInt("Id");
        _password = PlayerPrefs.GetString("Password");
        StartCoroutine(FetchOpponentsCoroutine(_id, _password));
    }


    private IEnumerator FetchOpponentsCoroutine(int id, string password)
    {
        string url = "https://ukfig2.sk/ukfIG2_Piskvorky/fetch_opponents.php"; // Replace with your backend URL
        string json = "{\"id\": " + id + ", \"password\": \"" + password + "\"}";

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
                HandleFetchOpponentsResponse(responseText);
            }
        }
    }

    private void HandleFetchOpponentsResponse(string responseText)
    {
        // Assuming the response is in JSON format
        // Example response: {"status": "success", "opponents": [{"id": 123, "nickname": "ivan", "invitee_user_id": 456, "came_here_at": "2024-11-30 12:00:00"}]}
        try
        {
            var response = JsonUtility.FromJson<FetchOpponentsResponse>(responseText);
            Debug.Log("Parsed Response: " + response.status); // Debug log for parsed response

            if (response.status == "success")
            {
                foreach (var opponent in response.opponents)
                {
                    CreateOpponentButton(opponent);
                }
            }
            else
            {
                Debug.LogError("Failed to fetch opponents.");
            }
        }
        catch (ArgumentException e)
        {
            Debug.LogError("JSON parse error: " + e.Message);
        }
    }

    private void CreateOpponentButton(OpponentData opponent)
    {
        // Create a new button for each opponent
        GameObject newButton = Instantiate(_buttonPrefab, _contentParent);
        TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
    
        if (buttonText == null)
        {
            Debug.LogError("Button Prefab does not have a TMP_Text component.");
            return;
        }
    
        buttonText.text = opponent.nickname;
        newButton.GetComponent<Button>().onClick.AddListener(() => OnOpponentButtonClicked(opponent.invitee_user_id));
    
        // Position the new button 40 units above the existing buttons
        RectTransform newButtonRectTransform = newButton.GetComponent<RectTransform>();
        RectTransform contentRectTransform = _contentParent.GetComponent<RectTransform>();
        newButtonRectTransform.anchoredPosition = new Vector2(0, -40 * (_contentParent.childCount - 1));
    
        newButton.SetActive(true);
    }


    private void OnOpponentButtonClicked(int opponentId)
{
    _id = PlayerPrefs.GetInt("Id");
    _nickName = PlayerPrefs.GetString("Nickname");
    _password = PlayerPrefs.GetString("Password");
    StartCoroutine(InviteOpponentCoroutine(opponentId, _id, _nickName, _password));
}

private IEnumerator InviteOpponentCoroutine(int opponentId, int userId, string nickname, string password)
{
    string url = "https://ukfig2.sk/ukfIG2_Piskvorky/invite_opponent.php"; // Replace with your backend URL
    string json = "{\"opponentId\": " + opponentId + ", \"userId\": " + userId + ", \"nickname\": \"" + nickname + "\", \"password\": \"" + password + "\"}";

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
            HandleInviteOpponentResponse(responseText);
        }
    }
}

private void HandleInviteOpponentResponse(string responseText)
{
    // Assuming the response is in JSON format
    // Example response: {"status": "success"} or {"status": "unsuccessful"}
    try
    {
        var response = JsonUtility.FromJson<InviteOpponentResponse>(responseText);
        Debug.Log("Parsed Response: " + response.status); // Debug log for parsed response

        if (response.status == "success")
        {
            Debug.Log("Successfully invited opponent.");
        }
        else
        {
            Debug.LogError("Failed to invite opponent.");
        }
    }
    catch (ArgumentException e)
    {
        Debug.LogError("JSON parse error: " + e.Message);
    }
}

[System.Serializable]
private class InviteOpponentResponse
{
    public string status;
}

    [System.Serializable]
    private class FetchOpponentsResponse
    {
        public string status;
        public List<OpponentData> opponents;
    }

    [System.Serializable]
    private class OpponentData
    {
        public int id;
        public string nickname;
        public int invitee_user_id;
        public string came_here_at;
    }
    //////////////////////////////REST Fetch Opponents//////////////////////

}