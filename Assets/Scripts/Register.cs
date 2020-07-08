using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Register : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField confirmPasswordInputField;
    public TMP_InputField nameInputField;
    public TMP_InputField dateInputField;

    private Player player;

    public Button registerButton;

    public Text popup;
    

    private static string httpServerAddress = "https://localhost:44310/";

    void Start()
    {
        player = FindObjectOfType<Player>();
        
    }

    public void OnRegisterButtonClick()
    {
        if (string.IsNullOrEmpty(emailInputField.text))
        {
            popup.text = "Email can't be void";

        }
        else if (string.IsNullOrEmpty(passwordInputField.text))
        {
            popup.text = "Password can't be void";

        }
        else if (passwordInputField.text != confirmPasswordInputField.text)
        {
            popup.text = "Passwords don't match";

        }
        else if (string.IsNullOrEmpty(confirmPasswordInputField.text))
        {
            popup.text = "Confirm Password can't be void";

        }
        else if (string.IsNullOrEmpty(nameInputField.text))
        {
            popup.text = "Name can't be void";

        }
        else if (string.IsNullOrEmpty(dateInputField.text))
        {
            popup.text = "Date of Birth can't be void";

        }
        else
        {

            StartCoroutine(RegisterNewUser());
        }
    }

    private IEnumerator RegisterNewUser()
    {


        yield return RegisterUser();
        popup.text = "User registered";
        yield return InitializeToken(emailInputField.text, passwordInputField.text);
        popup.text = "User with token";
        yield return GetUserId();
        
        yield return InsertPlayer();
        popup.text = "User with token";


        SceneManager.LoadScene("Login");

    }
    private IEnumerator RegisterUser()
    {
        UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "api/Account/Register", "POST");

        AspNetUserRegister newUser = new AspNetUserRegister();
        newUser.Email = emailInputField.text;
        newUser.Password = passwordInputField.text;
        newUser.ConfirmPassword = confirmPasswordInputField.text;
        newUser.Nickname = nameInputField.text;
        newUser.Date = dateInputField.text;


        string jsonData = JsonUtility.ToJson(newUser);
        Debug.Log(jsonData);
        byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);
        httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);

        httpClient.SetRequestHeader("Content-Type", "application/json");

        yield return httpClient.SendWebRequest();
        Debug.Log(httpClient.isNetworkError.ToString());
        Debug.Log(httpClient.isHttpError.ToString());
        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            popup.text = httpClient.error;
        }

        httpClient.Dispose();
    }

    private static IEnumerator InitializeToken(string email, string password)
    {
        Player player = FindObjectOfType<Player>();
        if (string.IsNullOrEmpty(player.Token))
        {
            UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "/Token", "POST");

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

    private IEnumerator GetUserId()
    {
        UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "api/Account/UserId", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            popup.text = httpClient.error;
        }
        else
        {
            player.Id = httpClient.downloadHandler.text.Replace("\"", "");
            popup.text = "User ID: " + player.Id;
            Debug.Log(player.Id);
        }

        httpClient.Dispose();
    }

    private IEnumerator InsertPlayer()
    {

        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.Id;
        player.Nickname = nameInputField.text;
        player.Date = dateInputField.text;
        playerSerializable.Nickname = player.Nickname;
        playerSerializable.Date = player.Date;
        using (UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "api/Player/RegisterPlayer", "POST"))
        {
            string playerData = JsonUtility.ToJson(playerSerializable);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(playerData);
            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);

            yield return httpClient.SendWebRequest();
        }
    }
    /*
    private void GetToken()
    {
        UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "Token", "POST");

        
        WWWForm dataToSend = new WWWForm();
        dataToSend.AddField("grant_type", "password");
        dataToSend.AddField("username", emailInputField.text);
        dataToSend.AddField("password", passwordInputField.text);

        httpClient.uploadHandler = new UploadHandlerRaw(dataToSend.data);
        httpClient.downloadHandler = new DownloadHandlerBuffer();

        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            Debug.Log(httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            AuthorizationToken authToken = JsonUtility.FromJson<AuthorizationToken>(jsonResponse);

        }
        httpClient.Dispose();

    }
    */
    

}
