using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Firebase;

namespace RestauranteNoseCual
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // 👇 Deshabilitar Crashlytics antes de inicializar Firebase
            Java.Lang.JavaSystem.SetProperty("firebase_crashlytics_collection_enabled", "false");

            FirebaseApp.InitializeApp(this);

            const int requestNotification = 0;
            string[] notiPermission =
            {
                Manifest.Permission.PostNotifications
            };
            if ((int)Build.VERSION.SdkInt < 33)
            {
                return;
            }
            if (CheckSelfPermission(Manifest.Permission.PostNotifications) == Permission.Granted)
                return;
            RequestPermissions(notiPermission, requestNotification);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}