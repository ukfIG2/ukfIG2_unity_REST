using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManagerMultiplayer : MonoBehaviour
{
    public enum Player { X, O, None }
    private Player[,] _currentPlay = new Player[3, 3];
    private GameObject[,] _spawnedObjects = new GameObject[3, 3]; // Tracks spawned objects
    [SerializeField] private Player _currentPlayer = Player.X;

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
    [SerializeField] private int _scoreX = 0;
    [SerializeField] private int _scoreO = 0;

    [SerializeField] private EventSystem _eventSystem;

    private void Start()
    {
        InitializeBoard();
    }

    private void Update()
    {
    }

    private void InitializeBoard()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                _currentPlay[x, y] = Player.None;
                _spawnedObjects[x, y] = null;
            }
        }
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

    public void HandleButtonClick(GameObject button, int x, int y)
    {
        if (_currentPlay[x, y] == Player.None)
        {
            if (_currentPlayer == Player.X)
            {
                _currentPlay[x, y] = Player.X;
            }
            else
            {
                _currentPlay[x, y] = Player.O;
            }

            SynchronizeDisplay();

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
                ChangePlayer();
            }
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
                    // Correct to X if it's missing or incorrect
                    if (currentObject != null) Destroy(currentObject);
                    _spawnedObjects[x, y] = Instantiate(_X, GetButtonPosition(x, y), Quaternion.identity);
                }
                else if (expectedPlayer == Player.O && (currentObject == null || currentObject.tag != "O"))
                {
                    // Correct to O if it's missing or incorrect
                    if (currentObject != null) Destroy(currentObject);
                    _spawnedObjects[x, y] = Instantiate(_O, GetButtonPosition(x, y), Quaternion.identity);
                }
            }
        }
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

}

