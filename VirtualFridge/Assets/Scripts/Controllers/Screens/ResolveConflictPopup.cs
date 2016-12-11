using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResolveConflictPopup : AbstractScreen
{
    #region SCENE REFERENCES

    public Text LocalDataLabel;
    public Text LocalDataDetails;
    public Text ServerDataLabel;
    public Text ServerDataDetails;
    //
    public GameObject LocalButton;
    public GameObject ServerButton;
    public GameObject DontKeepButton;

    #endregion

    public ProductData SelectedProduct { get; set; }

    private ProductData localProduct;
    private ProductData serverProduct;
    private bool isAnswer = false;

    #region INPUT HANDLING

    public void OnServerDataButton()
    {
        if (isClickable)
        {
            isAnswer = true;
            SelectedProduct = serverProduct;
        }
    }

    public void OnLocalDataButton()
    {
        if (isClickable)
        {
            isAnswer = true;
            SelectedProduct = localProduct;
        }
    }

    public void OnDontKeepButton()
    {
        if (isClickable)
        {
            isAnswer = true;
            SelectedProduct = null;
        }
    }

    #endregion

    public void Init(ProductData _product, bool isServer)
    {
        isAnswer = false;
        SelectedProduct = null;
        DontKeepButton.SetActive(true);
        ServerDataLabel.gameObject.SetActive(isServer);
        ServerDataDetails.gameObject.SetActive(isServer);
        ServerButton.SetActive(isServer);
        LocalDataLabel.gameObject.SetActive(!isServer);
        LocalDataDetails.gameObject.SetActive(!isServer);
        LocalButton.SetActive(!isServer);
        if (isServer)
        {
            serverProduct = _product;
            ServerDataLabel.text = "Czy chcesz zatrzymać wpis istniejący tylko na serwerze:";
            ServerDataDetails.text = serverProduct.ToString();
        }
        else
        {
            localProduct = _product;
            LocalDataLabel.text = "Czy chcesz zatrzymać wpis istniejący tylko lokalnie:";
            LocalDataDetails.text = localProduct.ToString();
        }
    }

    public void Init(ProductData _localProduct, ProductData _serverProduct)
    {
        isAnswer = false;
        SelectedProduct = null;
        localProduct = _localProduct;
        serverProduct = _serverProduct;
        LocalDataLabel.text = "Czy chcesz zatrzymać wpis istniejący lokalnie:";
        LocalDataDetails.text = localProduct.ToString();
        ServerDataLabel.text = "Czy istniejący na serwerze:";
        ServerDataDetails.text = serverProduct.ToString();
        ServerDataLabel.gameObject.SetActive(true);
        ServerDataDetails.gameObject.SetActive(true);
        ServerButton.SetActive(true);
        LocalDataLabel.gameObject.SetActive(true);
        LocalDataDetails.gameObject.SetActive(true);
        LocalButton.SetActive(true);
        DontKeepButton.SetActive(false);
    }

    public IEnumerator WaitForAnswer()
    {
        yield return new WaitUntil(IsAnswerSelected);
        Hide();
    }

    private bool IsAnswerSelected()
    {
        return isAnswer;
    }
}