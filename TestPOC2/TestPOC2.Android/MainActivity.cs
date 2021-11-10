using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Database.Sqlite;
using Android.Content;

namespace TestPOC2.Droid
{
    class VegetableDatabase : SQLiteOpenHelper
    {
        const string create_table_sql =
          "CREATE TABLE [vegetables] ([_id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, [name] TEXT NOT NULL UNIQUE)";
        const string DatabaseName = "vegetables.db";
        const int DatabaseVersion = 1;

        public VegetableDatabase(Context context) : base(context, DatabaseName, null, DatabaseVersion) { }
        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL(create_table_sql);
            // seed with data
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Vegetables')");
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Fruits')");
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Flower Buds')");
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Legumes')");
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Bulbs')");
            db.ExecSQL("INSERT INTO vegetables (name) VALUES ('Tubers')");
        }
        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            throw new NotImplementedException();
        }
    }

    [ContentProvider(authorities : new string[] { VegetableProvider.AUTHORITY }, Exported = true, ReadPermission = "com.example.READ_DATABASE", WritePermission = "com.example.WRITE_DATABASE")]
    public class VegetableProvider : ContentProvider
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
        VegetableDatabase vegeDB;
        public override bool OnCreate()
        {
            vegeDB = new VegetableDatabase(Context);
            return true;
        }

        public override Android.Database.ICursor Query(Android.Net.Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            switch (uriMatcher.Match(uri))
            {
                case GET_ALL:
                    return GetFromDatabase();
                case GET_ONE:
                    var id = uri.LastPathSegment;
                    return GetFromDatabase(id); // the ID is the last part of the Uri
                default:
                    return GetFromDatabase();
            }
        }
        Android.Database.ICursor GetFromDatabase()
        {
            return vegeDB.ReadableDatabase.RawQuery("SELECT _id, name FROM vegetables", null);
        }
        Android.Database.ICursor GetFromDatabase(string id)
        {
            return vegeDB.ReadableDatabase.RawQuery("SELECT _id, name FROM vegetables WHERE _id = " + id, null);
        }

        public override String GetType(Android.Net.Uri uri)
        {
            switch (uriMatcher.Match(uri))
            {
                case GET_ALL:
                    return VEGETABLES_MIME_TYPE; // list
                case GET_ONE:
                    return VEGETABLE_MIME_TYPE; // single item
                default:
                    throw new Exception("EHHHH");
            }
        }

        public override int Delete(Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            throw new Java.Lang.UnsupportedOperationException();
        }
        public override Android.Net.Uri Insert(Android.Net.Uri uri, ContentValues values)
        {
            throw new Java.Lang.UnsupportedOperationException();
        }
        public override int Update(Android.Net.Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            throw new Java.Lang.UnsupportedOperationException();
        }
    }

    [Activity(Label = "TestPOC2", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
