using System;
using System.ComponentModel;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using TaskList.BaseClass;

namespace TaskList.Model
{
    public class money : PropertyChangeBase
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
        int _money;
		[JsonProperty(PropertyName = "Money")]
		public int Money
		{
			get { return _money; }
			set { _money = value; }
		}
		bool _isminus;
		[JsonProperty(PropertyName = "IsMinus")]
		public bool IsMinus
		{
			get { return _isminus; }
			set { _isminus = value; }
        }
		private DateTime date;
		[JsonProperty(PropertyName = "Date")]
		public DateTime Date
		{
			get { return date; }
			set { date = value; }
		}

		[JsonIgnore]
		public Action<object> DeleteMenuClickAction { get; set; }
    }
}

