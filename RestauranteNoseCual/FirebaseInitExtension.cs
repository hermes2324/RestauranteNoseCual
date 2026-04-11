using Plugin.Firebase.CloudMessaging;

namespace RestauranteNoseCual
{
    public static class FirebaseInitExtension
    {
        public static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
#if ANDROID
            builder.Services.AddSingleton(
                CrossFirebaseCloudMessaging.Current
            );
#endif
            return builder;
        }
    }
}