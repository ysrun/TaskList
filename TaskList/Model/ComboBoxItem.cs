using System;
namespace TaskList.Model
{
    public class ComboBoxItem
    {
        public ComboBoxItem()
        {
        }
		string id;
		public string Id
		{
			get { return id; }
			set { id = value; }
		}
        int no;
        public int No
        {
            get { return no; }
            set { no = value; }
        }

		string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
    }
}
