using Password.Views;
using Xamarin.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Password.Models;
using Password.Services;
using BCrypt.Net;
using System;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;

namespace Password.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private UserData _user = new UserData();
        public Command OnSignUpLabelTapped { get; }
        public Command<string> LoginCommand { get; }


        // Propriété pour l'utilisateur
        public string User
        {
            get => _user.UserName;
            set
            {
                if (_user.UserName != value)
                {
                    _user.UserName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }



        // Commande pour la connexion

        public LoginViewModel()
        {
            LoginCommand = new Command<string>(ExecuteLoginCommand);
            OnSignUpLabelTapped = new Command(ExecuteOnSignUpLabelTapped);

        }
        private async void ExecuteOnSignUpLabelTapped()
        {
            var popup = new SignUpPopupPage();

            // Affichez la popup
            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void ExecuteLoginCommand(string password)
        {
            IsLoading = true; 

            var us = new UserData_DAO();
            if (await Task.Run(() => us.CheckLogin(User, Password))) 
            {
                // Redirection vers la page principale ou autre action
                Application.Current.MainPage = new AboutPage();
            }
            else
            {
                IsLoading = false; 
                await Application.Current.MainPage.DisplayAlert("Erreur", "Incorrect username or password", "OK");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
