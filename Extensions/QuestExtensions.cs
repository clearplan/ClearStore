using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClearStore.Extensions
{
    public static class QuestExtensions
    {
        private const string segoeUI = Fonts.SegoeUI;

        public static IContainer ParagraphStyle(this IContainer container)
        {
            return container
                .DefaultTextStyle(t => t.FontFamily(segoeUI))
                .Padding(8);
        }

        public static IContainer HeadingStyle(this IContainer container) => 
            container.DefaultTextStyle(t => t.FontFamily(segoeUI).FontSize(12).SemiBold());

        public static IContainer HeadingStyle(this IContainer container, string font = segoeUI)
        {
            return container
                .DefaultTextStyle(t => t.FontFamily(font).FontSize(12).SemiBold());
        }

        public static TextStyle HyperlinkStyle => TextStyle
            .Default
            .FontFamily(segoeUI)
            .FontColor(Colors.Blue.Medium);
        

        public static IContainer NormalStyle(this IContainer container)
        {
            return container
                .DefaultTextStyle(t => 
                    t.FontFamily(segoeUI).FontColor(Colors.Black).FontSize(10))
                .AlignLeft();
        }
    }
}
