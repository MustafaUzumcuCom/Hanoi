using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BabelIm.Infrastructure
{
    public static class UIExtensions
    {
        /// <summary>
        /// Gets the contents of a richtext box as a string
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <returns></returns>
        /// <remarks>http://blogs.msdn.com/jfoscoding/archive/2006/01/14/512825.aspx</remarks>
        public static string GetText(this RichTextBox richTextBox)
        {
            // use a TextRange to fish out the Text from the Document
            TextRange textRange = new TextRange(
                richTextBox.Document.ContentStart,
                richTextBox.Document.ContentEnd);

            return textRange.Text;
        }

        /// <summary>
        /// Resizes the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static System.Drawing.Image Resize(this System.Drawing.Image image, Size size)
        {
            Size newSize = new Size(size.Width, size.Height);

            if (image.Size.Width > image.Size.Height)
            {
                newSize.Height = (image.Size.Height * size.Width) / image.Size.Width;
            }
            else
            {
                newSize.Width = (image.Size.Width * size.Height) / image.Size.Height;
            }

            Rectangle               rectangle   = new Rectangle(0, 0, newSize.Width, newSize.Height);
            System.Drawing.Image    resized     = new Bitmap(newSize.Width, newSize.Height, image.PixelFormat);

            using (Graphics graphic = Graphics.FromImage(resized))
            {
                graphic.CompositingQuality  = CompositingQuality.HighQuality;
                graphic.SmoothingMode       = SmoothingMode.HighQuality;
                graphic.InterpolationMode   = InterpolationMode.HighQualityBicubic;

                graphic.DrawImage((System.Drawing.Image)image.Clone(), rectangle);
            }

            return resized;
        }
    }
}
