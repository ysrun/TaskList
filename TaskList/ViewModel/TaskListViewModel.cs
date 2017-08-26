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
            TodoInputViewModel = new TodoInputViewModel((obj) =>
            {
                if (obj is ChargeItem)
                {
                    (obj as ChargeItem).IsSelected = !(obj as ChargeItem).IsSelected;
                    SaveChargeItemState();
                    RaisePropertyChanged("VisibleTaskList");
                    RaisePropertyChanged("TaskCount");
                }
            });
            TodoInputViewModel.ButtonCaption = "追加";
            TodoInputViewModel.IsCreate = true;
			TodoInputViewModel.AddClickAction = async (o) =>
			{
                var item = CreateTodoItem(TodoInputViewModel);
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
                TodoInputViewModel.TaskText = string.Empty;
			};

			TodoEditViewModel = new TodoInputViewModel(null);
			TodoEditViewModel.ButtonCaption = "OK"; 
            TodoEditViewModel.IsEdit = true;
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
                RaisePropertyChanged("ModeButtonText");
                RaisePropertyChanged("ModeText");
                IsRefresh = true;
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
        }

        private void SaveChargeItemState()
        {
            foreach(var item in TodoInputViewModel.ChargeItems)
            {
                Application.Current.Properties["ChargeItem_" + item.Name] = item.IsSelected;
            }
        }

        private TodoItem CreateTodoItem(TodoInputViewModel targetViewModel, string name = "完了")
        {

            var item = new TodoItem()
            {
                Name = targetViewModel.TaskText,
                ButtonCaption = name,
                IsSetLimit = targetViewModel.IsUseLimitDate,
                UserName = targetViewModel.SelectedChargeItem.Id,
            };
            if(targetViewModel.IsUseLimitDate)
            {
                item.LimitDate = targetViewModel.LimitDate;
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
					TaskList.Remove(obj as TodoItem);
					(obj as TodoItem).Done = !(obj as TodoItem).Done;
					(obj as TodoItem).CompleteDate = DateTime.Now;
					await SaveItem(obj as TodoItem);
					RaisePropertyChanged("VisibleTaskList");
					RaisePropertyChanged("TaskCount");
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
                if (obj is TodoItem && CheckedDelete != null && await CheckedDelete())
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

                var selectedlist = TodoInputViewModel.ChargeItems.Where((citem) => citem.IsSelected);
                return new ObservableCollection<TodoItem>(_taskList.Where((item) => selectedlist.Select((sitem) => sitem.Id).Contains(item.UserName)));
            }
        }
        public int TaskCount
        {
            get { return _taskList != null ? 0 : _taskList.Count; }
        }

        public Func<Task<bool>> CheckedDelete { get; set; }
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
