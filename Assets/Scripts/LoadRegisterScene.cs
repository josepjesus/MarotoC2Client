using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadRegisterScene : MonoBehaviour
{
    public void OnClickButton()
    {
        SceneManager.LoadScene("Register");
    }
}
