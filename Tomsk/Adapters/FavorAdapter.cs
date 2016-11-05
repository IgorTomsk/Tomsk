using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Accessibility;
using Android.Widget;
using Mono.Data.Sqlite;

namespace Tomsk
{
	public class FavorAdapter : BaseAdapter<FavorCity>
	{

		// Best to public GLOBAL
		public static SqliteConnection ConnectionDB1 { get; set; }
		public static string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SCity.db3");

		List<FavorCity> items;
		Activity context;
		public FavorAdapter(Activity context, List<FavorCity> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override FavorCity this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{

			ImageButton BtnDel;


			var item = items[position];

			View view = convertView;
			if (view == null) // no view to re-use, create new

				view = context.LayoutInflater.Inflate(Resource.Layout.FavorLine, null);  // here we connect one row to list item


			view.FindViewById<TextView>(Resource.Id.FavorCityName).Text = item.CityName;

			BtnDel = view.FindViewById<ImageButton>(Resource.Id.BtnDelFavor);

			BtnDel.Click += (object sender, EventArgs e) =>
				{
				// Better to function but now is good
					try
					{

			            ConnectionDB1 = new SqliteConnection("Data Source=" + dbPath);

						using (var commander = ConnectionDB1.CreateCommand())
						{
							ConnectionDB1.Open();
					    	commander.CommandText = "DELETE FROM FavorCity WHERE Id = " + item.Id.ToString();
							commander.CommandType = CommandType.Text;
							commander.ExecuteNonQuery();
							ConnectionDB1.Close();

							items.Remove(item);
							context.RunOnUiThread(() => this.NotifyDataSetChanged());
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine("Can not Delete Record becase: " + ex);
						
					}



				};
					
			return view;
		}


	}

}
