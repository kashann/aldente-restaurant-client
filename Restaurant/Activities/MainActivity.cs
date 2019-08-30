using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Xamarin.Database;
using Newtonsoft.Json;
using Restaurant.Adapters;
using Restaurant.Classes;
using Restaurant.Dialogs;

namespace Restaurant.Activities
{
    [Activity(Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Landscape)]

    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        #region Attributes
        private const string FirebaseUrl = "https://restaurant-ae0a0.firebaseio.com/";
        public const string WebApi = "https://webtech-kashann.c9users.io/";
        private const string SharedPreferencesString = "MENU";
        private static readonly Database Db = new Database();
        public static string Password = "pass";
        public static List<OrderItem> TotalOrder = new List<OrderItem>();
        public static List<OrderItem> CurrentOrder = new List<OrderItem>();
        public static decimal OrderValue = 0;
        public static decimal TipAmount = 0;
        public static int Table;
        public static int WaiterId;
        public static EStatus Status = EStatus.Thinking;
        public static EPayment PaymentMethod;
        public static int CanceledCounter = 0;
        public static HttpClient Client = new HttpClient();
        private readonly string[] _homeStrings = { "", "", "", "Food", "Drinks", "Dessert" };
        private readonly int[] _homeImgs =
        {
            0, 0, 0, Resource.Drawable.food, Resource.Drawable.drink, Resource.Drawable.dessert
        };
        private DrawerLayout _drawerLayout;
        private NavigationView _navigationView;
        private GridView _gridView;
        private static List<KeyValuePair<string, List<Item>>> _menu;
        private static readonly List<string> MenuList = new List<string>()
        {
            "breakfast", "entrees", "soups", "menus", "main", "side", "burgers", "pizza",
            "pasta", "salads", "sushi", "toppings", "water", "soda", "fresh", "tea", "coffee",
            "beer", "red", "white", "rose", "cocktails", "spirits", "champagne", "dessert"
        };
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.SetFormat(Format.Rgba8888);
            SetContentView(Resource.Layout.Main);
            ConfigureToolbar();
            ConfigureNavigationDrawer();
            ConfigureHomeGrid(_homeStrings, _homeImgs);
            GetTableNr();
            GetWaiterId();
        }

        public override void OnBackPressed() { }

        #region CONFIGURE
        private void ConfigureToolbar()
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetTitle(Resource.String.menu);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Mipmap.restaurant_white_48dp);
        }

        private void ConfigureNavigationDrawer()
        {
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedOpen);
            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            _navigationView.SetNavigationItemSelectedListener(this);
        }

        private void ConfigureHomeGrid(string[] name, int[] img)
        {
            ClearGridView();
            HomeGridViewAdapter adapter = new HomeGridViewAdapter(this, name, img);
            _gridView = FindViewById<GridView>(Resource.Id.gridView);
            _gridView.Adapter = adapter;
            _gridView.SetPadding(210, 0, 10, 0);
            _gridView.ItemClick += (s, e) =>
            {
                switch (e.Position)
                {
                    case 3:
                        _navigationView.Menu.Clear();
                        _navigationView.InflateMenu(Resource.Menu.menuFood);
                        ClearGridView();
                        break;
                    case 4:
                        _navigationView.Menu.Clear();
                        _navigationView.InflateMenu(Resource.Menu.menuDrinks);
                        ClearGridView();
                        break;
                    case 5:
                        _navigationView.Menu.Clear();
                        ConfigureGridView(_menu.First(kvp => kvp.Key == "dessert").Value);
                        break;
                }
            };
        }

        private void ConfigureGridView(List<Item> listItems)
        {
            ClearGridView();
            CustomGridViewAdapter adapter = new CustomGridViewAdapter(this, listItems);
            _gridView = FindViewById<GridView>(Resource.Id.gridView);
            _gridView.Adapter = adapter;
            _gridView.SetPadding(210, 0, 10, 0);
            _gridView.ItemClick += (s, e) =>
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                DialogAddItem dialogAddItem = new DialogAddItem(listItems[e.Position]);
                dialogAddItem.SetStyle(DialogFragmentStyle.NoFrame, 0);
                dialogAddItem.Show(transaction, "dialog_addItem");
            };
            if(listItems.Count == 0)
                Toast.MakeText(this, "Please contact a waiter to sync the app!", ToastLength.Short).Show();
        }

        private void ClearGridView()
        {
            _gridView = FindViewById<GridView>(Resource.Id.gridView);
            _gridView.Adapter = null;
            _gridView.Dispose();
        }

        public static void LoadMenu()
        {
            Db.CreateDatabase();
            _menu = new List<KeyValuePair<string, List<Item>>>
            {
                new KeyValuePair<string, List<Item>>("breakfast", Db.SelectTable("breakfast")),
                new KeyValuePair<string, List<Item>>("entrees", Db.SelectTable("entrees")),
                new KeyValuePair<string, List<Item>>("soups", Db.SelectTable("soups")),
                new KeyValuePair<string, List<Item>>("menus", Db.SelectTable("menus")),
                new KeyValuePair<string, List<Item>>("main", Db.SelectTable("main")),
                new KeyValuePair<string, List<Item>>("side", Db.SelectTable("side")),
                new KeyValuePair<string, List<Item>>("burgers", Db.SelectTable("burgers")),
                new KeyValuePair<string, List<Item>>("pizza", Db.SelectTable("pizza")),
                new KeyValuePair<string, List<Item>>("pasta", Db.SelectTable("pasta")),
                new KeyValuePair<string, List<Item>>("salads", Db.SelectTable("salads")),
                new KeyValuePair<string, List<Item>>("sushi", Db.SelectTable("sushi")),
                new KeyValuePair<string, List<Item>>("toppings", Db.SelectTable("toppings")),
                new KeyValuePair<string, List<Item>>("water", Db.SelectTable("water")),
                new KeyValuePair<string, List<Item>>("soda", Db.SelectTable("soda")),
                new KeyValuePair<string, List<Item>>("fresh", Db.SelectTable("fresh")),
                new KeyValuePair<string, List<Item>>("tea", Db.SelectTable("tea")),
                new KeyValuePair<string, List<Item>>("coffee", Db.SelectTable("coffee")),
                new KeyValuePair<string, List<Item>>("beer", Db.SelectTable("beer")),
                new KeyValuePair<string, List<Item>>("red", Db.SelectTable("red")),
                new KeyValuePair<string, List<Item>>("white", Db.SelectTable("white")),
                new KeyValuePair<string, List<Item>>("rose", Db.SelectTable("rose")),
                new KeyValuePair<string, List<Item>>("cocktails", Db.SelectTable("cocktails")),
                new KeyValuePair<string, List<Item>>("spirits", Db.SelectTable("spirits")),
                new KeyValuePair<string, List<Item>>("champagne", Db.SelectTable("champagne")),
                new KeyValuePair<string, List<Item>>("dessert", Db.SelectTable("dessert"))
            };
        }
        #endregion

        #region Server
        public static async Task<HttpResponseMessage> ServerCreateTable()
        {
            var endpoint = new Uri(WebApi + "tables");
            var requestString = JsonConvert.SerializeObject(
                new
                {
                    table_number = Table,
                    status = Status.ToString(),
                    tip = TipAmount,
                    waiter = WaiterId
                });
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await Client.PostAsync(endpoint, content);
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

        public static async Task<HttpResponseMessage> ServerRequestWaiter()
        {
            int c = CanceledCounter;
            await Task.Delay(5000);
            if (CanceledCounter > c)
            {
                Toast.MakeText(Application.Context, "Your command has been canceled!", ToastLength.Long).Show();
                return null;
            }
            Status = EStatus.Waiting;
            var endpoint = new Uri(WebApi + "tables/" + Table);
            var requestString = JsonConvert.SerializeObject(
                new
                {
                    status = Status.ToString()
                });
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await Client.PutAsync(endpoint, content);
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

        public static async Task<HttpResponseMessage> ServerRequestBill()
        {
            var endpoint = new Uri(WebApi + "tables/" + Table);
            var requestString = JsonConvert.SerializeObject(
                new
                {
                    status = Status.ToString(),
                    total = OrderValue,
                    tip = TipAmount,
                    payment = PaymentMethod.ToString()
                });
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await Client.PutAsync(endpoint, content);
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
            Status = EStatus.Thinking;
            return responseMessage;
        }

        public static async Task<HttpResponseMessage> ServerOrderStatus()
        {
            var endpoint = new Uri(WebApi + "tables/" + Table);
            var requestString = JsonConvert.SerializeObject(
                new
                {
                    status = Status.ToString()
                });
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await Client.PutAsync(endpoint, content);
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
        #endregion

        #region Firebase
        public static async Task LoadFirebaseData()
        {
            Db.RemoveTable();
            var tasks = new List<Task>();
            foreach (var category in MenuList)
            {
                tasks.Add(FireBaseDownload(category));
            }
            try
            {
                foreach (var t in tasks)
                {
                    await t;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }

        public static async Task FireBaseDownload(string categ)
        {
            var firebase = new FirebaseClient(FirebaseUrl);
            var firebaseItems = await firebase
                .Child(categ)
                .OnceSingleAsync<List<Item>>();
            foreach (var i in firebaseItems)
            {
                Item item = new Item(i.Name, i.Description, i.Price, i.Image.Substring(i.Image.IndexOf(",", StringComparison.Ordinal) + 1));
                item.Category = categ;
                Db.InsertIntoTable(item);
            }
        }
        #endregion

        #region Shared Preferences
        public void GetTableNr()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences(SharedPreferencesString, FileCreationMode.Private);
            var nr = pref.GetString("table_number", null);
            if (nr == null)
            {
                Table = -1;
                Toast.MakeText(this, "Please contact a waiter to set table number!", ToastLength.Long).Show();
            }
            else
                Table = int.Parse(nr);
        }

        public void GetWaiterId()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences(SharedPreferencesString, FileCreationMode.Private);
            var nr = pref.GetString("waiter_id", null);
            if (nr == null)
            {
                WaiterId = -1;
                Toast.MakeText(this, "Please contact a waiter to set the waiter ID!", ToastLength.Long).Show();
            }
            else
                WaiterId = int.Parse(nr);
        }

        public static void SaveTableNr()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences(SharedPreferencesString, FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutString("table_number", Table.ToString()).Apply();
        }

        public static void SaveWaiterId()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences(SharedPreferencesString, FileCreationMode.Private);
            ISharedPreferencesEditor editor = pref.Edit();
            editor.PutString("waiter_id", WaiterId.ToString()).Apply();
        }
        #endregion

        #region Top Menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.topMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    _navigationView.Menu.Clear();
                    ConfigureHomeGrid(_homeStrings, _homeImgs);
                    break;
                case Resource.Id.topMenu_food:
                    _navigationView.Menu.Clear();
                    _navigationView.InflateMenu(Resource.Menu.menuFood);
                    ClearGridView();
                    break;
                case Resource.Id.topMenu_drinks:
                    _navigationView.Menu.Clear();
                    _navigationView.InflateMenu(Resource.Menu.menuDrinks);
                    ClearGridView();
                    break;
                case Resource.Id.topMenu_dessert:
                    _navigationView.Menu.Clear();
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "dessert").Value);
                    break;
                case Resource.Id.topMenu_service:
                    Toast.MakeText(this, "Your waiter has been requested!", ToastLength.Long).Show();
                    ServerRequestWaiter();
                    break;
                case Resource.Id.topMenu_bill:
                    if (TotalOrder.Count > 0)
                    {
                        FragmentTransaction trans = FragmentManager.BeginTransaction();
                        DialogBill dialogBill = new DialogBill();
                        dialogBill.SetStyle(DialogFragmentStyle.NoFrame, 0);
                        dialogBill.Show(trans, "dialog_bill");
                    }
                    else
                        Toast.MakeText(this, "Error! No order was sent!", ToastLength.Short).Show();
                    break;
                case Resource.Id.topMenu_cancel:
                    CanceledCounter++;
                    if(Status == EStatus.Waiting)
                        Toast.MakeText(this, "Sorry! Too late to cancel your request!", ToastLength.Short).Show();
                    break;
                case Resource.Id.topMenu_order:
                    if (CurrentOrder.Count > 0)
                    {
                        Intent intent = new Intent(this, typeof(OrderActivity));
                        StartActivity(intent);
                    }
                    else
                    {
                        Toast.MakeText(this, "Your current order is empty!", ToastLength.Long).Show();
                    }
                    break;
                case Resource.Id.topMenu_about:
                    FragmentTransaction transaction = FragmentManager.BeginTransaction();
                    DialogAbout dialogAbout = new DialogAbout();
                    dialogAbout.SetStyle(DialogFragmentStyle.NoFrame, 0);
                    dialogAbout.Show(transaction, "dialog_about");
                    break;
                case Resource.Id.topMenu_admin:
                    FragmentTransaction transactionSettings = FragmentManager.BeginTransaction();
                    DialogSettings dialogSettings = new DialogSettings();
                    dialogSettings.SetStyle(DialogFragmentStyle.NoFrame, 0);
                    dialogSettings.Show(transactionSettings, "dialog_settings");
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion

        #region Side Menu
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            switch (menuItem.ItemId)
            {
                case Resource.Id.menu_food_breakfast:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "breakfast").Value);
                    break;
                case Resource.Id.menu_food_appetizer:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "entrees").Value);
                    break;
                case Resource.Id.menu_food_soup:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "soups").Value);
                    break;
                case Resource.Id.menu_food_menus:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "menus").Value);
                    break;
                case Resource.Id.menu_food_main_dishes:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "main").Value);
                    break;
                case Resource.Id.menu_food_side_dishes:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "side").Value);
                    break;
                case Resource.Id.menu_food_burgers:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "burgers").Value);
                    break;
                case Resource.Id.menu_food_pizza:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "pizza").Value);
                    break;
                case Resource.Id.menu_food_pasta:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "pasta").Value);
                    break;
                case Resource.Id.menu_food_salads:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "salads").Value);
                    break;
                case Resource.Id.menu_food_sushi:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "sushi").Value);
                    break;
                case Resource.Id.menu_food_toppings:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "toppings").Value);
                    break;
                case Resource.Id.menu_drink_water:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "water").Value);
                    break;
                case Resource.Id.menu_drink_soda:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "soda").Value);
                    break;
                case Resource.Id.menu_drink_fresh_juice:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "fresh").Value);
                    break;
                case Resource.Id.menu_drink_tea:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "tea").Value);
                    break;
                case Resource.Id.menu_drink_coffee:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "coffee").Value);
                    break;
                case Resource.Id.menu_drink_beer:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "beer").Value);
                    break;
                case Resource.Id.menu_drink_wine_red:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "red").Value);
                    break;
                case Resource.Id.menu_drink_wine_white:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "white").Value);
                    break;
                case Resource.Id.menu_drink_wine_rose:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "rose").Value);
                    break;
                case Resource.Id.menu_drink_spirits:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "spirits").Value);
                    break;
                case Resource.Id.menu_drink_champagne:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "champagne").Value);
                    break;
                case Resource.Id.menu_drink_cocktails:
                    ConfigureGridView(_menu.First(kvp => kvp.Key == "cocktails").Value);
                    break;
            }
            return true;
        }
        #endregion
    }
}