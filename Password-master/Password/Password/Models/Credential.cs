using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Password.Models
{
    public class Credential : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string  UserId { get; set; }
        public string Application { get; set; }
        public string UserNameApp { get; set; }
        public string PasswordApp { get; set; }
        public string Email { get; set; }
        public string URL { get; set; }
        public string CreatedDate { get; set; }
        public string LastModifiedDate { get; set; }
        public string VisiblePassword { get; set; } = "*****";
        private string _eyeIcon = "\uf06e";
        public string EyeIcon
        {
            get => _eyeIcon;
            set
            {
                _eyeIcon = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

