using System;
using System.Collections.ObjectModel;
using TaskList.BaseClass;
using TaskList.Model;
using System.Linq;

namespace TaskList.ViewModel
{
    public class TodoInputViewModel:PropertyChangeBase
    {
        public TodoInputViewModel(Action<object> clickedAction):base()
        {
            TaskText = null;
			ChargeItems = new ObservableCollection<ChargeItem>()
			{
				new ChargeItem(){Name = "なし", NarrowDownName = "担当者なし",Id = null,IsSelected = true,ClickedAction = clickedAction},
				new ChargeItem(){Name = "智ちゃん",Id = "tomoko",IsSelected = true,ClickedAction = clickedAction},
				new ChargeItem(){Name = "義明",Id = "yoshiaki",IsSelected = true,ClickedAction = clickedAction},
			};
			SelectedChargeItem = ChargeItems.First();
            RegularItems = new ObservableCollection<ComboBoxItem>() 
            {
                new ComboBoxItem(){Name = "毎日",No = 0},
                new ComboBoxItem(){Name = "毎週",No = 1},
                new ComboBoxItem(){Name = "毎月",No = 2},
            };
            SelectedRegularItem = RegularItems.First();
            WeekItems = new ObservableCollection<ComboBoxItem>()
            {
				new ComboBoxItem(){Name = "日曜日",No = 0},
				new ComboBoxItem(){Name = "月曜日",No = 1},
				new ComboBoxItem(){Name = "火曜日",No = 2},
			    new ComboBoxItem(){Name = "水曜日",No = 3},
				new ComboBoxItem(){Name = "木曜日",No = 4},
				new ComboBoxItem(){Name = "金曜日",No = 5},
                new ComboBoxItem(){Name = "土曜日",No = 6},
            };
            SelectedWeekItem = WeekItems.First();
            MonthItems = new ObservableCollection<ComboBoxItem>()
			{
				new ComboBoxItem(){Name = "1日",No = 1},
                new ComboBoxItem(){Name = "2日",No = 2},
                new ComboBoxItem(){Name = "3日",No = 3},
                new ComboBoxItem(){Name = "4日",No = 4},
                new ComboBoxItem(){Name = "5日",No = 5},
                new ComboBoxItem(){Name = "6日",No = 6},
                new ComboBoxItem(){Name = "7日",No = 7},
                new ComboBoxItem(){Name = "8日",No = 8},
                new ComboBoxItem(){Name = "9日",No = 9},
                new ComboBoxItem(){Name = "10日",No = 10},
				new ComboBoxItem(){Name = "11日",No = 11},
				new ComboBoxItem(){Name = "12日",No = 12},
				new ComboBoxItem(){Name = "13日",No = 13},
				new ComboBoxItem(){Name = "14日",No = 14},
				new ComboBoxItem(){Name = "15日",No = 15},
				new ComboBoxItem(){Name = "16日",No = 16},
				new ComboBoxItem(){Name = "17日",No = 17},
				new ComboBoxItem(){Name = "18日",No = 18},
				new ComboBoxItem(){Name = "19日",No = 19},
				new ComboBoxItem(){Name = "20日",No = 20},
				new ComboBoxItem(){Name = "21日",No = 21},
				new ComboBoxItem(){Name = "22日",No = 22},
				new ComboBoxItem(){Name = "23日",No = 23},
				new ComboBoxItem(){Name = "24日",No = 24},
				new ComboBoxItem(){Name = "25日",No = 25},
				new ComboBoxItem(){Name = "26日",No = 26},
				new ComboBoxItem(){Name = "27日",No = 27},
				new ComboBoxItem(){Name = "28日",No = 28},
				new ComboBoxItem(){Name = "29日",No = 29},
				new ComboBoxItem(){Name = "30日",No = 30},
                new ComboBoxItem(){Name = "31日",No = 31},
			};
            SelectedMonthItem = MonthItems.First();

			LimitDate = DateTime.Now;
        }

		private bool _isShowDetail;
		public bool IsShowDetail
		{
			get
			{
				return _isShowDetail;
			}
			set
			{
				_isShowDetail = value;
				if (!value)
				{
					IsUseLimitDate = false;
					LimitDate = DateTime.Now;
                    IsUseRegular = false;
                    SelectedRegularItem = RegularItems != null ? RegularItems.First() : null;
                    SelectedWeekItem = WeekItems != null ? WeekItems.First() : null;
                    SelectedMonthItem = MonthItems != null ? MonthItems.First() : null;
					SelectedChargeItem = ChargeItems != null ? ChargeItems.First() : null;
				}
				RaisePropertyChanged();
			}
		}
		private string _buttonCaption;
		public string ButtonCaption
		{
			get
			{
				return _buttonCaption;
			}
			set
			{
				_buttonCaption = value;
				RaisePropertyChanged();
			}
		}

        public Action<object> AddClickAction { get; set; }
        public Action<object> CancelAction { get; set; }

		private DateTime _limitDate;
		public DateTime LimitDate
		{
			get { return _limitDate; }
			set
			{
				_limitDate = value;
				RaisePropertyChanged();
			}
		}
		private string _taskText;
		public string TaskText
		{
			get
			{
				return _taskText;
			}
			set
			{
				_taskText = value;
				RaisePropertyChanged();
                RaisePropertyChanged("IsNotEmptyTaskText");
			}
		}
        public bool IsNotEmptyTaskText
        {
            get { return !string.IsNullOrEmpty(TaskText); }
        }
		private bool _isUseLimitDate;
		public bool IsUseLimitDate
		{
			get
			{
				return _isUseLimitDate;
			}
			set
			{
				_isUseLimitDate = value;
                if (value)
                    IsUseRegular = false;
				RaisePropertyChanged();
			}
		}
		private bool _isUseRegular;
		public bool IsUseRegular
		{
			get
			{
				return _isUseRegular;
			}
			set
			{
				_isUseRegular = value;
                if (value)
                    IsUseLimitDate = false;
				RaisePropertyChanged();
			}
		}

        private ObservableCollection<ComboBoxItem> _regularItems;
		public ObservableCollection<ComboBoxItem> RegularItems
		{
			get { return _regularItems; }
			set
			{
				_regularItems = value;
				RaisePropertyChanged();
			}
		}

		private ObservableCollection<ChargeItem> _chargeItems;
		public ObservableCollection<ChargeItem> ChargeItems
		{
			get { return _chargeItems; }
			set
			{
				_chargeItems = value;
				RaisePropertyChanged();
			}
		}

		private ComboBoxItem _selectedRegularItem;
		public ComboBoxItem SelectedRegularItem
		{
			get { return _selectedRegularItem; }
			set
			{
				_selectedRegularItem = value;
				RaisePropertyChanged();
			}
		}

		private ChargeItem _selectedChargeItem;
		public ChargeItem SelectedChargeItem
		{
			get { return _selectedChargeItem; }
			set
			{
				_selectedChargeItem = value;
				RaisePropertyChanged();
			}
		}

		private ObservableCollection<ComboBoxItem> _weekItems;
		public ObservableCollection<ComboBoxItem> WeekItems
		{
			get { return _weekItems; }
			set
			{
				_weekItems = value;
				RaisePropertyChanged();
			}
		}
		private ComboBoxItem _selectedWeekItem;
		public ComboBoxItem SelectedWeekItem
		{
			get { return _selectedWeekItem; }
			set
			{
				_selectedWeekItem = value;
				RaisePropertyChanged();
			}
		}
		private ObservableCollection<ComboBoxItem> _monthItems;
		public ObservableCollection<ComboBoxItem> MonthItems
		{
			get { return _monthItems; }
			set
			{
				_monthItems = value;
				RaisePropertyChanged();
			}
		}
		private ComboBoxItem _selectedMonthItem;
		public ComboBoxItem SelectedMonthItem
		{
			get { return _selectedMonthItem; }
			set
			{
				_selectedMonthItem = value;
				RaisePropertyChanged();
			}
		}
		private bool _isEdit;
		public bool IsEdit
		{
			get { return _isEdit; }
			set
			{
				_isEdit = value;
				RaisePropertyChanged();
			}
		}
		private bool _isDetail;
		public bool IsDetail
		{
			get { return _isDetail; }
			set
			{
				_isDetail = value;
				RaisePropertyChanged();
			}
		}
        public bool IsEnabled
        {
            get { return IsEdit || IsCreate; }
        }
		private bool _isCreate;
		public bool IsCreate
		{
			get { return _isCreate; }
			set
			{
				_isCreate = value;
				RaisePropertyChanged();
			}
		}
        public string Id { get; set; }
    }
}
