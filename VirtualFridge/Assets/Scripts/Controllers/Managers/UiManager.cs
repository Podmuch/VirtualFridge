using UnityEngine;
using System.Collections;

public class UiManager : MonoBehaviour
{
    #region SCENE REFERENCES

    public MainScreen MainScreen;
    public LoginScreen LoginScreen;
    public LoadingScreen LoadingScreen;

    #endregion

    public AbstractScreen CurrentScreen { get; private set; }

    public void Init()
    {
        CurrentScreen = LoginScreen;
        LoginScreen.SetStateImmediately(true);
        LoadingScreen.SetStateImmediately(false);
        MainScreen.SetStateImmediately(false);
    }

    public void ChangeScreen(AbstractScreen newScreen)
    {
        if (newScreen != CurrentScreen)
        {
            CurrentScreen.Hide();
            newScreen.Show();
            CurrentScreen = newScreen;
        }
    }
}
