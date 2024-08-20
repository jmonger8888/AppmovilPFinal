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

        private async void ControlAsistenciaClicked(object sender, EventArgs e)
        {
            // Navega a la página de Control de asistencia
            await Navigation.PushAsync(new ControlAsistencia());
        }

        private async void ConsultasClicked(object sender, EventArgs e)
        {
            // Navega a la página de Consultas
            await Navigation.PushAsync(new Consultas());
        }

        /*private async void ReportesClicked(object sender, EventArgs e)
        {
            // Navega a la página de Gestión de Estudiantes
            await Navigation.PushAsync(new Reportes());
        }*/

    }
}

