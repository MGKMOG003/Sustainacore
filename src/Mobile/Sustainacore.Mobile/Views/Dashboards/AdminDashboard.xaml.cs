namespace Sustainacore.Mobile.Views.Dashboards;

public partial class AdminDashboard : ContentPage
{
    public AdminDashboard(string name)
    {
        Content = new VerticalStackLayout
        {
            Padding = 24,
            Children =
            {
                new Label{ Text = $"Welcome, {name}", FontSize=22, FontAttributes=FontAttributes.Bold },
                new Label{ Text = "Welcome to Admin dashboard." }
            }
        };
    }
}
