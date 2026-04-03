// MVVM strict : seul le DataContext est défini ici.
using TPdotnetWpf.ViewModels;
using System.Windows;

namespace TPdotnetWpf.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
