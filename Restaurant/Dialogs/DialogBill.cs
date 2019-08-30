using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Restaurant.Activities;
using Restaurant.Adapters;
using Restaurant.Classes;

namespace Restaurant.Dialogs
{
    class DialogBill : DialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.DialogBill, container, false);
            decimal tipAmount = 0;
            decimal orderValue = MainActivity.OrderValue;
            RadioButton radioCash = view.FindViewById<RadioButton>(Resource.Id.rbCash);
            RadioButton radioCard = view.FindViewById<RadioButton>(Resource.Id.rbCard);
            var tip = view.FindViewById<Spinner>(Resource.Id.billSpinner);
            var total = view.FindViewById<TextView>(Resource.Id.billTotal);
            total.Text = "Total: " + MainActivity.OrderValue + " RON";
            var tips = new List<KeyValuePair<string, decimal>>
            {
                new KeyValuePair<string, decimal>("Tip", -1),
                new KeyValuePair<string, decimal>("0%", 1),
                new KeyValuePair<string, decimal>("5%", 1.05m),
                new KeyValuePair<string, decimal>("10%", 1.1m),
                new KeyValuePair<string, decimal>("15%", 1.15m),
                new KeyValuePair<string, decimal>("20%" , 1.2m),
                new KeyValuePair<string, decimal>("25%", 1.25m)
            };
            List<string> tipPercentages = new List<string>();
            foreach (var tipVal in tips)
            {
                tipPercentages.Add(tipVal.Key);
            }
            var spinnerAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.SpinnerItemLayout, tipPercentages);
            tip.Adapter = spinnerAdapter;
            spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            tip.ItemSelected += (sender, args) =>
            {
                if (args.Position != 0)
                {
                    tipAmount = -MainActivity.OrderValue;
                    orderValue = MainActivity.OrderValue;
                    orderValue *= tips[args.Position].Value;
                    tipAmount += orderValue;
                    orderValue = Decimal.Round(orderValue, 2);
                    total.Text = "Total: " + orderValue + " RON";
                }
                else
                {
                    total.Text = "Total: " + MainActivity.OrderValue + " RON";
                }
            };
            view.FindViewById<Button>(Resource.Id.billCancel).Click += (sender, args) =>
            {
                Dismiss();
            };
            view.FindViewById<Button>(Resource.Id.billSend).Click += (sender, args) =>
            {
                if ((radioCard.Checked || radioCash.Checked) && tip.SelectedItemPosition != 0)
                {
                    MainActivity.PaymentMethod = radioCard.Checked ? EPayment.Card : EPayment.Cash;
                    Toast.MakeText(Activity, "Your check is on its way! (" + orderValue + " RON " +
                        MainActivity.PaymentMethod.ToString() + ")", ToastLength.Long).Show();
                    MainActivity.OrderValue = Decimal.Round(orderValue, 2);
                    MainActivity.TipAmount = Decimal.Round(tipAmount, 2);
                    MainActivity.Status = EStatus.Bill;
                    MainActivity.ServerRequestBill();
                    MainActivity.TotalOrder.Clear();
                    MainActivity.OrderValue = 0;
                    Dismiss();
                }
                else if ((radioCard.Checked || radioCash.Checked) && tip.SelectedItemPosition == 0)
                {
                    Toast.MakeText(Activity, "Please select a tip amount!", ToastLength.Short).Show();
                }
                else if (!radioCard.Checked && !radioCash.Checked && tip.SelectedItemPosition == 0)
                {
                    Toast.MakeText(Activity, "Please select a payment method and a tip amount!", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(Activity, "Please select a payment method!", ToastLength.Short).Show();
                }
            };
            BillListViewAdapter adapter = new BillListViewAdapter(Activity, MainActivity.TotalOrder);
            var lv = view.FindViewById<ListView>(Resource.Id.lvOrder);
            lv.Adapter = adapter;
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