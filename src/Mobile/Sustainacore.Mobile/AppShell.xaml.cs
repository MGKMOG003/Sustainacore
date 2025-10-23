using Microsoft.Maui.Controls;
using Sustainacore.Mobile.Views;

namespace Sustainacore.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("//Login", typeof(LoginPage));
        Routing.RegisterRoute("//Dashboard", typeof(DashboardPage));
    }
}
