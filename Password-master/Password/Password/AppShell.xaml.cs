using Password.ViewModels;
using Password.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Password
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
          
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
