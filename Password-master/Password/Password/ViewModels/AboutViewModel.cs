using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Password.Models;
using Password.Services;
using Rg.Plugins.Popup.Services;
using Password.Views;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Essentials;
using System;

namespace Password.ViewModels
{
    public class AboutViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Credential> Credentials { get; set; } = new ObservableCollection<Credential>();

        private Credential_DAO _credentialDAO;
        private UserData_DAO _userDataDAO;
        public Command AddCommand { get; }
        public Command ModifyCommand { get; }
        public Command DeleteCommand { get; }
        public Command TogglePasswordVisibilityCommand { get; }
        private string _passwordAppVisible = "*****";
        public string PasswordAppVisible
        {
            get => _passwordAppVisible;
            set
            {
                _passwordAppVisible = value;
                OnPropertyChanged();
            }
        }
        private Credential _currentlyVisibleCredential;
        public Credential CurrentlyVisibleCredential
        {
            get => _currentlyVisibleCredential;
            set
            {
                _currentlyVisibleCredential = value;
                OnPropertyChanged();
            }
        }

        private string _countdown;
        public string Countdown
        {
            get => _countdown;
            set
            {
                _countdown = value;
                OnPropertyChanged();
            }
        }




        private string _eyeIcon = "\uf06e";
        public string EyeIcon
        {
            get
            {
                if (int.TryParse(_countdown, out int countdownNumber) && countdownNumber >= 0 && countdownNumber <= 5)
                {
                    return countdownNumber.ToString();
                }
                return _eyeIcon;
            }
            set
            {
                _eyeIcon = value;
                OnPropertyChanged();
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchCredentials();
            }
        }
        private bool _canTogglePassword = true;
        public bool CanTogglePassword
        {
            get => _canTogglePassword;
            set
            {
                _canTogglePassword = value;
                OnPropertyChanged();
            }
        }

        public AboutViewModel()
        {
            AddCommand = new Command(ExecuteAddCommand);
            ModifyCommand = new Command<object>(ExecuteModifyCommand);
            DeleteCommand = new Command(ExecuteDeleteCommand);
            TogglePasswordVisibilityCommand = new Command<object>(ExecuteTogglePasswordVisibilityCommand);
            _credentialDAO = new Credential_DAO();
            _userDataDAO = new UserData_DAO();
            RefreshData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ExecuteAddCommand()
        {
            var popup = new PopupAdd();
            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void ExecuteModifyCommand(object selectedItem)
        {
            if (selectedItem is Credential credential)
            {
                var popup = new ModifyPopupPage();
                var viewModel = popup.BindingContext as ModifyPopupPageViewModel;

                if (viewModel != null)
                {
                    await viewModel.InitializeAsync(credential.Id.ToString());
                }

                await PopupNavigation.Instance.PushAsync(popup);
            }
        }


        private async void ExecuteDeleteCommand(object selectedItem)
        {
            if (selectedItem is Credential credential)
            {
                Credentials.Remove(credential);
                await _credentialDAO.DeleteCredential(credential.Id);
                await RefreshData();

            }
        }

        public async Task RefreshData()
        {
            Credentials.Clear();
            var credentials = await _credentialDAO.GetAllCredentials();

            string storedUserID = Preferences.Get("IdUser", "");

            foreach (var credential in credentials)
            {
                if (credential.UserId == storedUserID)
                {
                    Credentials.Add(credential);
                }
            }
        }


        private async void ExecuteTogglePasswordVisibilityCommand(object obj)
        {
            if (obj is Credential credential)
            {
                var userBase64 = Preferences.Get("IV", "");
                if (userBase64 != null && userBase64.Length > 0)
                {
                    string decryptedPassword = EncryptionService.Decrypt(credential.PasswordApp, userBase64);

                    credential.VisiblePassword = decryptedPassword;
                    credential.OnPropertyChanged(nameof(credential.VisiblePassword));

                    OnPropertyChanged(nameof(Credentials));

                    credential.EyeIcon = "5";

                    for (int i = 4; i >= 0; i--)
                    {
                        await Task.Delay(1000);
                        credential.EyeIcon = i.ToString();
                    }

                    credential.VisiblePassword = "*****";
                    credential.EyeIcon = "\uf06e";
                    credential.OnPropertyChanged(nameof(credential.VisiblePassword));

                    OnPropertyChanged(nameof(Credentials));
                }
            }
        }




        private async void SearchCredentials()
        {
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                await RefreshData();
            }
            else
            {
                Credentials.Clear();
                var allCredentials = await _credentialDAO.GetAllCredentials();
                var filteredCredentials = allCredentials
                    .Where(credential => credential.Application.ToLower().Contains(_searchText.ToLower()));
                foreach (var credential in filteredCredentials)
                {
                    Credentials.Add(credential);
                }
            }
        }
    }
}
