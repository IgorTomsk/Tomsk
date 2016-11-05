using System;
using System.Collections.Generic;

namespace Tomsk
{
	public class Country
		{
		public Int16 Id { get; set; }
		public string CountryName { get; set; }
		}

	public class City
	{
		public Int16 Id { get; set; }
		public Int16 CountryId { get; set; }
		public string CityName { get; set; }
		public int ImageResourceId { get; set; }
	}

	public class FavorCity
	{
		public Int16 Id { get; set; }
		public string CityName { get; set; }

	}


	// for JSON

	public class AskCountry
	{

		public string Timestamp { get; set; }
		public string Error { get; set; }
		public List<Result> Result { get; set; }

	}

	public class Result
	{

		public int Id { get; set; }
		public string Name { get; set; }
		public string ImageLink { get; set; }
		public List<Cities> Cities { get; set; }

	}

	public class Cities
	{
		public int Id { get; set; }
		public int CountryId { get; set; }
		public string Name { get; set; }
		public string ImageLink { get; set; }
		public List<Artists> Artists { get; set; }

	}
	public class Artists
	{
		public int Id { get; set; }
		public int CityId { get; set; }
		public int Age { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Description { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Viber { get; set; }
		public string ImageLink { get; set; }
		public List<Images> Images { get; set; }

	}

	public class Images
	{
		public string ImageLink { get; set; }
		public bool ShouldShowWatermark { get; set; }
	}


}

