using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Xamarin.Essentials;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppmovilPFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }
        public async void OnRegisterLabelTapped(object sender, EventArgs e)
        {
            //Navegar a la página de registro
            await Navigation.PushAsync(new Registro());
        }
        //por medio del firebase auth verifica si el usuario existe y se procede hacer login 
        public async void OnLoginButtonClicked(object sender, EventArgs e)
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
                Console.WriteLine("Iniciando autenticación con Firebase...");

                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAqASqMwQkDgjD5PovIehR7pwZPtcBGiQk"));
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);
                var firebaseToken = auth.FirebaseToken;

                Console.WriteLine("Autenticación exitosa. Token: " + firebaseToken);
                await DisplayAlert("Inicio de sesión exitoso", "Bienvenido/a a la aplicación", "OK");

                // Navegar a la página principal o dashboard
                await Navigation.PushAsync(new MainPage());
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine("Error de autenticación: " + ex.Reason);
                await DisplayAlert("Error", "No se pudo iniciar sesión: " + ex.Reason, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inesperado: " + ex.Message);
                await DisplayAlert("Error", "Se produjo un error inesperado: ", "Complete con los datos correctos", "OK");
            }
        }
        //restablecer contrasena
        public async void OnForgotPasswordLabelTapped(object sender, EventArgs e)
        {
            string email = UsernameEntry.Text;

            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Error", "Por favor, ingrese su correo electrónico para restablecer la contraseña.", "OK");
                return;
            }

            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAqASqMwQkDgjD5PovIehR7pwZPtcBGiQk"));
                await authProvider.SendPasswordResetEmailAsync(email);
                await DisplayAlert("Éxito", "Se ha enviado un enlace para restablecer la contraseña a su correo electrónico.", "OK");
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine("Error de Firebase: " + ex.Reason);
                await DisplayAlert("Error", "No se pudo enviar el correo de restablecimiento: " + ex.Reason, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inesperado: " + ex.Message);
                await DisplayAlert("Error", "Se produjo un error inesperado: " + ex.Message, "OK");
            }
        }

    }

}


