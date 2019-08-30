using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Restaurant.Dialogs
{
    class DialogAbout : DialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.DialogAbout, container, false);
            view.FindViewById<Button>(Resource.Id.aboutBack).Click += (sender, args) => Dismiss();
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