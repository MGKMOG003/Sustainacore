using Microsoft.Maui.Controls;

namespace Sustainacore.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}
