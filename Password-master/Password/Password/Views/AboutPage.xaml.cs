using Password.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Password.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            this.BindingContext = new AboutViewModel();

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var viewModel = BindingContext as AboutViewModel;
            if (viewModel != null)
            {
                await viewModel.RefreshData();
            }
        }


    }
}