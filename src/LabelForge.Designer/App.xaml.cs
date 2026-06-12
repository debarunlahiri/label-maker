// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

namespace LabelMaker;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        UserAppTheme = AppTheme.Light;
        MainPage = new AppShell();
    }
}
