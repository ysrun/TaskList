using System;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using TaskList.iOS;
using TaskList.Controls;
using MyApp.iOS.Renderers;

//[assembly: ExportRenderer(typeof(ViewCell), typeof(ViewCellItemSelectedCustomRenderer))]
//namespace TaskList.iOS
//{
//	public class ViewCellItemSelectedCustomRenderer : ViewCellRenderer
//	{
//        private UIView bgView;
//		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
//		{
//			var cell = base.GetCell(item, reusableCell, tv);
//            cell.BackgroundColor = UIColor.Purple;
//            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
//			if (bgView == null)
//			{
//				bgView = new UIView();
//                //bgView.Layer.BackgroundColor = UIColor.Clear;
//                bgView.BackgroundColor = UIColor.Green;
//			}
//            cell.SelectedBackgroundView = bgView;
//			return cell;
//		}
//	}
//}

[assembly: ExportRenderer(typeof(NativeViewCell), typeof(NativeViewCellRenderer))]
namespace MyApp.iOS.Renderers
{
	public class NativeViewCellRenderer : ViewCellRenderer
	{
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);

			// removes default selection style (gray background color on tapped event)
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;

			return cell;
		}
	}
}

