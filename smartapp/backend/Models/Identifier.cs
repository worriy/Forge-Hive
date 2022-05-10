using System;
using System.ComponentModel.DataAnnotations;

namespace Hive.Backend.DataModels
{
	public abstract class Identifier 
	{
		public Identifier() 
		{
		}

		public Guid Id { get; set; }
	}
}