using UnityEngine;
using System.Collections;

public class LoadingScreen : AbstractScreen
{
    #region SCENE REFERENCES

    public Transform LoadingIndicator;

    #endregion

    [SerializeField] private float rotatingSpeed = 10;

    #region MONO BEHAVIOUR

    private void Update()
    {
        LoadingIndicator.Rotate(0, 0, rotatingSpeed*Time.deltaTime);
    }

    #endregion
}
