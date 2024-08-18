using System;
using Xamarin.Forms;

namespace AppmovilPFinal
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void GestionEstudiantesClicked(object sender, EventArgs e)
        {
            // Navega a la página de Gestión de Estudiantes
            await Navigation.PushAsync(new GestionEstudiantes());
        }


    }
}

