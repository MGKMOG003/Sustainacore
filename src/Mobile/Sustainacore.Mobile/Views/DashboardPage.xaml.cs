using System;
using Microsoft.Maui.Controls;

namespace Sustainacore.Mobile.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var email = await SecureStorage.GetAsync("user_email") ?? "";
        EmailLabel.Text = string.IsNullOrWhiteSpace(email) ? "" : $"Logged in as: {email}";
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        SecureStorage.Remove("auth_token");
        SecureStorage.Remove("user_email");

        if (Shell.Current is not null)
        {
            await Shell.Current.GoToAsync("//Login");
        }
        else
        {
            await Navigation.PopToRootAsync();
        }
    }
}
