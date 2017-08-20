using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskList
{
    [ContentProperty("Source")]
    public class ImageResource:IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Source)) return null;
            return ImageSource.FromResource(Source);
        }
    }
}
