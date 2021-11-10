using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using Android.Database;

namespace TestPOC.Droid
{
    public class VegetableProvider
    {
        const int GET_ALL = 0; // return code when list of Vegetables requested
        const int GET_ONE = 1; // return code when a single Vegetable is requested by ID
        static UriMatcher uriMatcher = BuildUriMatcher();
        static UriMatcher BuildUriMatcher()
        {
            var matcher = new UriMatcher(UriMatcher.NoMatch);
            // Uris to match, and the code to return when matched
            matcher.AddURI(AUTHORITY, BASE_PATH, GET_ALL); // all vegetables
            matcher.AddURI(AUTHORITY, BASE_PATH + "/#", GET_ONE); // specific vegetable by numeric ID
            return matcher;
        }

        public const string AUTHORITY = "com.xamarin.sample.VegetableProvider";
        static string BASE_PATH = "vegetables";
        public static readonly Android.Net.Uri CONTENT_URI = Android.Net.Uri.Parse("content://" + AUTHORITY + "/" + BASE_PATH);
        // MIME types used for getting a list, or a single vegetable
        public const string VEGETABLES_MIME_TYPE = ContentResolver.CursorDirBaseType + "/vnd.com.xamarin.sample.Vegetables";
        public const string VEGETABLE_MIME_TYPE = ContentResolver.CursorItemBaseType + "/vnd.com.xamarin.sample.Vegetables";
        // Column names
        public static class InterfaceConsts
        {
            public const string Id = "_id";
            public const string Name = "name";
        }
    }

    [Activity(Label = "TestPOC", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var uri = VegetableProvider.CONTENT_URI;
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            string[] projection = new string[] { VegetableProvider.InterfaceConsts.Id, VegetableProvider.InterfaceConsts.Name };
            string[] fromColumns = new string[] { VegetableProvider.InterfaceConsts.Name };
            var result = ContentResolver.Query(uri, projection, null, null, null);
            var x = ContentResolver.AcquireContentProviderClient(uri);
            Console.WriteLine();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
