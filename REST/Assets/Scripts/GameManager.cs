using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string[,] tictactoeArray = new string[,] { 
        { "?", "?", "?" }, 
        { "?", "?", "?" }, 
        { "?", "?", "?" } 
    };

    // Buttony pre mriežku
    public GameObject tictactoeButton00;
    public GameObject tictactoeButton01;
    public GameObject tictactoeButton02;

    public GameObject tictactoeButton10;
    public GameObject tictactoeButton11;
    public GameObject tictactoeButton12;

    public GameObject tictactoeButton20;
    public GameObject tictactoeButton21;
    public GameObject tictactoeButton22;

    // Značky X a O
    public GameObject X;
    public GameObject O;

    private int round = 0;

    void Start()
    {
        // Ak je to potrebné, môžeš inštancovať objekty tu, 
        // napr. ak `tictactoeButtonXX` nie sú priradené cez editor.
        // Example:
        // tictactoeButton00 = new GameObject("Button00");
        // Nastav transformácie alebo komponenty, ak je potrebné.
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Príklad pre každé políčko
                HandleButtonClick(hit, tictactoeButton00, 0, 0);
                HandleButtonClick(hit, tictactoeButton01, 0, 1);
                HandleButtonClick(hit, tictactoeButton02, 0, 2);

                HandleButtonClick(hit, tictactoeButton10, 1, 0);
                HandleButtonClick(hit, tictactoeButton11, 1, 1);
                HandleButtonClick(hit, tictactoeButton12, 1, 2);

                HandleButtonClick(hit, tictactoeButton20, 2, 0);
                HandleButtonClick(hit, tictactoeButton21, 2, 1);
                HandleButtonClick(hit, tictactoeButton22, 2, 2);
            }
        }
    }

    void HandleButtonClick(RaycastHit hit, GameObject button, int x, int y)
    {
        if (hit.transform.name == button.name && tictactoeArray[x, y] == "?")
        {
            // Striedanie X a O podľa kola
            if (round % 2 == 0)
        {
            tictactoeArray[x, y] = "X";
            Instantiate(X, button.transform.position, Quaternion.identity);
        }
            else
                {
                    tictactoeArray[x, y] = "O";
                    Instantiate(O, button.transform.position, Quaternion.identity); // Upravené na `O`
                }
                round++;
            checkWinner();
        }
    }

    public void checkWinner()
    {
        if ((tictactoeArray[0, 0] == "O" && tictactoeArray[0, 1] == "O" && tictactoeArray[0, 2] == "O") ||
            (tictactoeArray[1, 0] == "O" && tictactoeArray[1, 1] == "O" && tictactoeArray[1, 2] == "O") ||
            (tictactoeArray[2, 0] == "O" && tictactoeArray[2, 1] == "O" && tictactoeArray[2, 2] == "O") ||
            (tictactoeArray[0, 0] == "O" && tictactoeArray[1, 0] == "O" && tictactoeArray[2, 0] == "O") ||
            (tictactoeArray[0, 1] == "O" && tictactoeArray[1, 1] == "O" && tictactoeArray[2, 1] == "O") ||
            (tictactoeArray[0, 2] == "O" && tictactoeArray[1, 2] == "O" && tictactoeArray[2, 2] == "O") ||
            (tictactoeArray[0, 0] == "O" && tictactoeArray[1, 1] == "O" && tictactoeArray[2, 2] == "O") ||
            (tictactoeArray[0, 2] == "O" && tictactoeArray[1, 1] == "O" && tictactoeArray[2, 0] == "O"))
        {
            Debug.Log("O wins");
        }
        else if ((tictactoeArray[0, 0] == "X" && tictactoeArray[0, 1] == "X" && tictactoeArray[0, 2] == "X") ||
                 (tictactoeArray[1, 0] == "X" && tictactoeArray[1, 1] == "X" && tictactoeArray[1, 2] == "X") ||
                 (tictactoeArray[2, 0] == "X" && tictactoeArray[2, 1] == "X" && tictactoeArray[2, 2] == "X") ||
                 (tictactoeArray[0, 0] == "X" && tictactoeArray[1, 0] == "X" && tictactoeArray[2, 0] == "X") ||
                 (tictactoeArray[0, 1] == "X" && tictactoeArray[1, 1] == "X" && tictactoeArray[2, 1] == "X") ||
                 (tictactoeArray[0, 2] == "X" && tictactoeArray[1, 2] == "X" && tictactoeArray[2, 2] == "X") ||
                 (tictactoeArray[0, 0] == "X" && tictactoeArray[1, 1] == "X" && tictactoeArray[2, 2] == "X") ||
                 (tictactoeArray[0, 2] == "X" && tictactoeArray[1, 1] == "X" && tictactoeArray[2, 0] == "X"))
        {
            Debug.Log("X wins");
        }
    }
}
