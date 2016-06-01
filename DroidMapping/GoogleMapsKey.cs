using Android.App;

#if RELEASE
[assembly: MetaDataAttribute("com.google.android.maps.v2.API_KEY", Value="AIzaSyDsSu4A0J-kNQtgphqrhDI5WrTqD9ZiH-4")]
#else
[assembly: MetaDataAttribute("com.google.android.maps.v2.API_KEY", Value="AIzaSyDsSu4A0J-kNQtgphqrhDI5WrTqD9ZiH-48")]
#endif