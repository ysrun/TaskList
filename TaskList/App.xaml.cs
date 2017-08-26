using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TaskList
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Root = new RootPage();
            MainPage = Root;
        }
        public static Color MainBackGround = Color.FromHex("dce8ef");
        public static RootPage Root;

		protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }


    public class RootPage : MasterDetailPage
    {
		public RootPage()
		{
			var menuPage = new HumMenuPage();
            menuPage.Icon = "Menu.png";
			menuPage.Menu.ItemSelected += (sender, e) => NavigateTo(e.SelectedItem as MenuListItem);

			Master = menuPage;

			var detail = new NavigationPage(taskListPage);
            detail.BarBackgroundColor = Color.DeepSkyBlue; //Color.FromHex("3498DB");
			detail.BarTextColor = Color.White;
			Detail = detail;

		}
        public static TaskListPage taskListPage = new TaskListPage();
        public static MoneyPage moneyPage = new MoneyPage();

		/// <summary>
		/// ページ遷移のメソッドです。
		/// </summary>
		/// <param name="menu">MenuItem</param>
		void NavigateTo(MenuListItem menu)
		{
            NavigateTo(menu.TargetType);

		}
        public void NavigateTo(Type targetType)
        {
			Page displayPage;

			if (targetType == typeof(TaskListPage))
				displayPage = taskListPage;
			else if (targetType == typeof(MoneyPage))
				displayPage = moneyPage;
			else
				displayPage = (Page)Activator.CreateInstance(targetType);


			// 同じく各ページに移動する時にもバーの色を再設定 (このやり方では必須)
			var detail = new NavigationPage(displayPage);
			detail.BarBackgroundColor = targetType == typeof(MoneyPage) ? Color.Green : Color.DeepSkyBlue;
			detail.BarTextColor = Color.White;
			Detail = detail;

			IsPresented = false;
        }
    }
	public class HumMenuPage : ContentPage
	{
		public ListView Menu { get; set; }

		public HumMenuPage()
		{
			Title = "Menu"; // Icon を指定しても Title プロパティは必須項目です。
            BackgroundColor = Color.FromHex("dce8ef");

			// ListView 設定
			Menu = new MenuListView();

			var menuLabel = new ContentView
			{
				Padding = new Thickness(10, 36, 0, 5),
				Content = new Label
				{
					TextColor = Color.FromHex("333"),
					Text = "MENU",
					FontSize = 18,
				}
			};

			// タイトルの menuLabel と ListView を並べています。
			var layout = new StackLayout
			{
				Spacing = 0,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			layout.Children.Add(menuLabel);
			layout.Children.Add(Menu);

			Content = layout;
		}
	}
	public class MenuListView : ListView
	{
		public MenuListView()
		{
			List<MenuListItem> data = new MenuListData(); // インスタンス化して、
			ItemsSource = data; // ItemsSource として指定します。
			VerticalOptions = LayoutOptions.FillAndExpand;
			BackgroundColor = Color.Transparent;

			// ItemTemplate で使用しているのが ImageCell なので Android では Text が水色になってしまいます。
			// 嫌な場合は ImageCell ではなく ViewCell で ItemTemplate を作りましょう。
			var cell = new DataTemplate(typeof(ImageCell));
			cell.SetBinding(TextCell.TextProperty, "Title");
			cell.SetBinding(ImageCell.ImageSourceProperty, "IconSource");

			ItemTemplate = cell;
		}
	}
	public class MenuListItem
	{
		public string Title { get; set; }

		public string IconSource { get; set; }
		// この Type で移動先のページクラスを指定しています。
		public Type TargetType { get; set; }
	}


    public class MenuListData : List<MenuListItem>
    {
        public MenuListData()
        {
            this.Add(new MenuListItem()
            {
                Title = "TaskList",
                TargetType = typeof(TaskListPage)
            });

            this.Add(new MenuListItem()
            {
                Title = "Money",
                TargetType = typeof(MoneyPage)
            });

        }
    }
}
