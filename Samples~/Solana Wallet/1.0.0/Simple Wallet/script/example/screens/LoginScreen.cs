using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System;
using Solana.Unity.Wallet;

namespace Solana.Unity.SDK.Example
{
    [RequireComponent(typeof(TxtLoader))]
    public class LoginScreen : SimpleScreen
    {
        public TMP_InputField _passwordInputField;
        public TextMeshProUGUI _passwordText;
        public Button _createNewWalletBtn;
        public Button _loginToWalletBtn;
        public Button _loginBtn;
        public Button _tryAgainBtn;
        public Button _backBtn;
        public TextMeshProUGUI _messageTxt;

        public List<GameObject> _panels = new List<GameObject>();
        public SimpleScreenManager parentManager;

        private string _password;
        private string _mnemonics;
        private string _path;
        private string _pubKey;
        private string[] _paths;
        private string _loadedKey;

        private void OnEnable()
        {
            _passwordInputField.text = String.Empty;
        }

        private void Start()
        {
            _passwordText.text = "";

            SwitchButtons("Login");

            if(_loginToWalletBtn != null)
            {
                _loginToWalletBtn.onClick.AddListener(() =>
                {
                    SwitchPanels(1);
                });
            }
 
            if(_backBtn != null)
            {
                _backBtn.onClick.AddListener(() =>
                {
                    SwitchPanels(0);
                });
            }

            if(_createNewWalletBtn != null)
            {
                _createNewWalletBtn.onClick.AddListener(() =>
                {
                    SimpleWallet.Instance.Logout();
                    manager.ShowScreen(this, "generate_screen");
                    SwitchPanels(0);
                });
            }

            _passwordInputField.onSubmit.AddListener(delegate { LoginChecker(); });

            _loginBtn.onClick.AddListener(LoginChecker);
            _tryAgainBtn.onClick.AddListener(() => { SwitchButtons("Login"); });  

            if(_messageTxt != null)
                _messageTxt.gameObject.SetActive(false);
        }

        private async void LoginChecker()
        {
            string password = _passwordInputField.text;
            Account account = await SimpleWallet.Instance.Login(password);
            if (account != null)
            {
                //SimpleWallet.instance.GenerateWalletWithMenmonic(_simpleWallet.LoadPlayerPrefs(_simpleWallet.MnemonicsKey));
                //MainThreadDispatcher.Instance().Enqueue(() => { _simpleWallet.StartWebSocketConnection(); }); 
                manager.ShowScreen(this, "wallet_screen");
                gameObject.SetActive(false);
            }
            else
            {
                SwitchButtons("TryAgain");
            }
        }

        private void SwitchButtons(string btnName)
        {
            _loginBtn.gameObject.SetActive(false);
            _tryAgainBtn.gameObject.SetActive(false);

            switch (btnName)
            {
                case "Login":
                    _loginBtn.gameObject.SetActive(true);
                    _passwordInputField.gameObject.SetActive(true);
                    return;
                case "TryAgain":
                    _tryAgainBtn.gameObject.SetActive(true);
                    _passwordInputField.text = string.Empty;
                    _passwordInputField.gameObject.SetActive(false);
                    return;
            }
        }

        private void SwitchPanels(int order)
        {
            _passwordInputField.text = String.Empty;

            foreach (GameObject panel in _panels)
            {
                if (panel.transform.GetSiblingIndex() == order)
                    panel.SetActive(true);
                else
                    panel.SetActive(false);
            }
        }

        //
        // WebGL
        //
        [DllImport("__Internal")]
        private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

        // Called from browser
        public void OnFileUpload(string url)
        {
            StartCoroutine(OutputRoutine(url));
        }
        private IEnumerator OutputRoutine(string url)
        {
            var loader = new WWW(url);
            yield return loader;
            _loadedKey = loader.text;

            //LoginWithPrivateKeyCallback();
        }
    }
}

