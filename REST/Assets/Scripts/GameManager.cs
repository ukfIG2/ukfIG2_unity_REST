using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Player { X, O, None }
    private Player[,] _tictactoeArray = new Player[3, 3];
    [SerializeField] private Player _currentPlayer = Player.X;

    // Buttons for the grid
    [SerializeField] private GameObject _tictactoeButton00;
    [SerializeField] private GameObject _tictactoeButton01;
    [SerializeField] private GameObject _tictactoeButton02;

    [SerializeField] private GameObject _tictactoeButton10;
    [SerializeField] private GameObject _tictactoeButton11;
    [SerializeField] private GameObject _tictactoeButton12;

    [SerializeField] private GameObject _tictactoeButton20;
    [SerializeField] private GameObject _tictactoeButton21;
    [SerializeField] private GameObject _tictactoeButton22;

    // Marks X and O
    [SerializeField] private GameObject _X;
    [SerializeField] private GameObject _O;

    [SerializeField] private int _round = 0;
    [SerializeField] private int _scoreX = 0;
    [SerializeField] private int _scoreO = 0;

    private bool _roundENDed= false;
    private void Start()
    {
        InitializeBoard();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !_roundENDed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Example for each cell
                HandleButtonClick(hit, _tictactoeButton00, 0, 0);
                HandleButtonClick(hit, _tictactoeButton01, 0, 1);
                HandleButtonClick(hit, _tictactoeButton02, 0, 2);

                HandleButtonClick(hit, _tictactoeButton10, 1, 0);
                HandleButtonClick(hit, _tictactoeButton11, 1, 1);
                HandleButtonClick(hit, _tictactoeButton12, 1, 2);

                HandleButtonClick(hit, _tictactoeButton20, 2, 0);
                HandleButtonClick(hit, _tictactoeButton21, 2, 1);
                HandleButtonClick(hit, _tictactoeButton22, 2, 2);
            }
        }
    }

    private void HandleButtonClick(RaycastHit hit, GameObject button, int x, int y)
    {
        if (hit.transform.name == button.name && _tictactoeArray[x, y] == Player.None)
        {
            if (_currentPlayer == Player.X)
            {
                _tictactoeArray[x, y] = Player.X;
                Instantiate(_X, button.transform.position, Quaternion.Euler(90, 0, 0));
            }
            else
            {
                _tictactoeArray[x, y] = Player.O;
                Instantiate(_O, button.transform.position, Quaternion.identity);
            }

            if (CheckWinner())
            {
                if (_currentPlayer == Player.X)
                {
                    _scoreX++;
                    _roundENDed = true;
                    Debug.Log("X wins");
                }
                else
                {
                    _scoreO++;
                    _roundENDed = true;
                    Debug.Log("O wins");
                }
                StartCoroutine(ResetBoardAfterDelay());
            }
            else if (IsBoardFull())
            {
                Debug.Log("Draw");
                _roundENDed = true; 
                StartCoroutine(ResetBoardAfterDelay());
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
        PrintTicTacToeArray(); // Add this line to print the array

    }

    private bool CheckWinner()
    {
        // Check rows, columns, and diagonals for a win
        for (int i = 0; i < 3; i++)
        {
            if ((_tictactoeArray[i, 0] == _currentPlayer && _tictactoeArray[i, 1] == _currentPlayer && _tictactoeArray[i, 2] == _currentPlayer) ||
                (_tictactoeArray[0, i] == _currentPlayer && _tictactoeArray[1, i] == _currentPlayer && _tictactoeArray[2, i] == _currentPlayer))
            {
                return true;
            }
        }
        if ((_tictactoeArray[0, 0] == _currentPlayer && _tictactoeArray[1, 1] == _currentPlayer && _tictactoeArray[2, 2] == _currentPlayer) ||
            (_tictactoeArray[0, 2] == _currentPlayer && _tictactoeArray[1, 1] == _currentPlayer && _tictactoeArray[2, 0] == _currentPlayer))
        {
            return true;
        }
        return false;
    }

    private bool IsBoardFull()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (_tictactoeArray[i, j] == Player.None)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private IEnumerator ResetBoardAfterDelay()
    {
        yield return new WaitForSeconds(5); // Wait for 5 seconds before resetting the board

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                _tictactoeArray[i, j] = Player.None;
            }
        }

        // Destroy all instantiated pieces
        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("piece"))
        {
            Destroy(piece);
        }

        _round++;
        _currentPlayer = Player.X;
        _roundENDed = false;
    }

    private void InitializeBoard()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                _tictactoeArray[i, j] = Player.None;
            }
        }
    }

    private void PrintTicTacToeArray()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Debug.Log($"[{i},{j}] = {_tictactoeArray[i, j]}");
            }
        }
    }

}