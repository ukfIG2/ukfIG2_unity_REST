using UnityEngine;
using MySqlConnector;
using System;

public class DatabaseManager : MonoBehaviour
{
    private const string url = "dbadmin.ukfig2.sk";
    private const string username = "infinityRunner";
    private const string password = "\"*;E|O]L??@lt0G~j'VZ,\"";
    private const string port = "3315";
    private const string databaseName = "ukfIG2_UNITY";

    void Start()
    {
        string connectionString = $"Server={url};Database={databaseName};User ID={username};Password={password};Port={port}";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Debug.Log("Connection successful!");
                GetAllFromHra();
            }
            catch (MySqlException ex)
            {
                Debug.LogError("Error: " + ex.Message);
            }
        }
    }

    public int MaxScore(string nickName)
    {
        int maxScore = -1; // Default value if nickname is not found
        string connectionString = $"Server={url};Database={databaseName};User ID={username};Password={password};Port={port}";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Debug.Log("Connection successful!");

                string query = "SELECT MAX(score) FROM Hra WHERE nickname = @nickName";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@nickName", nickName);
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    maxScore = Convert.ToInt32(result);
                }
            }
            catch (MySqlException ex)
            {
                Debug.LogError("Error: " + ex.Message);
            }
        }
        return maxScore;
    }

    public void InsertScore(int score, string playerNickname)
    {
        string connectionString = $"Server={url};Database={databaseName};User ID={username};Password={password};Port={port}";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Debug.Log("Connection successful!");

                string query = "INSERT INTO Hra (nickname, score, timeStamp) VALUES (@nickname, @score, @timeStamp)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@nickname", playerNickname);
                cmd.Parameters.AddWithValue("@score", score);
                cmd.Parameters.AddWithValue("@timeStamp", DateTime.Now);

                cmd.ExecuteNonQuery();
                Debug.Log("Score inserted successfully!");
            }
            catch (MySqlException ex)
            {
                Debug.LogError("Error: " + ex.Message);
            }
        }
    }

    public void GetAllFromHra()
    {
        string connectionString = $"Server={url};Database={databaseName};User ID={username};Password={password};Port={port}";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Debug.Log("Connection successful!");

                string query = "SELECT * FROM Hra";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string nickname = reader["nickname"].ToString();
                    int score = Convert.ToInt32(reader["score"]);
                    DateTime timeStamp = Convert.ToDateTime(reader["timeStamp"]);

                    Debug.Log($"Nickname: {nickname}, Score: {score}, Time Stamp: {timeStamp}");
                }
            }
            catch (MySqlException ex)
            {
                Debug.LogError("Error: " + ex.Message);
            }
        }
    }
}