using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Restaurant.Activities;

namespace Restaurant.Dialogs
{
    class DialogSettings : DialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.DialogSettings, container, false);
            view.FindViewById<Button>(Resource.Id.settingsBack).Click += (sender, args) => Dismiss();
            var pass = view.FindViewById<EditText>(Resource.Id.etPassword);
            var etTable = view.FindViewById<EditText>(Resource.Id.etTable);
            if(MainActivity.Table > 0)
                etTable.Text = MainActivity.Table.ToString();
            var etWaiter = view.FindViewById<EditText>(Resource.Id.etWaiter);
            if (MainActivity.WaiterId > 0)
                etWaiter.Text = MainActivity.WaiterId.ToString();
            var btnSet = view.FindViewById<Button>(Resource.Id.settingsSet);
            var sync = view.FindViewById<Button>(Resource.Id.settingsSync);
            etTable.Visibility = ViewStates.Gone;
            etWaiter.Visibility = ViewStates.Gone;
            btnSet.Visibility = ViewStates.Gone;
            sync.Visibility = ViewStates.Gone;
            var passOk = view.FindViewById<Button>(Resource.Id.settingsOk);
            passOk.Click += (sender, args) =>
            {
                if (pass.Text == MainActivity.Password)
                {
                    pass.Visibility = ViewStates.Gone;
                    passOk.Visibility = ViewStates.Gone;
                    etTable.Visibility = ViewStates.Visible;
                    etWaiter.Visibility = ViewStates.Visible;
                    btnSet.Visibility = ViewStates.Visible;
                    sync.Visibility = ViewStates.Visible;
                }
                else if (pass.Text.Equals(""))
                    Toast.MakeText(Activity, "Please provide password!", ToastLength.Short).Show();
                else
                    Toast.MakeText(Activity, "Wrong password!", ToastLength.Short).Show();
            };
            sync.Click += async (sender, args) =>
            {
                Toast.MakeText(Activity, "Sync started", ToastLength.Short).Show();
                await MainActivity.LoadFirebaseData();
                MainActivity.LoadMenu();
                Toast.MakeText(Activity, "Sync completed", ToastLength.Short).Show();
                Dismiss();
            };
            btnSet.Click += async (sender, args) =>
            {
                int t, w;
                var isTableNumeric = int.TryParse(etTable.Text, out t);
                var isWaiterNumeric = int.TryParse(etWaiter.Text, out w);
                if (!string.IsNullOrEmpty(etTable.Text) && !string.IsNullOrEmpty(etWaiter.Text) && t > 0 &&
                    w > 0 && etTable.Text.Length < 4 && etWaiter.Text.Length < 4 && isTableNumeric && isWaiterNumeric)
                {
                    MainActivity.Table = t;
                    MainActivity.WaiterId = w;
                    MainActivity.SaveTableNr();
                    MainActivity.SaveWaiterId();
                    MainActivity.ServerCreateTable();
                    Dismiss();
                    Toast.MakeText(Activity, "Device setted to table " + t + " and waiter id " + w, ToastLength.Long).Show();
                }
                    else Toast.MakeText(Activity, "Wrong input!", ToastLength.Short).Show();
            };
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.DimAmount = 0.80f;
            Dialog.Window.AddFlags(WindowManagerFlags.DimBehind);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.DialogAnimation;
        }
    }
}