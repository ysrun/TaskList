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
            Action<object> clickedaction = (obj) =>
            {
                if (obj is ChargeItem)
                {
                    (obj as ChargeItem).IsSelected = !(obj as ChargeItem).IsSelected;
                    SaveChargeItemState();
                    RaisePropertyChanged("VisibleTaskList");
                    RaisePropertyChanged("TaskCount");
                }
            };

            LimitItems = new ObservableCollection<ChargeItem>()
			{
				new ChargeItem(){Name = "期限なし", Id = "nolimit",IsSelected = true,ClickedAction = clickedaction},
				new ChargeItem(){Name = "期限あり",Id = "limit",IsSelected = true,ClickedAction = clickedaction},
				new ChargeItem(){Name = "繰り返し",Id = "loop",IsSelected = true,ClickedAction = clickedaction},
			};

            TodoInputViewModel = new TodoInputViewModel(clickedaction);

            TodoInputViewModel.ButtonCaption = "追加";
            TodoInputViewModel.IsCreate = true;
			TodoInputViewModel.AddClickAction = async (o) =>
			{
                var item = CreateTodoItem(TodoInputViewModel);
				TodoInputViewModel.TaskText = string.Empty;
                if (TodoItemManager.DefaultManager.IsDoingTaskShow)
                {
                    if (TaskList.Count > 0)
                    {
                        TaskList.Insert(0, item);
                    }
                    else
                    {
                        TaskList.Add(item);
                    }
                    RaisePropertyChanged("VisibleTaskList");
                    RaisePropertyChanged("TaskCount");
                }
                await SaveItem(item);
			};

			TodoEditViewModel = new TodoInputViewModel(null);
			TodoEditViewModel.ButtonCaption = "OK"; 
			TodoEditViewModel.AddClickAction = async (o) =>
			{
                if (o is TodoInputViewModel)
                {
                    var item = CreateTodoItem(o as TodoInputViewModel);

                    if (TodoItemManager.DefaultManager.IsDoingTaskShow)
                    {
                        //Idから同じアイテムを探す
                        var updtitem = TaskList.Where((itm) => itm.Id == item.Id).FirstOrDefault();
                        if (updtitem != null)
                        {
                            //編集画面で書き換えない情報を渡す
                            item.Priority = updtitem.Priority;
                            //アイテムを削除し再挿入
                            int index = TaskList.IndexOf(updtitem);
                            TaskList.Remove(updtitem);
                            TaskList.Insert(index, item);
                        }
                        else
                        {
                            TaskList.Add(item);
                        }
						RaisePropertyChanged("VisibleTaskList");
                    }

                    await SaveItem(item);
                    if (TodoEditViewModel.CancelAction != null)
                    {
                        TodoEditViewModel.CancelAction(null);
                    }
                }
			};

            ModeChangeClickAction = (o) =>
            {
                TodoItemManager.DefaultManager.IsDoingTaskShow = !TodoItemManager.DefaultManager.IsDoingTaskShow;
                IsDoingTaskShow = TodoItemManager.DefaultManager.IsDoingTaskShow;
                RaisePropertyChanged("ModeButtonText");
                RaisePropertyChanged("ModeText");
				RaisePropertyChanged("VisibleTaskList");
				RaisePropertyChanged("TaskCount");
                //IsRefresh = true;
            };
            ApperingAction = () =>
            {
                if(!isLoaded)
                {
                    isLoaded = true;
                    IsRefresh = true;
                }
            };

            ListRefreshAction = () =>
            {
                return RefreshList();
            };

            LoadChaegeItemsState();

        }

        private void LoadChaegeItemsState()
        {
			foreach (var item in TodoInputViewModel.ChargeItems)
			{
                if(Application.Current.Properties.ContainsKey("ChargeItem_" + item.Name))
                {
                    item.IsSelected = (bool)Application.Current.Properties["ChargeItem_" + item.Name];
                }
			}
            foreach (var item in LimitItems)
			{
				if (Application.Current.Properties.ContainsKey("ChargeItem_" + item.Name))
				{
					item.IsSelected = (bool)Application.Current.Properties["ChargeItem_" + item.Name];
				}
			}
        }

        private void SaveChargeItemState()
        {
            foreach(var item in TodoInputViewModel.ChargeItems)
            {
                Application.Current.Properties["ChargeItem_" + item.Name] = item.IsSelected;
            }
            foreach (var item in LimitItems)
			{
				Application.Current.Properties["ChargeItem_" + item.Name] = item.IsSelected;
			}        
        }

        private TodoItem CreateTodoItem(TodoInputViewModel targetViewModel)
        {

            var item = new TodoItem()
            {
                Name = targetViewModel.TaskText,
                //ButtonCaption = name,
                IsSetLimit = targetViewModel.IsUseLimitDate,
                UserName = targetViewModel.SelectedChargeItem.Id,
            };
            if(targetViewModel.IsUseLimitDate)
            {
                item.LimitDate = targetViewModel.LimitDate;
                item.IsSetLimit = true;
            }
            if(targetViewModel.IsUseRegular)
            {
                item.IsRegularTask = true;
                item.RegularTaskType = targetViewModel.SelectedRegularItem.No;
                DateTime nowdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                switch (item.RegularTaskType)
                {
                    case 0:
                        item.LimitDate = nowdate;
                        break;
                    case 1:
                        item.RegularTaskData = targetViewModel.SelectedWeekItem.No.ToString();
                        int weekno;
                        if(int.TryParse(item.RegularTaskData,out weekno))
                        {
                            if ((int)DateTime.Now.DayOfWeek == weekno)
                            {
                                item.LimitDate = nowdate;
                            }
                            if((int)DateTime.Now.DayOfWeek < weekno)
                            {
                                item.LimitDate = nowdate.AddDays(weekno - (int)DateTime.Now.DayOfWeek);
                            }
                            if((int)DateTime.Now.DayOfWeek > weekno)
                            {
                                item.LimitDate = nowdate.AddDays(7 - ((int)DateTime.Now.DayOfWeek - weekno));
                            }
                        }
                        break;
                    case 2:
                        item.RegularTaskData = targetViewModel.SelectedMonthItem.No.ToString();
						int day;
                        if (int.TryParse(item.RegularTaskData, out day))
                        {
                            int daysinmonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                            item.LimitDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day > daysinmonth ? daysinmonth : day);
                            if(day < DateTime.Now.Day)
                            {
                                item.LimitDate = item.LimitDate.AddMonths(1);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            if(!string.IsNullOrEmpty(targetViewModel.Id))
            {
                item.Id = targetViewModel.Id;
            }
            SetAction(item);
            return item;
        }
        private void SetAction(TodoItem item)
        {
			Action<object> stateChangeButtonClickedAction = new Action<object>(async (obj) =>
			{
				if (obj is TodoItem)
				{
                    TodoItem todoitem = obj as TodoItem;
                    if(!todoitem.IsRegularTask)
                    {
                        //リストちらつき対策、一旦リストからデータを消す
					    TaskList.Remove(todoitem);
                    }
                    else
                    {
                        //Idがnullの複製を用いて、履歴のInsertを実施
                        todoitem = todoitem.GetClone();
                        todoitem.Id = null;
                    }
                    RaisePropertyChanged("VisibleTaskList");
					RaisePropertyChanged("TaskCount");
					todoitem.Done = !todoitem.Done;
					todoitem.CompleteDate = DateTime.Now;
					await SaveItem(todoitem);
					//定期タスク以外は、リストちらつき対策として消したデータを戻す
                    //定期タスクは、リストに履歴を追加
					TaskList.Add(todoitem);
                    if (todoitem.IsRegularTask)
                    {
                        //次回繰り返し日付の設定
                        todoitem = obj as TodoItem;
                        var nowdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
						switch (todoitem.RegularTaskType)
						{
							case 0:
                                todoitem.LimitDate = todoitem.LimitDate.AddDays(1);
								break;
							case 1:
								todoitem.LimitDate = todoitem.LimitDate.AddDays(7);
								break;
							case 2:
                                var nextmonth = todoitem.LimitDate.AddMonths(1);
								int day;
								if (int.TryParse(todoitem.RegularTaskData, out day))
								{
									int daysinmonth = DateTime.DaysInMonth(nextmonth.Year, nextmonth.Month);
                                    todoitem.LimitDate = new DateTime(nextmonth.Year, nextmonth.Month, day > daysinmonth ? daysinmonth : day);
								}
								break;
							default:
								break;
						}
                        //日付変更後の繰り返しデータもアップデート
                        await SaveItem(todoitem);
                    }
				}
			});
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
                if (obj is TodoItem && CheckedDelete != null && await CheckedDelete((obj as TodoItem).Name))
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
            Action<object> editMenuClickAction = async (obj) =>
            {
                if (obj is TodoItem)
                {
                    TodoEditViewModel.IsShowDetail = false;
                    TodoEditViewModel.IsEdit = !(obj as TodoItem).Done;
                    TodoEditViewModel.IsDetail = (obj as TodoItem).Done;
                    TodoEditViewModel.Id = (obj as TodoItem).Id;
					TodoEditViewModel.TaskText = (obj as TodoItem).Name;
					if (!string.IsNullOrEmpty((obj as TodoItem).UserName))
					{
						TodoEditViewModel.IsShowDetail = true;
						TodoEditViewModel.SelectedChargeItem = TodoEditViewModel.ChargeItems.Where((itm) => itm.Id == (obj as TodoItem).UserName).FirstOrDefault();
					}

					if ((obj as TodoItem).LimitDate > new DateTime(1900, 1, 1))
					{
						TodoEditViewModel.IsShowDetail = true;
						TodoEditViewModel.IsUseLimitDate = true;
						TodoEditViewModel.LimitDate = (obj as TodoItem).LimitDate;
					}
                    if((obj as TodoItem).IsRegularTask)
                    {
                        TodoEditViewModel.IsShowDetail = true;
                        TodoEditViewModel.IsUseRegular = true;
                        TodoEditViewModel.SelectedRegularItem = TodoEditViewModel.RegularItems.Where((itm) => itm.No == (obj as TodoItem).RegularTaskType).FirstOrDefault();
                        if (TodoEditViewModel.SelectedRegularItem != null)
                        {
                            switch (TodoEditViewModel.SelectedRegularItem.No)
                            {
                                case 1:
                                    int week;
                                    if(int.TryParse((obj as TodoItem).RegularTaskData,out week))
                                    {
                                        TodoEditViewModel.SelectedWeekItem = TodoEditViewModel.WeekItems.Where((itm) => itm.No == week).FirstOrDefault();                                        
                                    }
                                    break;
                                case 2:
									int month;
									if (int.TryParse((obj as TodoItem).RegularTaskData, out month))
									{
										TodoEditViewModel.SelectedMonthItem = TodoEditViewModel.MonthItems.Where((itm) => itm.No == month).FirstOrDefault();
									}
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
					await TodoEdit();                    
                }
            };
            item.ClickAction = stateChangeButtonClickedAction;
            item.StarClickAction = starclickedAction;
            item.DeleteMenuClickAction = deleteMenuClickAction;
            item.EditMenuClickAction = editMenuClickAction;
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
                    //item.ButtonCaption = TodoItemManager.DefaultManager.IsDoingTaskShow ? "完了" : "実行中に戻す";
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

                var selectedlist = TodoInputViewModel.ChargeItems.Where((citem) => citem.IsSelected);

                var tasklist = _taskList.Where((item) => selectedlist.Select((sitem) => sitem.Id).Contains(item.UserName) 
                                               && item.Done != IsDoingTaskShow);
                bool isnolimitselected = LimitItems.Where((item) => item.Id == "nolimit" && item.IsSelected).Count() != 0;
                bool islimitselected = LimitItems.Where((item) => item.Id == "limit" && item.IsSelected).Count() != 0;
                bool isloopselected = LimitItems.Where((item) => item.Id == "loop" && item.IsSelected).Count() != 0;

                tasklist = tasklist.Where((item)=> (item.IsSetLimit && islimitselected) || (item.IsRegularTask && isloopselected) || (!item.IsSetLimit && !item.IsRegularTask && isnolimitselected));

                //

                if (!IsDoingTaskShow)
					return new ObservableCollection<TodoItem>(tasklist
                                                              .OrderByDescending(item => item.createdAt)
															  .OrderByDescending(item => item.CompleteDate));
                else
					return new ObservableCollection<TodoItem>(tasklist
                                                              .OrderByDescending(item => item.createdAt)
                                                              .OrderBy(item => item.SortDate)
                                                              .OrderByDescending(item => item.Priority));
            }
        }
        public int TaskCount
        {
            get { return _taskList != null ? 0 : _taskList.Count; }
        }

        public Func<string,Task<bool>> CheckedDelete { get; set; }
        public Func<Task<bool>> TodoEdit { get; set; }
        public TodoInputViewModel TodoInputViewModel { get; set; }
        public TodoInputViewModel TodoEditViewModel { get; set; }
        public Action ApperingAction { get; set; }
        public Action<object> ModeChangeClickAction { get; set; }
        public Func<Task> ListRefreshAction { get; set; }


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

		private ObservableCollection<ChargeItem> _limitItems;
		public ObservableCollection<ChargeItem> LimitItems
		{
			get { return _limitItems; }
			set
			{
				_limitItems = value;
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
        private bool isLoaded { get; set; }

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
