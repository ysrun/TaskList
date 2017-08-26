using System.Threading.Tasks;
using TaskList.ViewModel;
using Xamarin.Forms;

namespace TaskList
{
    public partial class TaskListPage : ContentPage
    {
        public TaskListPage()
        {
            InitializeComponent();
            SetDialog();
            this.BindingContextChanged += (sender, e) =>
            {
                SetDialog();
            };
        }
        private void SetDialog()
        {
			if (BindingContext is TaskListViewModel)
			{
				(BindingContext as TaskListViewModel).CheckedDelete = async () =>
				{
					return await DisplayAlert("確認", "削除します、宜しいですか？", "はい", "いいえ");
				};
                (BindingContext as TaskListViewModel).TodoEdit = async () =>
				{
                    TodoInputView view = new TodoInputView(){Margin = new Thickness(0,20,0,0)};
                    (BindingContext as TaskListViewModel).TodoEditViewModel.CancelAction = (obj) => 
                    {
                        App.Root.NavigateTo(typeof(TaskListPage));
                    };
                    view.BindingContext = (BindingContext as TaskListViewModel).TodoEditViewModel;
                    ContentPage cp = new ContentPage() { Content = view };
                    NavigationPage.SetHasBackButton(cp, false);
                    NavigationPage.SetHasNavigationBar(cp, false);
                    await Navigation.PushAsync(cp);
                    return true;
				};
			}
        }
    }
}
