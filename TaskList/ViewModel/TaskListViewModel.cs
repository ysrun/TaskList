using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TaskList.BaseClass;
using TaskList.Model;
using Xamarin.Forms;

namespace TaskList.ViewModel
{
    public class TaskListViewModel : PropertyChangeBase
    {
        public TaskListViewModel()
        {
            Initialize();
        }
        private void Initialize()
        {
            ModeChangeClickAction = (o) =>
            {
                TodoItemManager.DefaultManager.IsDoingTaskShow = !TodoItemManager.DefaultManager.IsDoingTaskShow;
                RaisePropertyChanged("ModeButtonText");
                RaisePropertyChanged("ModeText");
                IsRefresh = true;
            };
            ApperingAction = () =>
            {
                IsRefresh = true;
            };

            AddClickAction = async (o) =>
            {
                var item = CreateTodoItem();
                if (TodoItemManager.DefaultManager.IsDoingTaskShow)
                {
                    if(TaskList.Count > 0)
                    {
                        TaskList.Insert(0,item);
                    }
                    else
                    {
						TaskList.Add(item);
					}
					RaisePropertyChanged("VisibleTaskList");
					RaisePropertyChanged("TaskCount");
                }
                TaskText = null;
                await SaveItem(item);
            };
            ListRefreshAction = () =>
            {
                return RefreshList();
            };

            Action<object> clickedAction = (obj) => 
            {
                if(obj is ChargeItem)
                {
                    (obj as ChargeItem).IsSelected = !(obj as ChargeItem).IsSelected;
                    SaveChargeItemState();
                    RaisePropertyChanged("VisibleTaskList");
                    RaisePropertyChanged("TaskCount");
                }
            };

            ChargeItems = new ObservableCollection<ChargeItem>() 
            {
                new ChargeItem(){Name = "なし", NarrowDownName = "担当者なし",Id = null,IsSelected = true,ClickedAction = clickedAction},
				new ChargeItem(){Name = "智ちゃん",Id = "tomoko",IsSelected = true,ClickedAction = clickedAction},
                new ChargeItem(){Name = "義明",Id = "yoshiaki",IsSelected = true,ClickedAction = clickedAction},
            };
            LoadChaegeItemsState();
            SelectedChargeItem = ChargeItems.First();
            LimitDate = DateTime.Now;
        }

        private void LoadChaegeItemsState()
        {
			foreach (var item in ChargeItems)
			{
                if(Application.Current.Properties.ContainsKey("ChargeItem_" + item.Name))
                {
                    item.IsSelected = (bool)Application.Current.Properties["ChargeItem_" + item.Name];
                }
			}
        }

        private void SaveChargeItemState()
        {
            foreach(var item in ChargeItems)
            {
                Application.Current.Properties["ChargeItem_" + item.Name] = item.IsSelected;
            }
        }

        private TodoItem CreateTodoItem(string name = "完了")
        {

            var item = new TodoItem()
            {
                Name = TaskText,
                ButtonCaption = name,
                IsSetLimit = IsUseLimitDate,
                UserName = SelectedChargeItem.Id,
            };
            if(IsUseLimitDate)
            {
                item.LimitDate = LimitDate;
            }
            SetAction(item);
            return item;
        }
        private void SetAction(TodoItem item)
        {
            Action<object> clickedAction = async (obj) =>
            {
                if (obj is TodoItem)
                {
                    TaskList.Remove(obj as TodoItem);
                    (obj as TodoItem).Done = !(obj as TodoItem).Done;
                    (obj as TodoItem).CompleteDate = DateTime.Now;
                    await SaveItem(obj as TodoItem);
					RaisePropertyChanged("VisibleTaskList");
					RaisePropertyChanged("TaskCount");
                }
            };

            Action<object> starclickedAction = async (obj) =>
            {
                if (obj is TodoItem)
                {
                    (obj as TodoItem).Priority = (obj as TodoItem).Priority == 1 ? 0 : 1;
                    await SaveItem(obj as TodoItem);
                }
            };

            Action<object> deleteMenuClickAction = async (obj) =>
            {
                if (obj is TodoItem)
                {
                    try
                    {
                        TaskList.Remove(obj as TodoItem);
                        await DeleteItem(obj as TodoItem);
                        RaisePropertyChanged("VisibleTaskList");
                        RaisePropertyChanged("TaskCount");
                    }
                    catch
                    {
                        IsRefresh = true;
                    }
                }
            };
            item.ClickAction = clickedAction;
            item.StarClickAction = starclickedAction;
            item.DeleteMenuClickAction = deleteMenuClickAction;
        }


        private async Task RefreshList()
        {
            TaskList = null;
            var items = await TodoItemManager.DefaultManager.GetTodoItemsAsync();
            IsDoingTaskShow = TodoItemManager.DefaultManager.IsDoingTaskShow;
            if (items != null && items.Count > 0)
            {
                foreach (TodoItem item in items)
                {
                    SetAction(item);
                    item.ButtonCaption = TodoItemManager.DefaultManager.IsDoingTaskShow ? "完了" : "実行中に戻す";
                }
            }
            TaskList = items;
            IsRefresh = false;
        }
        private async Task SaveItem(TodoItem targetitem)
        {
            await TodoItemManager.DefaultManager.SaveTaskAsync(targetitem);
        }

        private async Task DeleteItem(TodoItem targetItem)
        {
            await TodoItemManager.DefaultManager.DeleteTaskAsync(targetItem);
        }

        #region Property

        private ObservableCollection<TodoItem> _taskList = new ObservableCollection<TodoItem>();
        public ObservableCollection<TodoItem> TaskList
        {
            get
            {
                return _taskList;
            }
            set
            {
                _taskList = value;
                RaisePropertyChanged();
                RaisePropertyChanged("VisibleTaskList");
                RaisePropertyChanged("TaskCount");
            }
        }
        public ObservableCollection<TodoItem> VisibleTaskList
        {
            get
            {
                if (_taskList == null)
                    return null;

                var selectedlist = ChargeItems.Where((citem) => citem.IsSelected);
                return new ObservableCollection<TodoItem>(_taskList.Where((item) => selectedlist.Select((sitem) => sitem.Id).Contains(item.UserName)));
            }
        }
        public int TaskCount
        {
            get { return _taskList != null ? 0 : _taskList.Count; }
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
        public Action ApperingAction { get; set; }
        public Action<object> AddClickAction { get; set; }
        public Action<object> DeleteMenuClickAction { get; set; }
        public Action<object> ModeChangeClickAction { get; set; }
        public Func<Task> ListRefreshAction { get; set; }

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
        private bool _isDoingTaskShow = true;
        public bool IsDoingTaskShow
        {
            get { return _isDoingTaskShow; }
            set
            {
                _isDoingTaskShow = value;
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
        private bool _isRefresh;
        public bool IsRefresh
        {
            get
            {
                return _isRefresh;
            }
            set
            {
                _isRefresh = value;
                RaisePropertyChanged();
            }
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
                if(!value)
                {
                    IsUseLimitDate = false;
                    LimitDate = DateTime.Now;
                    SelectedChargeItem = ChargeItems != null ? ChargeItems.First() : null;
                }
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
        public string ModeButtonText
        {
            get { return TodoItemManager.DefaultManager.IsDoingTaskShow ? "完了タスクを表示" : "実行中タスクを表示"; }
        }
        public string ModeText
        {
            get { return TodoItemManager.DefaultManager.IsDoingTaskShow ? "実行中タスクを表示中" : "完了タスクを表示中"; }
        }

        #endregion
    }
}
