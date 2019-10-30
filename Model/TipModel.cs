namespace techsearch.Model
{
	class TipModel
	{
		private string _name;
		public string Name
		{
			get
			{
				return this._name?.Replace('.', '-');
			}
			set
			{
				this._name = value;
			}
		}
		public string Command { get; set; }
		public string Description { get; set; }
		public string Url { get; set; }
		public string Category { get; set; }

		public override string ToString()
		{
			return $"{Category} {Name}";
		}
	}
} 
