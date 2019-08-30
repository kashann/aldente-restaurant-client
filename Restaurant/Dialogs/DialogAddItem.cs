using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Restaurant.Activities;
using Restaurant.Classes;

namespace Restaurant.Dialogs
{
    class DialogAddItem : DialogFragment
    {
        private OrderItem Item { get; }
        public DialogAddItem(Item item)
        {
            Item = new OrderItem(item);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.DialogAddItem, container, false);
            var imgView = view.FindViewById<ImageView>(Resource.Id.dialogImage);
            if (Item.Image != null)
            {
                byte[] decodedBytes = Base64.Decode(Item.Image, Base64Flags.Default);
                imgView.SetImageBitmap(DecodeImage(decodedBytes));
            }
            var name = view.FindViewById<TextView>(Resource.Id.dialogName);
            name.Text = Item.Name;
            var price = view.FindViewById<TextView>(Resource.Id.dialogPrice);
            price.Text = "Price: " + Item.Price + " RON";
            var description = view.FindViewById<TextView>(Resource.Id.dialogDescription);
            description.Text = Item.Description;
            var quantity = view.FindViewById<TextView>(Resource.Id.dialogQuantity);
            var observations = view.FindViewById<EditText>(Resource.Id.etObservations);
            var buttonMinus = view.FindViewById<Button>(Resource.Id.btnDecreaseQuantity);
            buttonMinus.Visibility = ViewStates.Invisible;

            view.FindViewById<Button>(Resource.Id.btnIncreaseQuantity).Click += (sender, args) =>
            {
                int nr = Convert.ToInt32(quantity.Text);
                nr++;
                quantity.Text = nr.ToString();
                if(nr > 1)
                {
                    buttonMinus.Visibility = ViewStates.Visible;
                }
            };
            view.FindViewById<Button>(Resource.Id.btnDecreaseQuantity).Click += (sender, args) =>
            {
                int nr = Convert.ToInt32(quantity.Text);
                if (nr > 1)
                {
                    nr--;
                    quantity.Text = nr.ToString();
                }
                if (nr == 1)
                {
                    buttonMinus.Visibility = ViewStates.Invisible;
                }
            };
            view.FindViewById<Button>(Resource.Id.btnAddToOrder).Click += (sender, args) =>
            {
                Toast.MakeText(Application.Context, "Your item has been added to your order!", ToastLength.Short).Show();
                Item.Quantity = Convert.ToInt32(quantity.Text);
                Item.Observation = observations.Text;
                MainActivity.CurrentOrder.Add(Item);
                Dismiss();
            };
            view.FindViewById<Button>(Resource.Id.btnCancel).Click += (sender, args) =>
            {
                Dismiss();
            };
            observations.FocusChange += (sender, args) =>
            {
                InputMethodManager inputManager = (InputMethodManager)View.Context.GetSystemService(Context.InputMethodService);

                inputManager.HideSoftInputFromWindow(observations.WindowToken, HideSoftInputFlags.NotAlways);
            };
            GC.Collect();
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

        private Bitmap DecodeImage(byte[] bytes)
        {
            try
            {
                BitmapFactory.Options options = new BitmapFactory.Options {InJustDecodeBounds = true};
                BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options);

                int reqSize = 350;
                int scale = 1;
                while (options.OutWidth / scale / 2 >= reqSize && options.OutHeight / scale / 2 >= reqSize)
                {
                    scale *= 2;
                }

                BitmapFactory.Options options2 = new BitmapFactory.Options {InSampleSize = scale};
                return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
    }
}