using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using Restaurant.Classes;

namespace Restaurant.Adapters
{
    class BillListViewAdapter : BaseAdapter
    {
        private readonly Context _context;
        private readonly List<OrderItem> _order;

        public BillListViewAdapter(Context context, List<OrderItem> order)
        {
            _context = context;
            _order = order;
        }

        public override int Count => _order.Count;

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public int GetPrice(int position)
        {
            return _order[position].Price;
        }

        public int GetQuantity(int position)
        {
            return _order[position].Quantity;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view;
            LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
            if (convertView == null)
            {
                view = inflater.Inflate(Resource.Layout.ListViewLayout, null);
                TextView nameView = view.FindViewById<TextView>(Resource.Id.listName);
                TextView priceView = view.FindViewById<TextView>(Resource.Id.listPrice);
                nameView.SetTextColor(Color.White);
                nameView.SetTextSize(ComplexUnitType.Dip, 20);
                priceView.SetTextColor(Color.White);
                priceView.SetTextSize(ComplexUnitType.Dip, 20);
                string nume = _order[position].Name;
                if (nume.Length > 30)
                {
                    nume = nume.Substring(0, 30) + "...";
                }
                nameView.Text = nume;
                priceView.Text = _order[position].Quantity + " x " + _order[position].Price 
                    + " RON = " + _order[position].Price * _order[position].Quantity + " RON";
                var obsView = view.FindViewById<TextView>(Resource.Id.listObservations);
                var quantView = view.FindViewById<TextView>(Resource.Id.listQuantity);
                var img = view.FindViewById<ImageView>(Resource.Id.listImage);
                var minus = view.FindViewById<ImageButton>(Resource.Id.lvMinus);
                var plus = view.FindViewById<ImageButton>(Resource.Id.lvPlus);
                var delete = view.FindViewById<ImageButton>(Resource.Id.lvDelete);
                obsView.Visibility = ViewStates.Gone;
                quantView.Visibility = ViewStates.Gone;
                img.Visibility = ViewStates.Gone;
                minus.Visibility = ViewStates.Gone;
                plus.Visibility = ViewStates.Gone;
                delete.Visibility = ViewStates.Gone;
            }
            else
            {
                view = convertView;
            }
            return view;
        }
    }
}