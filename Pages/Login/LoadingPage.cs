using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiFirebase.Pages.Login
{
    public class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            // Fondo azul oscuro
            BackgroundColor = Color.FromArgb("#cdd8e8");

            Content = new Grid
            {
                Padding = 30,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new VerticalStackLayout
                    {
                        Spacing = 25,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            // 🖼 Imagen superior (logo o icono)
                            new Image
                            {
                                Source = "logosanji.png", // ⚠️ Asegúrate de que esté en Resources/Images/
                                WidthRequest = 200,
                                HeightRequest = 200,
                                HorizontalOptions = LayoutOptions.Center
                            },

                            new ActivityIndicator
                            {
                                IsRunning = true,
                                Color = Colors.White,
                                WidthRequest = 50,
                                HeightRequest = 50
                            },

                            new Label
                            {
                                Text = "Cargando...",
                                FontSize = 20,
                                TextColor = Colors.White,
                                HorizontalTextAlignment = TextAlignment.Center
                            }
                        }
                    }
                }
            };
        }
    }
}
