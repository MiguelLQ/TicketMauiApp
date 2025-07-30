namespace MauiFirebase.Pages.Login;

public partial class OnboardingPage : ContentPage
{
    public List<OnboardingSlide> Slides { get; set; }

    //Callback al terminar onboarding
    public Action CuandoFinaliza { get; set; }

    public OnboardingPage()
    {
        InitializeComponent();

        Slides = new List<OnboardingSlide>
        {
            new() { Imagen = "logosanji.png", Titulo = "�Bienvenido!", Descripcion = "Esta App de la Municipalidad de San Jer�nimo � Andahuaylas est� pensada para facilitarte el acceso a servicios y mejorar tu experiencia ciudadana." },
            new() { Imagen = "on1.png", Titulo = "�Qu� puedes hacer aqu�?", Descripcion = "Visualizar tus tickets y los premios que puedes canjear." },
            new() { Imagen = "on2.png", Titulo = "Recicla f�cil!", Descripcion = "Reg�strate y usa tu Qr �nico para reciclar." },
            new() { Imagen = "on3.png", Titulo = "Monitorea!", Descripcion = "Sigue y visualiza la ubicaci�n de los veh�culos recolectores en tiempo real." },
        };

        BindingContext = this;
        NavigationPage.SetHasNavigationBar(this, false);

    }

    private void OnEmpezarClicked(object sender, EventArgs e)
    {
        Preferences.Set("onboarding_visto", true);
        CuandoFinaliza?.Invoke(); // ejecuta lo que definas desde App.xaml.cs
    }

    public class OnboardingSlide
    {
        public string Imagen { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
    }
    private int _ultimaPagina => Slides.Count - 1;

    private void OnCarouselPositionChanged(object sender, PositionChangedEventArgs e)
    {
        BotonEmpezar.IsVisible = e.CurrentPosition == _ultimaPagina;
    }
 

}
