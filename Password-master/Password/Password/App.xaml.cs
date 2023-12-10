using Password.Services;
using Password.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Password
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            Preferences.Set("IdUser", null);
            Preferences.Set("IV", null);

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
