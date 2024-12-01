using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Newtonsoft.Json;


public class GameManagerMultiPlayer : MonoBehaviour
{
    public enum Player { X, O, None }
    public enum PlayerNumbers { Player_1, Player_2 }
    private Player _player1;
    private Player _player2;

    private Player[,] _currentPlay = new Player[3, 3];
    private GameObject[,] _spawnedObjects = new GameObject[3, 3]; // Tracks spawned objects
    [SerializeField] private Player _currentPlayer;
    [SerializeField] private PlayerNumbers _currentPlayerTurn;
    [SerializeField] private PlayerNumbers _playersTurn;

    [SerializeField] private GameObject _tictactoeButton00;
    [SerializeField] private GameObject _tictactoeButton01;
    [SerializeField] private GameObject _tictactoeButton02;

    [SerializeField] private GameObject _tictactoeButton10;
    [SerializeField] private GameObject _tictactoeButton11;
    [SerializeField] private GameObject _tictactoeButton12;

    [SerializeField] private GameObject _tictactoeButton20;
    [SerializeField] private GameObject _tictactoeButton21;
    [SerializeField] private GameObject _tictactoeButton22;

    [SerializeField] private GameObject _X;
    [SerializeField] private GameObject _O;

    [SerializeField] private int _round = 0;
    [SerializeField] private int _player1Score = 0;
    [SerializeField] private int _player2Score = 0;

    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private int _gameId;
    [SerializeField] private int _userId;

    private void Start()
    {
        _gameId = PlayerPrefs.GetInt("gameId");
        _userId = PlayerPrefs.GetInt("Id");
        Debug.Log(PlayerPrefs.GetInt("Id"));
        Debug.Log(PlayerPrefs.GetString("Password"));
        Debug.Log(PlayerPrefs.GetString("Nickname"));
        Debug.Log(_userId);
        Debug.Log(_gameId);
        //InitializeBoard();
        StartCoroutine(FetchGameData());
        _eventSystem.enabled = true;
        //InvokeRepeating("CheckGameStatus", 5.0f, 5.0f); // Check every 2 seconds
    }

    private void CheckGameStatus()
{
    StartCoroutine(HasGameChanged());
}

    private void Update()
    {
    }

    private void InitializeBoard()
    {
        Debug.Log("Board initialized");
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                _currentPlay[x, y] = Player.None;
                _spawnedObjects[x, y] = null;
            }
        }
        _round=0;
    }

    public void HandleButton00() => HandleButtonClick(_tictactoeButton00, 0, 0);
    public void HandleButton01() => HandleButtonClick(_tictactoeButton01, 0, 1);
    public void HandleButton02() => HandleButtonClick(_tictactoeButton02, 0, 2);
    public void HandleButton10() => HandleButtonClick(_tictactoeButton10, 1, 0);
    public void HandleButton11() => HandleButtonClick(_tictactoeButton11, 1, 1);
    public void HandleButton12() => HandleButtonClick(_tictactoeButton12, 1, 2);
    public void HandleButton20() => HandleButtonClick(_tictactoeButton20, 2, 0);
    public void HandleButton21() => HandleButtonClick(_tictactoeButton21, 2, 1);
    public void HandleButton22() => HandleButtonClick(_tictactoeButton22, 2, 2);

private bool IsPlayersTurn()
{
    return (_currentPlayer == Player.X && _playersTurn == PlayerNumbers.Player_1) ||
           (_currentPlayer == Player.O && _playersTurn == PlayerNumbers.Player_2);
}

    public void HandleButtonClick(GameObject button, int x, int y)
{
    // Check if it's the current player's turn
    if (!IsPlayersTurn())
    {
        Debug.Log("It's not your turn! Fetching updated game data...");
        StartCoroutine(FetchGameData());
        return;
    }

    // Check if the cell is already occupied
    if (_currentPlay[x, y] != Player.None)
    {
        Debug.Log("This cell is already occupied!");
        return;
    }

    // Update the local game state
    _currentPlay[x, y] = _currentPlayer;

    // Display the move
    SynchronizeDisplay();

    // Check if there's a winner
    if (CheckWinner())
    {
        Debug.Log($"{_currentPlayer} wins!");
        StartCoroutine(ResetBoardAfterDelay());
        _eventSystem.enabled = false;
    }
    else if (IsBoardFull())
    {
        Debug.Log("Draw!");
        StartCoroutine(ResetBoardAfterDelay());
        _eventSystem.enabled = false;
    }
    else
    {
        // Change the turn and update game data on the server
        //ChangePlayer();
        StartCoroutine(UpdateGameData());
    }
}



    private void ChangePlayer()
    {
        _currentPlayer = (_currentPlayer == Player.X) ? Player.O : Player.X;
    }

    private bool CheckWinner()
    {
        for (int i = 0; i < 3; i++)
        {
            // Check rows and columns
            if ((_currentPlay[i, 0] == _currentPlayer && _currentPlay[i, 1] == _currentPlayer && _currentPlay[i, 2] == _currentPlayer) ||
                (_currentPlay[0, i] == _currentPlayer && _currentPlay[1, i] == _currentPlayer && _currentPlay[2, i] == _currentPlayer))
            {
                return true;
            }
        }

        // Check diagonals
        if ((_currentPlay[0, 0] == _currentPlayer && _currentPlay[1, 1] == _currentPlayer && _currentPlay[2, 2] == _currentPlayer) ||
            (_currentPlay[0, 2] == _currentPlayer && _currentPlay[1, 1] == _currentPlayer && _currentPlay[2, 0] == _currentPlayer))
        {
            return true;
        }

        return false;
    }

    private bool IsBoardFull()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (_currentPlay[x, y] == Player.None)
                {
                    return false;
                }
            }
        }
        _eventSystem.enabled = false;
        return true;
    }

    private IEnumerator ResetBoardAfterDelay()
    {
        yield return new WaitForSeconds(5);

        // Destroy all spawned objects
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (_spawnedObjects[x, y] != null)
                {
                    Destroy(_spawnedObjects[x, y]);
                    _spawnedObjects[x, y] = null;
                    _currentPlay[x, y] = Player.None;
                }
            }
        }

        _round++;
        ChangePlayer();
        _eventSystem.enabled = true;
    }

    private void SynchronizeDisplay()
{
    for (int x = 0; x < 3; x++)
    {
        for (int y = 0; y < 3; y++)
        {
            Player expectedPlayer = _currentPlay[x, y];
            GameObject currentObject = _spawnedObjects[x, y];

            if (expectedPlayer == Player.None && currentObject != null)
            {
                // Remove the object if no player is expected here
                Destroy(currentObject);
                _spawnedObjects[x, y] = null;
            }
            else if (expectedPlayer == Player.X && (currentObject == null || currentObject.tag != "X"))
            {
                if (currentObject != null) Destroy(currentObject);
                _spawnedObjects[x, y] = Instantiate(_X, GetButtonPosition(x, y), Quaternion.identity);
            }
            else if (expectedPlayer == Player.O && (currentObject == null || currentObject.tag != "O"))
            {
                if (currentObject != null) Destroy(currentObject);
                _spawnedObjects[x, y] = Instantiate(_O, GetButtonPosition(x, y), Quaternion.identity);
            }
        }
    }

    // Provide feedback on the current player's turn
    Debug.Log($"It's {_playersTurn}'s turn ({_currentPlayer}).");
}

    private Vector3 GetButtonPosition(int x, int y)
    {
        if (x == 0 && y == 0) return _tictactoeButton00.transform.position;
        if (x == 0 && y == 1) return _tictactoeButton01.transform.position;
        if (x == 0 && y == 2) return _tictactoeButton02.transform.position;
        if (x == 1 && y == 0) return _tictactoeButton10.transform.position;
        if (x == 1 && y == 1) return _tictactoeButton11.transform.position;
        if (x == 1 && y == 2) return _tictactoeButton12.transform.position;
        if (x == 2 && y == 0) return _tictactoeButton20.transform.position;
        if (x == 2 && y == 1) return _tictactoeButton21.transform.position;
        if (x == 2 && y == 2) return _tictactoeButton22.transform.position;

       // Debug.Log($"{x}{y}");

        return Vector3.zero;
    }

    private void _debug_currentPosition()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Debug.Log($"{i}{j} - {_currentPlay[i, j]}");
            }
        }
    }

////////////////Fetch game data///////////////
    private IEnumerator FetchGameData()
    {
        string nickname = PlayerPrefs.GetString("Nickname");
        string password = PlayerPrefs.GetString("Password");
        string url = $"https://ukfig2.sk/ukfIG2_Piskvorky/getGame.php?nickname={nickname}&password={password}&gameId={_gameId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                ProcessGameData(webRequest.downloadHandler.text);
            }
        }
    }
private void ProcessGameData(string jsonData)
{/*
    

    

    // Manually parse the game JSON string
    if (!string.IsNullOrEmpty(gameData.game))
    {
        Game game = JsonUtility.FromJson<Game>(gameData.game);
        if (game != null && game.board != null)
        {
            Debug.Log("Board data found:");
            PrintBoard(game.board);

            // Update game state using the board data
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    Debug.Log($"Board[{x}][{y}] = {game.board[x].value[y]}");
                    switch (game.board[x].value[y])
                    {
                        case "X":
                            _currentPlay[x, y] = Player.X;
                            break;
                        case "O":
                            _currentPlay[x, y] = Player.O;
                            break;
                        default:
                            _currentPlay[x, y] = Player.None;
                            break;
                    }
                }
            }
            _debug_currentPosition();
            SynchronizeDisplay();
        }
        else
        {
            Debug.LogError("Board data is null");
        }
    }
    else
    {
        Debug.LogError("Game data is null");
        InitializeBoard();
    }

    // Update round and scores
    _round = gameData.round;
    _player1Score = gameData.player_1_score;
    _player2Score = gameData.player_2_score;

    // Determine whose turn it is
    if (gameData.player_turn == "player_1")
    {
        _playersTurn = PlayerNumbers.Player_1;
        Debug.Log("Player 1's turn.");
    }
    else if (gameData.player_turn == "player_2")
    {
        _playersTurn = PlayerNumbers.Player_2;
        Debug.Log("Player 2's turn.");
    }
}

private void PrintBoard(Board[] board)
{
    if (board == null)
    {
        Debug.LogError("Board is null");
        return;
    }

    foreach (var row in board)
    {
        foreach (var value in row.value)
        {
            Debug.Log($"Value: {value}");
        }
    }
}
*/
    Debug.Log(jsonData);
    GameData data = JsonConvert.DeserializeObject<GameData>(jsonData);

    int currentPlayerId = PlayerPrefs.GetInt("Id");
    Debug.Log(data);

    // Convert player IDs to integers
    int player1Id = int.Parse(data.player_1);
    int player2Id = int.Parse(data.player_2);

    // Map player roles based on IDs
    if (player1Id == currentPlayerId)
    {
        _player1 = Player.X;
        _player2 = Player.O;
        _currentPlayer = Player.X;
    }
    else if (player2Id == currentPlayerId)
    {
        _player1 = Player.O;
        _player2 = Player.X;
        _currentPlayer = Player.O;
    }
    else
    {
        Debug.LogError("Current player is not part of this game!");
        return;
    }

    Debug.Log("Current player is " + _currentPlayer);

try
{
    // Extract the board
    List<List<string>> board = data.game.board;

    // Debug the board and update _currentPlay
    for (int i = 0; i < board.Count; i++)
    {
        for (int j = 0; j < board[i].Count; j++)
        {
            // Log the board value
            Debug.Log($"board[{i}][{j}] = {board[i][j]}");

            // Update _currentPlay based on board value
            switch (board[i][j])
            {
                case "X":
                    _currentPlay[i, j] = Player.X;
                    break;
                case "O":
                    _currentPlay[i, j] = Player.O;
                    break;
                case "None":
                    _currentPlay[i, j] = Player.None;
                    break;
                default:
                    Debug.LogWarning($"Unexpected board value at [{i}][{j}]: {board[i][j]}");
                    break;
            }

            // Optionally log _currentPlay for verification
            Debug.Log($"_currentPlay[{i},{j}] = {_currentPlay[i, j]}");
        }
    }
}
catch (System.Exception ex)
{
    Debug.LogError($"Error processing game data: {ex.Message}");
}

    SynchronizeDisplay();
    _round = int.Parse(data.round);
    _player1Score = int.Parse(data.player_1_score);
    _player2Score = int.Parse(data.player_2_score);

    // Determine whose turn it is
    if (data.player_turn == "player_1")
    {
        _playersTurn = PlayerNumbers.Player_1;
        Debug.Log("Player 1's turn.");
    }
    else if (data.player_turn == "player_2")
    {
        _playersTurn = PlayerNumbers.Player_2;
        Debug.Log("Player 2's turn.");
    }
}
    
[System.Serializable]
public class GameData
{
    public string id { get; set; }
    public string player_1 { get; set; }
    public string player_2 { get; set; }
    public Game game { get; set; }
    public string created_at { get; set; }
    public string updated_at { get; set; }
    public string status { get; set; }
    public string player_turn { get; set; }
    public string player_1_score { get; set; }
    public string player_2_score { get; set; }
    public string round { get; set; }
}

[System.Serializable]
public class Game
{
    public List<List<string>> board { get; set; }
}
//////////////////////////////////////////////
   private IEnumerator UpdateGameData()
{
    string nickname = PlayerPrefs.GetString("Nickname");
    string password = PlayerPrefs.GetString("Password");

    string gameDataJson = ConvertCurrentPlayToStandardJson();
    Debug.Log(gameDataJson); // Verify the serialized JSON

    WWWForm form = new WWWForm();
    form.AddField("nickname", nickname);
    form.AddField("password", password);
    form.AddField("gameId", _gameId);
    form.AddField("game", gameDataJson);
    form.AddField("round", _round);
    form.AddField("player_1_score", _player1Score);
    form.AddField("player_2_score", _player2Score);
    form.AddField("player_turn", _currentPlayer == Player.X ? "player_2" : "player_1");
    form.AddField("status", "changed");

    using (UnityWebRequest webRequest = UnityWebRequest.Post("https://ukfig2.sk/ukfIG2_Piskvorky/updateGame.php", form))
    {
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(webRequest.error);
        }
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
            Debug.Log("Game data updated successfully.");
        }
    }
}



private IEnumerator HasGameChanged()
{
    Debug.Log("Game should change hasgamechanged");
    string nickname = PlayerPrefs.GetString("nickname");
    string password = PlayerPrefs.GetString("password");
    string url = $"https://ukfig2.sk/ukfIG2_Piskvorky/checkGameStatus.php?nickname={nickname}&password={password}&gameId={_gameId}";

    using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
    {
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(webRequest.error);
        }
        else
        {
            string jsonResponse = webRequest.downloadHandler.text;
            GameStatusData statusData = JsonUtility.FromJson<GameStatusData>(jsonResponse);

            if (statusData.status == "changed")
            {
                StartCoroutine(FetchGameData());
            }
        }
    }
}

[System.Serializable]
private class GameStatusData
{
    public string status;
}






private string ConvertCurrentPlayToStandardJson()
{
    string json = "{ \"board\": [\n";

    for (int x = 0; x < 3; x++)
    {
        json += "  [ ";
        for (int y = 0; y < 3; y++)
        {
            string value = _currentPlay[x, y] == Player.None ? "None" : _currentPlay[x, y].ToString();
            json += $"\"{value}\"";
            if (y < 2) json += ", "; // Add a comma if it's not the last element in the row
        }
        json += " ]";
        if (x < 2) json += ",\n"; // Add a comma if it's not the last row
    }

    json += "\n] }";
    return json;
}

}

