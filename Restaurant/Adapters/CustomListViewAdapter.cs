using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Restaurant.Activities;
using Restaurant.Classes;

namespace Restaurant.Adapters
{
    class CustomListViewAdapter : BaseAdapter
    {
        private readonly Context _context;
        private readonly List<OrderItem> _items;

        public CustomListViewAdapter(Context context, List<OrderItem> items)
        {
            _context = context;
            _items = items;
        }

        public override int Count => _items.Count;

        public override Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public int GetPrice(int position)
        {
            return _items[position].Price;
        }

        public int GetQuantity(int position)
        {
            return _items[position].Quantity;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view;
            LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
            if (convertView == null)
            {
                view = inflater.Inflate(Resource.Layout.ListViewLayout, null);
                ImageView imgView = view.FindViewById<ImageView>(Resource.Id.listImage);
                TextView nameView = view.FindViewById<TextView>(Resource.Id.listName);
                TextView priceView = view.FindViewById<TextView>(Resource.Id.listPrice);
                TextView quantityView = view.FindViewById<TextView>(Resource.Id.listQuantity);
                TextView observationView = view.FindViewById<TextView>(Resource.Id.listObservations);
                if (_items[position].Image != null)
                {
                    byte[] decodedBytes = Base64.Decode(_items[position].Image, Base64Flags.Default);
                    imgView.SetImageBitmap(DecodeImage(decodedBytes));
                }
                nameView.Text = _items[position].Name;
                priceView.Text = _items[position].Price + " RON";
                quantityView.Text = "Quantity: " + _items[position].Quantity;
                observationView.Text = _items[position].Observation;
                ImageButton minus = view.FindViewById<ImageButton>(Resource.Id.lvMinus);
                ImageButton plus = view.FindViewById<ImageButton>(Resource.Id.lvPlus);
                ImageButton delete = view.FindViewById<ImageButton>(Resource.Id.lvDelete);
                if (_items[position].Quantity == 1)
                {
                    minus.Visibility = ViewStates.Invisible;
                }
                minus.Click += (sender, args) =>
                {
                    if (MainActivity.CurrentOrder[position].Quantity > 1)
                    {
                        MainActivity.CurrentOrder[position].Quantity--;
                        quantityView.Text = "Quantity: " + MainActivity.CurrentOrder[position].Quantity.ToString();
                    }
                    if (MainActivity.CurrentOrder[position].Quantity == 1)
                    {
                        minus.Visibility = ViewStates.Invisible;
                    }
                    NotifyDataSetChanged();
                };
                plus.Click += (sender, args) =>
                {
                    MainActivity.CurrentOrder[position].Quantity++;
                    quantityView.Text = "Quantity: " + MainActivity.CurrentOrder[position].Quantity.ToString();
                    if (MainActivity.CurrentOrder[position].Quantity > 1)
                    {
                        minus.Visibility = ViewStates.Visible;
                    }
                    NotifyDataSetChanged();
                };
                delete.Click += (sender, args) =>
                {
                    MainActivity.CurrentOrder.RemoveAt(position);
                    NotifyDataSetChanged();
                };
            }
            else
            {
                view = convertView;
            }
            return view;
        }

        private Bitmap DecodeImage(byte[] bytes)
        {
            try
            {
                BitmapFactory.Options options = new BitmapFactory.Options {InJustDecodeBounds = true};
                BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options);

                int reqSize = 50;
                int scale = 1;
                while (options.OutWidth / scale / 2 >= reqSize && options.OutHeight / scale / 2 >= reqSize)
                {
                    scale *= 2;
                }

                BitmapFactory.Options options2 = new BitmapFactory.Options { InSampleSize = scale };
                return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length, options2);
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            return null;
        }
    }
}