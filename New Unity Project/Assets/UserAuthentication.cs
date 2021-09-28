using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserAuthentication : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInputField;
    
    [SerializeField] TMP_InputField passwordInputField;
    private string AuthKey = "AIzaSyDUB6iPUfCsVjku1bsTb8PTehmj1dKY3_s";

    public void SignUpUserButton()
    {
        SignUpUser(emailInputField.text, passwordInputField.text);
    }
    public void SignInUserButton()
    {
        SignInUser(emailInputField.text, passwordInputField.text);
    }

    public void SignUpUser(string email, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<PlayerData>("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + AuthKey, userData).Then(
            response =>
            {
                
                Data.idToken = response.idToken;
                Data.localId = response.localId;
                response.level = 0;
                response.xp = 0;
                Data.SaveToDatabase(response);
                Data.LoadFromDataBase();
                Launcher.instance.StartAfterLogin();

            }).Catch(error =>
            {
                Debug.Log(error);
            });
    }

    public void SignInUser(string email,  string password)
    {
        
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<PlayerData>("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + AuthKey, userData).Then(
            response =>
            {
                Debug.Log(response.idToken + response.localId + response.username);
                
                Data.idToken = response.idToken;
                Data.localId = response.localId;
                Data.username = response.username;
                Data.LoadFromDataBase();


                Launcher.instance.StartAfterLogin();

            }).Catch(error =>
            {
                Debug.Log(error);
            });
    }

    private void GetUsername()
    {
        RestClient.Get<PlayerData> ("https://multiplayer-game-258e6-default-rtdb.firebaseio.com/users/" + "/" + Data.localId + ".json?auth=" + Data.idToken).Then(response =>
        {
            Data.playerData.username = response.username;
        });
    }

}
