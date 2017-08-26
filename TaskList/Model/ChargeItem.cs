using System;
using System.ComponentModel;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using TaskList.BaseClass;

namespace TaskList.Model
{
    public class ChargeItem : PropertyChangeBase
    {
        string id;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

		string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string narrowDownName;
        public string NarrowDownName
        {
            get { return string.IsNullOrEmpty(narrowDownName) ? name : narrowDownName; }
            set { narrowDownName = value; }
        }

        public Action<object> ClickedAction { get; set; }

		private bool isSelected;
		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				isSelected = value;
				RaisePropertyChanged();
			}
		}
    }
}

