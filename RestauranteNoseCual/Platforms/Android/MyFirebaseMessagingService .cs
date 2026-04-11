using Android.App;
using Firebase.Messaging;

namespace RestauranteNoseCual.Platforms.Android
{
    [Service(Exported = false)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnNewToken(string token)
        {
            // el plugin maneja esto automáticamente en versiones nuevas
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            // el plugin maneja esto automáticamente en versiones nuevas
        }
    }
}