using System;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace TaskList.AttachedProperty
{
    public class ButtonAttached
    {
        public static readonly BindableProperty OnClickActionProperty =
        BindableProperty.CreateAttached("OnClickAction", typeof(Action<object>), typeof(ButtonAttached), null, propertyChanging: OnClickActionChanged);

        private static void OnClickActionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var button = bindable as Button;
            if (newValue is Action<object>)
            {
                button.Clicked += (sender, e) => ((Action<object>)newValue)(sender is BindableObject ? (sender as BindableObject).BindingContext : null);
            }
        }

    }
    public class ContentPageAttached
    {
		public static readonly BindableProperty OnApperingActionProperty =
            BindableProperty.CreateAttached("OnApperingAction", typeof(Action), typeof(ContentPageAttached), null, propertyChanging: OnApperingActionChanged);

		private static void OnApperingActionChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var page = bindable as ContentPage;
			if (newValue is Action)
			{
                page.Appearing += (sender, e) => ((Action)newValue)();
			}
		}
    }

    public class MenuItemAttached
    {
		public static readonly BindableProperty OnClickActionProperty =
            BindableProperty.CreateAttached("OnClickAction", typeof(Action<object>), typeof(MenuItemAttached), null, propertyChanging: OnClickActionChanged);

		private static void OnClickActionChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var menu = bindable as MenuItem;
			if (newValue is Action<object>)
			{
				menu.Clicked += (sender, e) => ((Action<object>)newValue)(sender is BindableObject ? (sender as BindableObject).BindingContext : null);
			}
		}
    }
    public class ListViewAttached
    {
		public static readonly BindableProperty OnRefreshActionProperty =
	        BindableProperty.CreateAttached("OnRefreshAction", typeof(Func<Task>), typeof(ListViewAttached), null, propertyChanging: OnActionChanged);

		private static void OnActionChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var listview = bindable as ListView;
   //         listview.ItemTapped += (sender, e) => 
   //         {
   //             if (e == null) return; // has been set to null, do not 'process' tapped event
   //             ((ListView)sender).SelectedItem = null; // de-select the row   
			//};
			if (newValue is Func<Task>)
			{
                listview.IsPullToRefreshEnabled = true;

                listview.RefreshCommand = new Command( async (obj) => 
                {
                    await (newValue as Func<Task>)();
                    listview.EndRefresh();
                });
				//listview.ItemSelected += (sender, e) =>
				//{
				//	if (listview.SelectedItem != null || e.SelectedItem != null)
				//	{
				//		((ListView)sender).SelectedItem = null;
				//	}
				//};
			}
		}
		public static readonly BindableProperty IsRefreshProperty =
	BindableProperty.CreateAttached("IsRefresh", typeof(bool), typeof(ListViewAttached), false, propertyChanging: IsRefreshChanged);
		
        private static void IsRefreshChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var listview = bindable as ListView;
            if (newValue is bool && (bool)newValue)
			{
                listview.BeginRefresh();
                //listview.SetValue(IsRefreshProperty, false);
                //Device.BeginInvokeOnMainThread(() => listview.SetValue(IsRefreshProperty, false));
			}
		}

	

	
	}
}