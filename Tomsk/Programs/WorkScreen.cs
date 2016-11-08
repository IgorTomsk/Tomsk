
using System;
using System.Collections.Generic;
//using System.Collections;
using System.IO;
//using System.Linq;
//using System.Text;

using Android.App;
//using Android.Content;
using Android.OS;
//using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mono.Data.Sqlite;


using System.Data;
using SQLite;
using System.Net;
//using System.Threading.Tasks;
//using System.Json;
using Newtonsoft.Json;

//using Android;





namespace Tomsk
{
	[Activity(Label = "")]
	public class WorkScreen : Activity
	{
		public static SqliteConnection ConnectionDB { get; set; }
		public static string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SelectCity1.db3");
		public static bool showCity;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			LoadBase();

			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.CountryCity);

			this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

			AddTab("Страны",  new FragmentCountry());
			AddTab("Избранное",  new FragmentFavorite());
			AddTab("Настройки",  new FragmentOther());

			if (savedInstanceState != null)
				this.ActionBar.SelectTab(this.ActionBar.GetTabAt(savedInstanceState.GetInt("tab")));


		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutInt("tab", this.ActionBar.SelectedNavigationIndex);

			base.OnSaveInstanceState(outState);
		}



		void AddTab(string tabText,  Fragment view)
		{
			var tab = this.ActionBar.NewTab();
			tab.SetText(tabText);
	

			// must set event handler before adding tab
			tab.TabSelected += delegate (object sender, ActionBar.TabEventArgs e)
			{
				var fragment = this.FragmentManager.FindFragmentById(Resource.Id.fragmentBox);
				if (fragment != null)
					e.FragmentTransaction.Remove(fragment);
				e.FragmentTransaction.Add(Resource.Id.fragmentBox, view);
			};
			tab.TabUnselected += delegate (object sender, ActionBar.TabEventArgs e)
			{
				e.FragmentTransaction.Remove(view);
			};

			this.ActionBar.AddTab(tab);
		}


		//==========================   FRAGMENT number ONE (Country/City) =========================================

		class FragmentCountry : Fragment
		{

			List<Country> tableItems = new List<Country>();
			ListView listView;

			List<City> tableItemsCity = new List<City>();
			ListView listViewCity;



			public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{

				base.OnCreateView(inflater, container, savedInstanceState);

				var view = inflater.Inflate(Resource.Layout.TabCountry, container, false);


				listView = view.FindViewById<ListView>(Resource.Id.CountryLV);
				listViewCity = view.FindViewById<ListView>(Resource.Id.CityLV);

				ConnectionDB.Open();
				var conn = new SQLiteConnection(dbPath);

				var table = conn.Table<Country>();

				tableItems = new List<Country>();
				foreach (var t in table)
				{
					tableItems.Add(new Country() { Id = t.Id, CountryName = t.CountryName });
				}

				ConnectionDB.Close();


				listView.Adapter = new CountryAdapter(this.Activity, tableItems);

				showCity = true;  // Show and hidden CityList

				listView.ItemClick += OnListItemClick;



				return view;
			}

			protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
			{

				if (showCity)
				{
					int is_favor = 0;
					var i = tableItems[e.Position];


					ConnectionDB.Open();
					var conn = new SQLiteConnection(dbPath);

					tableItemsCity = new List<City>();
					var tableCity = conn.Table<City>();

					foreach (var t in tableCity)  //   I'll use SQL operatior when it not be test task
					{
						if (i.Id == t.CountryId)
						{

							try
							{
								using (var cmd = ConnectionDB.CreateCommand())
								{
									cmd.CommandText = "select * from FavorCity where Id = " + t.Id.ToString();

									using (var reader = cmd.ExecuteReader())
									{
										is_favor = 0;
										while (reader.Read())
										{
											is_favor += 1;
										}
									}

								}

							}
							catch (Exception ex)
							{
								Console.WriteLine(ex);
							};


							if (is_favor > 0)  // Already if favorite

							{
								tableItemsCity.Add(new City()
								{
									Id = t.Id,
									CountryId = t.CountryId,
									CityName = t.CityName,
									ImageResourceId = Resource.Drawable.star_icon
								});
							}
							else
							{
								tableItemsCity.Add(new City()
								{
									Id = t.Id,
									CountryId = t.CountryId,
									CityName = t.CityName,
									ImageResourceId = Resource.Drawable.star_nosel_icon
								});

							}

						}

					}

					ConnectionDB.Close();

					showCity = false;   // next click NOT SOW CityList

					listViewCity.Adapter = new CityAdapter(this.Activity, tableItemsCity);
					listViewCity.ItemClick += OnListItemCityClick;
				}
				else  // hide CityList
				{ 
				
					tableItemsCity = new List<City>();
					listViewCity.Adapter = new CityAdapter(this.Activity, tableItemsCity);
					showCity = true;
				}


			}

			protected void OnListItemCityClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
			{

				// ADD to FAVORITE


				if (tableItemsCity[e.Position].ImageResourceId != Resource.Drawable.star_icon)
				{
					var t = tableItemsCity[e.Position];
					try
					{

						ConnectionDB = new SqliteConnection("Data Source=" + dbPath);

						using (var commander = ConnectionDB.CreateCommand())
						{
							ConnectionDB.Open();
							commander.CommandText = "INSERT INTO FavorCity VALUES(" + t.Id.ToString() + ");";
							commander.CommandType = CommandType.Text;
							commander.ExecuteNonQuery();
							ConnectionDB.Close();

							// change star and refresh
							tableItemsCity[e.Position].ImageResourceId = Resource.Drawable.star_icon;
							listViewCity.Adapter = new CityAdapter(this.Activity, tableItemsCity);

						}

					}
					catch (Exception ex)
					{
						Console.WriteLine("Can not UPDATE Record becase: " + ex);

					}
				}
				else
				{
					Android.Widget.Toast.MakeText(Activity, "Уже в Избранном", Android.Widget.ToastLength.Short).Show();
				}

			}

		}
//==========================   FRAGMENT FAVORITE =========================================

			class FragmentFavorite : Fragment
		{
			List<FavorCity> tableItems = new List<FavorCity>();
			ListView listView;
			string LocalName;
		
		

			public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				base.OnCreateView(inflater, container, savedInstanceState);

				var view = inflater.Inflate(Resource.Layout.TabFaforite, container, false);

				listView = view.FindViewById<ListView>(Resource.Id.FavoriteLV);

				MakeFavorList();

				listView.Adapter = new FavorAdapter(this.Activity, tableItems);

				return view;
			}
//========================

			public void MakeFavorList()
			{
				ConnectionDB.Open();
				var conn = new SQLiteConnection(dbPath);

				tableItems = new List<FavorCity>();
				var table = conn.Table<FavorCity>();

				foreach (var t in table)
				{
					LocalName = "";

					try
					{
						using (var cmd = ConnectionDB.CreateCommand())
						{
							cmd.CommandText = "select CityName from City where Id = " + t.Id.ToString();
							using (var reader = cmd.ExecuteReader())
							{
								while (reader.Read())
								{
									LocalName = reader[0].ToString();
								}
							}

						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					};


					tableItems.Add(new FavorCity() { Id = t.Id, CityName = LocalName });
				}

				ConnectionDB.Close();

			}

		}


//========================= Simply Fragmrnt ===============


		class FragmentOther : Fragment
		{
			public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				base.OnCreateView(inflater, container, savedInstanceState);

				var view = inflater.Inflate(Resource.Layout.TabSetup, container, false);

				return view;
			}
		}


//==================================================================


		public static bool LoadBase()
		{
			ConnectionDB = new SqliteConnection("Data Source=" + dbPath);
			bool exists = File.Exists(dbPath);
			if (!exists)
			{
				CreateDataBase();
	
			}

			// Prepare for reload

			using (var commander = ConnectionDB.CreateCommand())
			{
				ConnectionDB.Open();
				commander.CommandText =
					" DELETE FROM Country; " +
					" DELETE FROM City;" +
     		        " DELETE FROM FavorCity;";
				commander.CommandType = CommandType.Text;
				commander.ExecuteNonQuery();
				ConnectionDB.Close();

			}

			//    Load from API

			string url = "https://atw-backend.azurewebsites.net/api/countries";

			var stringfromurl = GetCountries(url);

			if (stringfromurl != null)

			{
				var root = JsonConvert.DeserializeObject<AskCountry>(stringfromurl);

				var countryList = new List<Result>();
				var cityList = new List<Cities>();

				countryList = root.Result;

				using (var commander = ConnectionDB.CreateCommand())
				{
					ConnectionDB.Open();

					foreach (var item in countryList)
					{
						commander.CommandText = " INSERT INTO Country VALUES (" + item.Id.ToString() + ", '" + item.Name + "');";
						commander.CommandType = CommandType.Text;
						commander.ExecuteNonQuery();

						cityList = item.Cities;

						foreach (var itemCity in cityList)
						{
							commander.CommandText = " INSERT INTO City VALUES (" + itemCity.Id.ToString() +
								", " + item.Id.ToString() + ",'" + itemCity.Name + "' );";
							commander.CommandType = CommandType.Text;
							commander.ExecuteNonQuery();
						}
					}

					ConnectionDB.Close();
				}

			}
			return true;
		}

		//=================================================================================

		public static string GetCountries(String url)
		{
			try
			{
// for test mode not asyc

				var request = WebRequest.Create(url);
				var response = (HttpWebResponse)request.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var datastream = response.GetResponseStream();
					var reader = new StreamReader(datastream);
					var stringfromurl = reader.ReadToEnd();
					return stringfromurl;
				}
				else
				{
					return null;
				}
			}
			catch {
				return null;
				};

		}

//=================================================================================
		static Boolean CreateDataBase()
		{
			try
			{

				SqliteConnection.CreateFile(dbPath);
			
				using (var commander = ConnectionDB.CreateCommand())
				{
					ConnectionDB.Open();
					commander.CommandText = 
						" CREATE TABLE Country (Id INTEGER PRIMARY KEY, CountryName ntext); " +
						" CREATE TABLE City (Id INTEGER PRIMARY KEY, CountryId INTEGER, CityName ntext);" +
						" CREATE TABLE FavorCity (Id INTEGER PRIMARY KEY)";
					commander.CommandType = CommandType.Text;
					commander.ExecuteNonQuery();
					ConnectionDB.Close();

				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Can not CreateDataBase becase: " + ex);
				return false;
			}
		}

		//===========================================================

		// It's run before web get


		static Boolean LoadData()
		{
			try
			{

				using (var commander = ConnectionDB.CreateCommand())
				{
					ConnectionDB.Open();

					commander.CommandText = " DELETE FROM City; DELETE FROM Country; DELETE FROM FavorCity; ";
					commander.CommandType = CommandType.Text;
					commander.ExecuteNonQuery();

					commander.CommandText =
				        " INSERT INTO Country VALUES (1,'Россия'); " +
						" INSERT INTO Country VALUES (2,'Армения'); " +
						" INSERT INTO Country VALUES (3,'Япония');" +
						" INSERT INTO Country VALUES (4,'Канада');" +
						" INSERT INTO Country VALUES (5,'USA');" +
						" INSERT INTO Country VALUES (6,'Germany');" +



						" INSERT INTO City VALUES (1,1,'Томск');" +
						" INSERT INTO City VALUES (2,1,'Колпашево');" +
						" INSERT INTO City VALUES (3,1,'Парабель');" +
						" INSERT INTO City VALUES (4,1,'Каргасок');" +
						" INSERT INTO City VALUES (5,1,'Асино');";
					commander.CommandType = CommandType.Text;
					commander.ExecuteNonQuery();

					commander.CommandText = 
						" INSERT INTO City VALUES (6,2,'Ереван');" +
						" INSERT INTO City VALUES (7,2,'ГородА2');" +
						" INSERT INTO City VALUES (8,2,'ГородА2');" ;
					commander.CommandType = CommandType.Text;
					commander.ExecuteNonQuery();

					commander.CommandText =
						" INSERT INTO City VALUES (9,3,'Токио');" +
						" INSERT INTO City VALUES (10,3,'Хиросима');" +
						" INSERT INTO City VALUES (11,3,'Нагасаки');";
					commander.CommandType = CommandType.Text;
					commander.ExecuteNonQuery();

					commander.CommandText =
						" INSERT INTO City VALUES (12,4,'Оттава');" +
						" INSERT INTO City VALUES (13,4,'Монреаль');" +
						" INSERT INTO City VALUES (14,4,'Квебек');";
					commander.CommandType = CommandType.Text;
					commander.ExecuteNonQuery();

					commander.CommandText =
	" INSERT INTO FavorCity VALUES (2);" +
	" INSERT INTO FavorCity VALUES (6);" +
	" INSERT INTO FavorCity VALUES (14);";
					commander.CommandType = CommandType.Text;
					commander.ExecuteNonQuery();




					ConnectionDB.Close();
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Can not INSERT RECORDS: " + ex);
				return false;
			}
		}

	}
}
