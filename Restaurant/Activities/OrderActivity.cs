using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.Support.V7.App;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using Restaurant.Adapters;
using Restaurant.Classes;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Restaurant.Activities
{
    [Activity(Label = "@string/your_order", ScreenOrientation = ScreenOrientation.Landscape)]
    public class OrderActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.SetFormat(Format.Rgba8888);
            SetContentView(Resource.Layout.Order);
            ConfigureToolbar();
            ConfigureListView();
            FindViewById<Button>(Resource.Id.btnRefresh).Click += (sender, args) =>
            {
                ConfigureListView();
            };
            FindViewById<Button>(Resource.Id.btnSendOrder).Click += async (sender, args) =>
            {
                MainActivity.OrderValue += ConfigureListView();
                var lv = FindViewById<ListView>(Resource.Id.lvOrder);
                lv.Adapter = null;
                foreach (OrderItem o in MainActivity.CurrentOrder)
                {
                    MainActivity.TotalOrder.Add(o);
                }
                MainActivity.Status = EStatus.Ordered;
                OnBackPressed();
                Toast.MakeText(this, "Your order has been sent!", ToastLength.Short).Show();
                await MainActivity.ServerOrderStatus();
                await ServerSendOrder(MainActivity.CurrentOrder);
                MainActivity.CurrentOrder.Clear();
            };
        }

        public async Task<HttpResponseMessage> ServerSendOrder(List<OrderItem> list)
        {
            var endpoint = new Uri($"{MainActivity.WebApi}tables/{MainActivity.Table}/orders");
            var order = new List<JsonOrderItem>();
            foreach (var oi in list)
            {
                order.Add(new JsonOrderItem(oi));
            }
            var requestString = JsonConvert.SerializeObject(order);
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await MainActivity.Client.PostAsync(endpoint, content);
            }
            catch (Exception ex)
            {
                if (responseMessage == null)
                {
                    responseMessage = new HttpResponseMessage();
                }
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.ReasonPhrase = $"RestHttpClient.SendRequest failed: {ex}";
                Toast.MakeText(Application.Context, "Server error!", ToastLength.Short).Show();
            }
            return responseMessage;
        }

        public int ConfigureListView()
        {
            var total = FindViewById<TextView>(Resource.Id.total);
            CustomListViewAdapter adapter = new CustomListViewAdapter(this, MainActivity.CurrentOrder);
            var lv = FindViewById<ListView>(Resource.Id.lvOrder);
            lv.Adapter = adapter;
            int sum = 0;
            for (int i = 0; i < lv.Count; i++)
            {
                sum += adapter.GetPrice(i) * adapter.GetQuantity(i);
            }
            total.Text = "Total: " + sum + " RON";
            return sum;
        }

        private void ConfigureToolbar()
        {
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbarOrder);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }
    }
}