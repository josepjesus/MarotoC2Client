using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public Button loginButton;

    public Text popup;

    private string httpServerAddress = "https://localhost:44310/";

    public Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void OnLoginButtonClicked()
    {
        if (string.IsNullOrEmpty(emailInputField.text))
        {
            popup.text = "Email can't be void";

        }
        else if (string.IsNullOrEmpty(passwordInputField.text))
        {
            popup.text = "Password can't be void";

        }
        else
        {
            StartCoroutine(TryLogin());

        }
        
    }

    private IEnumerator TryLogin()
    {
        yield return InitializeToken(emailInputField.text, passwordInputField.text);
        yield return GetPlayerInfo();

        SceneManager.LoadScene("Challenge 2");
    }


    IEnumerator InitializeToken(string email, string password)
    {
        Player player = FindObjectOfType<Player>();
        if (string.IsNullOrEmpty(player.Token))
        {
            UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "Token", "POST");

            
            WWWForm dataToSend = new WWWForm();
            dataToSend.AddField("grant_type", "password");
            dataToSend.AddField("username", email);
            dataToSend.AddField("password", password);

            httpClient.uploadHandler = new UploadHandlerRaw(dataToSend.data);
            httpClient.downloadHandler = new DownloadHandlerBuffer();

            httpClient.SetRequestHeader("Accept", "application/json");

            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("Helper > InitToken: " + httpClient.error);
            }
            else
            {
                string jsonResponse = httpClient.downloadHandler.text;
                AuthorizationToken authToken = JsonUtility.FromJson<AuthorizationToken>(jsonResponse);
                player.Token = authToken.access_token;
            }
            httpClient.Dispose();
        }
    }

    IEnumerator GetPlayerInfo()
    {
        Player player = FindObjectOfType<Player>();
        UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "api/Player/Info", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetPlayerInfo: " + httpClient.error);
        }
        else
        {
            PlayerSerializable playerSerializable = JsonUtility.FromJson<PlayerSerializable>(httpClient.downloadHandler.text);
            player.Id = playerSerializable.Id;
            player.Nickname = playerSerializable.Nickname;
            player.Date = playerSerializable.Date;
        }

        httpClient.Dispose();
    }

    
}
