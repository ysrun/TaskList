using System;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using TaskList.iOS;

[assembly: ExportRenderer(typeof(ViewCell), typeof(ViewCellItemSelectedCustomRenderer))]
namespace TaskList.iOS
{
	public class ViewCellItemSelectedCustomRenderer : ViewCellRenderer
	{
        private UIView bgView;
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);
            cell.BackgroundColor = UIColor.Purple;
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			if (bgView == null)
			{
				bgView = new UIView();
                //bgView.Layer.BackgroundColor = UIColor.Clear;
                bgView.BackgroundColor = UIColor.Green;
			}
            cell.SelectedBackgroundView = bgView;
			return cell;
		}
	}
}
