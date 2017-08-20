using System;
using Xamarin.Forms;

namespace TaskList.Controls
{
    public class SelectedButton:Button
    {
        public SelectedButton()
        {
        }

        internal bool isSetDefaultColor = false;
        internal Color defaultColor;

		public static readonly BindableProperty IsSelectedProperty =
	BindableProperty.Create(
        nameof(IsSelected),          // 対象のプロパティ名 (文字列)
        typeof(bool),         // 対象のプロパティの型
        typeof(SelectedButton),  // プロパティを定義する型（自クラス）
        false,                    // プロパティのデフォルト値
		propertyChanged: (bindable, oldValue, newValue) =>
		{ // 変更通知ハンドラ
            ((SelectedButton)bindable).IsSelected = (bool)newValue;
            if((bool)newValue)
            {
                ((SelectedButton)bindable).isSetDefaultColor = true;
                ((SelectedButton)bindable).defaultColor = ((SelectedButton)bindable).BackgroundColor;
                ((SelectedButton)bindable).BackgroundColor = ((SelectedButton)bindable).IsSelectedColor;
            }
            else if(((SelectedButton)bindable).isSetDefaultColor)
            {
                ((SelectedButton)bindable).BackgroundColor = ((SelectedButton)bindable).defaultColor;
			}
		},
		defaultBindingMode: BindingMode.TwoWay  // デフォルトのバインディングモード
	);

		public bool IsSelected
		{
            get { return (bool)GetValue(IsSelectedProperty); }
			set
			{
				SetValue(IsSelectedProperty, value);
			}
		}

		public static readonly BindableProperty IsSelectedColorProperty =
	BindableProperty.Create(
		nameof(IsSelectedColor),          // 対象のプロパティ名 (文字列)
		typeof(Color),         // 対象のプロパティの型
		typeof(SelectedButton),  // プロパティを定義する型（自クラス）
                Color.Transparent,                    // プロパティのデフォルト値
		propertyChanged: (bindable, oldValue, newValue) =>
		{ // 変更通知ハンドラ
            ((SelectedButton)bindable).IsSelectedColor = (Color)newValue;

		},
		defaultBindingMode: BindingMode.TwoWay  // デフォルトのバインディングモード
	);

		public Color IsSelectedColor
		{
			get { return (Color)GetValue(IsSelectedColorProperty); }
			set
			{
				SetValue(IsSelectedColorProperty, value);
			}
		}
    }
}
