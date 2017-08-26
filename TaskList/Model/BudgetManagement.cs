using System;
using Newtonsoft.Json;
using TaskList.BaseClass;

namespace TaskList.Model
{
    public class BudgetManagement: PropertyChangeBase
	{
		string _id;
		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}
		string _comment;
		[JsonProperty(PropertyName = "Comment")]
		public string Comment
		{
			get { return _comment; }
			set { _comment = value; }
		}
		int _amount;
		[JsonProperty(PropertyName = "Amount")]
		public int Amount
		{
			get { return _amount; }
			set { _amount = value; }
		}
		bool _isExpence;
		[JsonProperty(PropertyName = "IsExpence")]
		public bool IsMinus
		{
			get { return _isExpence; }
			set { _isExpence = value; }
		}
		private DateTime _inputDate;
		[JsonProperty(PropertyName = "InputDate")]
		public DateTime Date
		{
			get { return _inputDate; }
			set { _inputDate = value; }
		}

		[JsonIgnore]
		public Action<object> DeleteMenuClickAction { get; set; }

    }
}
