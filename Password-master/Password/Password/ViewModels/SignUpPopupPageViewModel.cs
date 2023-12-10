using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Password.Models;
using Password.Services;
using Rg.Plugins.Popup.Services;
using System;
using Password.Views;
using Xamarin.Forms;
using BCrypt.Net;
using Rg.Plugins.Popup.Pages;
using System.Security.Cryptography;

namespace Password.ViewModels
{
    public class SignUpPopupPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<UserData> UserDatas { get; set; } = new ObservableCollection<UserData>();

        private UserData_DAO _userDataDAO;
        public Command CreateCommand { get; }
        public Command ClosePopupCommand { get; }

        private string _user;
        public string User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                PasswordStrength = EvaluatePasswordStrength.CheckStrength(value);
                OnPropertyChanged();
            }
        }

        private EvaluatePasswordStrength.PasswordStrength _passwordStrength;
        public EvaluatePasswordStrength.PasswordStrength PasswordStrength
        {
            get { return _passwordStrength; }
            set
            {
                _passwordStrength = value;
                OnPropertyChanged();
            }
        }

        private string _passwordconfirm;
        public string PasswordConfirm
        {
            get { return _passwordconfirm; }
            set
            {
                _passwordconfirm = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                OnPropertyChanged();
            }
        }

        private DateTime _birthDate = DateTime.Now;
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set
            {
                _birthDate = value;
                OnPropertyChanged();
            }
        }

        public SignUpPopupPageViewModel()
        {
            _userDataDAO = new UserData_DAO();
            CreateCommand = new Command(ExecuteCreateCommand);
            ClosePopupCommand = new Command(ExecuteClosePopupCommand);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public byte[] GenerateRandomIV(int size = 16)
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] randomNumber = new byte[size];
                rng.GetBytes(randomNumber);
                return randomNumber;
            }
        }

        private async void ExecuteCreateCommand()
        {
            if (Password == PasswordConfirm)
            {
                var newUser = new UserData
                {
                    UserName = this.User,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(this.Password),
                    Email = this.Email,
                    Phone = this.Phone,
                    Birth = this.BirthDate.ToString("yyyy-MM-dd"),
                    IV = Convert.ToBase64String(GenerateRandomIV())
                };

                await _userDataDAO.AddUser(newUser);

                this.User = "";
                this.Password = "";
                this.PasswordConfirm = "";
                this.Email = "";
                this.Phone = "";
                this.BirthDate = DateTime.Now;

                await PopupNavigation.Instance.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Les mots de passe ne correspondent pas", "OK");
            }
        }

        private async void ExecuteClosePopupCommand()
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
