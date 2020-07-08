using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string _token;
    public string Token
    {
        get { return _token; }
        set { _token = value; }
    }

    private string _id;
    public string Id
    {
        get { return _id; }
        set { _id = value; }
    }

    private string _nickname;
    public string Nickname
    {
        get { return _nickname; }
        set { _nickname = value; }
    }

    private string _date;
    public string Date
    {
        get { return _date; }
        set { _date = value; }
    }
}

