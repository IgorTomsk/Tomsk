using System.Collections.Generic;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Tomsk
{
	public class CityAdapter : BaseAdapter<City>
	{
		List<City> items;
		Activity context;
		public CityAdapter(Activity context, List<City> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override City this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];

			View view = convertView;
			if (view == null) // no view to re-use, create new

				view = context.LayoutInflater.Inflate(Resource.Layout.CityLine, null);  // here we connect one row to list item


			view.FindViewById<TextView>(Resource.Id.CityNameAXML).Text = item.CityName;
			view.FindViewById<ImageView>(Resource.Id.imageStar).SetImageResource(item.ImageResourceId);

			return view;
		}
	}
}
