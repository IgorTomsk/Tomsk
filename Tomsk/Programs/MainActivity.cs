using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using Android.Content;

namespace Tomsk
{
	
	[Activity(Label = "Тестовое задание", MainLauncher = true, NoHistory = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

		}

		protected override void OnResume()
		{
			base.OnResume();

			Task startupWork = new Task(() =>
			{

				System.Threading.Thread.Sleep(3000);

			});

			startupWork.ContinueWith(t =>
			{

				StartActivity(new Intent(Application.Context, typeof(WorkScreen)));
			}, TaskScheduler.FromCurrentSynchronizationContext());

			startupWork.Start();
		}



	}

}

