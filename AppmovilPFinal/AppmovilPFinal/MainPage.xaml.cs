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

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            // Aquí puedes agregar tu lógica de autenticación
            if (IsValidUser(username, password))
            {
                await DisplayAlert("Login", "Login successful!", "OK");
                LoginSection.IsVisible = false; // Oculta la sección de login
                MenuSection.IsVisible = true;  // Muestra la sección del menú
            }
            else
            {
                await DisplayAlert("Login", "Invalid username or password.", "OK");
            }
        }

        private void OnRegisterTapped(object sender, EventArgs e)
        {
            // Navegar a la página de registro
            Navigation.PushAsync(new RegisterPage1());
        }

        private bool IsValidUser(string username, string password)
        {
            // Implementa tu lógica de validación aquí
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
        }
    }
}

