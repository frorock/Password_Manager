using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Password.Models;
using Password.Services;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using Xamarin.Essentials;
using System.Linq;

namespace Password.ViewModels
{
    public class ModifyPopupPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Credential> Credentials { get; set; } = new ObservableCollection<Credential>();

        private Credential_DAO _credentialDAO = new Credential_DAO();
        private UserData_DAO _userDataDAO = new UserData_DAO();

        public Command EditAppCommand { get; }
        public Command ClosePopupCommand { get; }

        private string _passwordApp;
        public string PasswordApp
        {
            get => _passwordApp;
            set
            {
                _passwordApp = value;
                PasswordStrength = EvaluatePasswordStrength.CheckStrength(value);
                OnPropertyChanged();
            }
        }

        private EvaluatePasswordStrength.PasswordStrength _passwordStrength;
        public EvaluatePasswordStrength.PasswordStrength PasswordStrength
        {
            get => _passwordStrength;
            set
            {
                _passwordStrength = value;
                OnPropertyChanged();
            }
        }

        private string _application;
        public string Application
        {
            get => _application;
            set
            {
                _application = value;
                OnPropertyChanged();
            }
        }

        private string _userNameApp;
        public string UserNameApp
        {
            get => _userNameApp;
            set
            {
                _userNameApp = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _url;
        public string URL
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        public string _credentialId;
        private AboutViewModel _aboutViewModel;


        public ModifyPopupPageViewModel()
        {
            EditAppCommand = new Command(ExecuteEditAppCommand);
            ClosePopupCommand = new Command(ExecuteClosePopupCommand);
        }

        // Méthode d'initialisation
        public async Task InitializeAsync(string credentialId)
        {
            _credentialId = credentialId;
            await LoadCredentialsFromDatabase();
            LoadCredentialData();
        }

        private async Task LoadCredentialsFromDatabase()
        {
            var allCredentials = await _credentialDAO.GetAllCredentials();
            foreach (var cred in allCredentials)
            {
                Credentials.Add(cred);
            }
        }

        private void LoadCredentialData()
        {
            var credential = Credentials.FirstOrDefault(c => c.Id.ToString() == _credentialId);
            var userBase64 = Preferences.Get("IV", "");
            if (credential != null && !string.IsNullOrEmpty(userBase64))
            {
                Application = credential.Application;
                UserNameApp = credential.UserNameApp;
                PasswordApp = EncryptionService.Decrypt(credential.PasswordApp, userBase64);
                Email = credential.Email;
                URL = credential.URL;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ExecuteEditAppCommand()
        {
            var encryptedPassword = EncryptionService.Encrypt(PasswordApp);
            string userId = Preferences.Get("IdUser", "");
            var updatedCredential = new Credential
            {
                Id = Convert.ToInt32(_credentialId), 
                Application = Application,
                UserNameApp = UserNameApp,
                PasswordApp = encryptedPassword,
                Email = Email,
                URL = URL,
                UserId = userId,
                LastModifiedDate = DateTime.Now.ToString()
            };

            await _credentialDAO.EditCredential(updatedCredential);
            await LoadCredentialsFromDatabase();

            await PopupNavigation.Instance.PopAsync();


        }

        private async void ExecuteClosePopupCommand()
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
