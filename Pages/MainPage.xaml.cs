using MauiFirebase.Models;
using MauiFirebase.PageModels;

namespace MauiFirebase.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}