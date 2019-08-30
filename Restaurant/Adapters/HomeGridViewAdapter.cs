using Android.Content;
using Android.Views;
using Android.Widget;

namespace Restaurant.Adapters
{
    public class HomeGridViewAdapter : BaseAdapter
    {
        private readonly Context _context;
        private readonly int[] _gridViewImage;
        private readonly string[] _gridViewName;


        public HomeGridViewAdapter(Context context, string[] gvName, int[] gvImg)
        {
            _context = context;
            _gridViewName = gvName;
            _gridViewImage = gvImg;
        }

        public override int Count => 6;

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
                var nameView = view.FindViewById<TextView>(Resource.Id.gridName);
                var imgView = view.FindViewById<ImageView>(Resource.Id.gridImage);
                nameView.Text = _gridViewName[position];
                imgView.SetImageResource(_gridViewImage[position]);
            }
            else
            {
                view = convertView;
            }
            return view;
        }

        public override bool AreAllItemsEnabled()
        {
            return false;
        }

        public override bool IsEnabled(int position)
        {
            if (position == 3 || position == 4 || position == 5)
                return true;
            return false;
        }
    }
}