using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppmovilPFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registro : ContentPage
    {
        public Registro()
        {
            InitializeComponent();
        }
        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            string email = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Por favor, complete todos los campos.", "OK");
                return;
            }

            try
            {
                Console.WriteLine("Registrando nuevo usuario en Firebase...");

                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAqASqMwQkDgjD5PovIehR7pwZPtcBGiQk"));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
                var firebaseToken = auth.FirebaseToken;

                Console.WriteLine("Registro exitoso. Token: " + firebaseToken);

                // Muestra un mensaje de éxito al usuario
                await DisplayAlert("Registro exitoso", "El usuario ha sido registrado correctamente.", "OK");

                // Opcional: Navegar a otra página o limpiar los campos
                UsernameEntry.Text = string.Empty;
                PasswordEntry.Text = string.Empty;
                await Navigation.PushAsync(new Login());
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine("Error de registro: " + ex.Reason);
                await DisplayAlert("Error", "No se pudo registrar: " + ex.Reason, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inesperado: " + ex.Message);
                await DisplayAlert("Error", "Se produjo un error inesperado: " + ex.Message, "OK");
            }
        }
    }
}
    
