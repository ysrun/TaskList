using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TaskList.BaseClass;
using TaskList.Model;

namespace TaskList.ViewModel
{
    public class MoneyPageViewModel : PropertyChangeBase
    {
        public MoneyPageViewModel()
        {
            Initialize();
        }
        private void Initialize()
        {
            ApperingAction = () =>
            {
                IsRefresh = true;
            };

            AddClickAction = async (o) =>
            {
                await AddItem();
            };
			MinusClickAction = async (o) =>
			{
                await AddItem(true);
			};
            ListRefreshAction = () =>
            {
                return RefreshList();
            };
            Date = DateTime.Now;
        }
        private async Task AddItem(bool isMinus = false)
        {
			var item = Createmoney(isMinus);
			MoneyList.Add(item);
			MoneyText = null;
			Comment = null;
			await SaveItem(item);
			int total = 0;
			foreach (money m in MoneyList)
			{
				total = m.IsMinus ? total - m.Money : total + m.Money;
			}
			Total = "残高：" + total.ToString("n0") + "円";
        }

        private money Createmoney(bool isminus = false)
        {
            int moneyint = 0;
            money item = null;
            if(int.TryParse(MoneyText, out moneyint))
            {
                item = new money()
                {
                    Money = moneyint,
                    Comment = Comment,
                    IsMinus = isminus,
                    Date = Date,
				};
				SetAction(item);
            }
            return item;
        }
        private void SetAction(money item)
        {
			Action<object> deleteMenuClickAction = async (obj) =>
			{
                if (obj is money)
				{
					try
					{
                        MoneyList.Remove(obj as money);
						await DeleteItem(obj as money);
						int total = 0;
                        foreach (money m in MoneyList)
						{
                            total = m.IsMinus ? total - m.Money : total + m.Money;
						}
						Total = "残高：" + total.ToString("n0") + "円";
					}
					catch
					{
						IsRefresh = true;
					}
				}
			};
            item.DeleteMenuClickAction = deleteMenuClickAction;
        }


        private async Task RefreshList()
        {
            MoneyList = null;
            var items = await moneyManager.DefaultManager.GetMoneyItemsAsync();

            int total = 0;
            foreach (money item in items)
            {
                SetAction(item);
                total = item.IsMinus ? total - item.Money : total + item.Money;
            }
            MoneyList = items;
            Total = "残高：" + total.ToString("n0") + "円";
            IsRefresh = false;
        }
        private async Task SaveItem(money targetitem)
        {
            await moneyManager.DefaultManager.SaveTaskAsync(targetitem);
        }

        private async Task DeleteItem(money targetItem)
        {
            await moneyManager.DefaultManager.DeleteTaskAsync(targetItem);
        }

        #region Property

        private ObservableCollection<money> _moneyList = new ObservableCollection<money>();
        public ObservableCollection<money> MoneyList
        {
            get
            {
                return _moneyList;
            }
            set
            {
                _moneyList = value;
                RaisePropertyChanged();
                RaisePropertyChanged("VisibleMoneyList");
            }
        }
        public ObservableCollection<money> VisibleMoneyList
        {
            get
            {
                return _moneyList == null ? null : _moneyList;
            }
        }
        public Action ApperingAction { get; set; }
        public Action<object> AddClickAction { get; set; }
        public Action<object> MinusClickAction { get; set; }
        public Func<Task> ListRefreshAction { get; set; }


        private string _moneyText;
        public string MoneyText
        {
            get
            {
                return _moneyText;
            }
            set
            {
                _moneyText = value;
                RaisePropertyChanged();
            }
        }
        private string _total;
        public string Total
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value;
                RaisePropertyChanged();
            }
        }

		private string _comment;
		public string Comment
		{
			get
			{
				return _comment;
			}
			set
			{
				_comment = value;
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
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                RaisePropertyChanged();
            }
        }
        #endregion
    }
}
