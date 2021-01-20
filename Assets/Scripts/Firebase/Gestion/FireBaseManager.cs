using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireBaseManager : Singleton<FireBaseManager>
{
    private DatabaseReference m_drRootReference;
    private FirebaseAuth m_faAuth;

    public FirebaseAuth Auth => m_faAuth;
    public DatabaseReference Database => m_drRootReference;
                
    protected override void Awake()
    {
        base.Awake();

        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri("https://footwar-c0754.firebaseio.com/");

        m_faAuth = FirebaseAuth.DefaultInstance;
        m_drRootReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    static public AuthError GetAuthError(AggregateException exceptions)
    {
        foreach (Exception exception in exceptions.Flatten().InnerExceptions)
        {
            if (exception is FirebaseException firebaseException) return (AuthError) firebaseException.ErrorCode;
        }

        return 0;
    }
}