﻿using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Restaurant.Activities
{
    [Activity(Theme = "@style/MyTheme.Splash", Icon = "@drawable/aldente", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MainActivity.LoadMenu();
            StartActivity(typeof(MainActivity));
        }

        public override void OnBackPressed() { }
    }
}