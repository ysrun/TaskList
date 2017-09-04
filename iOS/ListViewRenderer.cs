using System;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using TaskList.iOS;
using TaskList.Controls;
using MyApp.iOS.Renderers;


[assembly: ExportRenderer(typeof(UnselectableListView), typeof(UnselectableListViewRenderer))]
namespace MyApp.iOS.Renderers
{
    public class UnselectableListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				// Unsubscribe from event handlers and cleanup any resources
			}

			if (e.NewElement != null)
			{
				// Configure the native control and subscribe to event handlers
				Control.AllowsSelection = false;
			}
        }
    }
}

