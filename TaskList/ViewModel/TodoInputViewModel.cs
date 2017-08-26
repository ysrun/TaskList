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
			}
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
