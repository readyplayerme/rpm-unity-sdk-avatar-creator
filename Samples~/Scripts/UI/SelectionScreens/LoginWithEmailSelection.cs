using ReadyPlayerMe;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;
using UnityEngine.UI;

public class LoginWithEmailSelection : State
{
    [SerializeField] private Button sendActivationCodeButton;
    [SerializeField] private Button haveCodeButton;
    [SerializeField] private Button changeEmailButton;
    [SerializeField] private Button loginButton;

    [SerializeField] private InputField emailField;
    [SerializeField] private InputField codeField;

    [SerializeField] private GameObject emailPanel;
    [SerializeField] private GameObject codePanel;

    public override StateType StateType => StateType.LoginWithCodeFromEmail;
    public override StateType NextState => StateType.BodyTypeSelection;

    private void OnEnable()
    {
        sendActivationCodeButton.onClick.AddListener(OnSendActivationCode);
        haveCodeButton.onClick.AddListener(OnHaveCodeButton);
        changeEmailButton.onClick.AddListener(OnChangeEmail);
        loginButton.onClick.AddListener(OnLogin);
    }

    private void OnDisable()
    {
        sendActivationCodeButton.onClick.RemoveListener(OnSendActivationCode);
        haveCodeButton.onClick.RemoveListener(OnHaveCodeButton);
        changeEmailButton.onClick.RemoveListener(OnChangeEmail);
        loginButton.onClick.RemoveListener(OnLogin);
    }

    private void OnSendActivationCode()
    {
        AuthManager.SendEmailCode(emailField.text);
        OnHaveCodeButton();
    }

    private void OnHaveCodeButton()
    {
        emailPanel.SetActive(false);
        codePanel.SetActive(true);
    }

    private void OnChangeEmail()
    {
        emailPanel.SetActive(true);
        codePanel.SetActive(false);
    }

    private async void OnLogin()
    {
        LoadingManager.EnableLoading("Signing In");

        AuthManager.OnSignInError += OnSignInError;

        if (await AuthManager.LoginWithCode(codeField.text))
        {
            Debug.Log("How");
            OnChangeEmail();
            LoadingManager.DisableLoading();
            StateMachine.SetState(StateType.AvatarSelection);
        }
    }

    private void OnSignInError(string error)
    {
        AuthManager.OnSignInError -= OnSignInError;
        LoadingManager.EnableLoading(error, LoadingManager.LoadingType.Popup, false);
    }
}
