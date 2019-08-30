using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using Restaurant.Classes;
using Exception = Java.Lang.Exception;

namespace Restaurant.Adapters
{
    public class CustomGridViewAdapter : BaseAdapter
    {
        private readonly Context _context;
        private readonly List<Item> _items;

        public CustomGridViewAdapter(Context context, List<Item> items)
        {
            _context = context;
            _items = items;
        }

        public override int Count => _items.Count;

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view;
            LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
            if(convertView == null)
            {
                view = inflater.Inflate(Resource.Layout.GridViewLayout, null);
                TextView nameView = view.FindViewById<TextView>(Resource.Id.gridName);
                TextView priceView = view.FindViewById<TextView>(Resource.Id.gridPrice);
                ImageView imgView = view.FindViewById<ImageView>(Resource.Id.gridImage);
                var line = view.FindViewById<View>(Resource.Id.gridLine);
                line.Visibility = ViewStates.Visible;
                nameView.Text = _items[position].Name;
                priceView.Text = _items[position].Price + " RON";
                if (_items[position].Image != null)
                {
                    byte[] decodedBytes = Base64.Decode(_items[position].Image, Base64Flags.Default);
                    imgView.SetImageBitmap(DecodeImage(decodedBytes));
                }
                GC.Collect();
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

                int reqSize = 150;
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
                e.PrintStackTrace();
            }
            return null;
        }
    }
}