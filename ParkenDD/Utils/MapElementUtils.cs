using System.Collections.Generic;
using Windows.UI.Xaml.Controls.Maps;

namespace ParkenDD.Utils
{
    public static class MapElementUtils
    {
        public static MapIcon GetTopmostIcon(this IList<MapElement> elements)
        {
            MapIcon iconOnTop = null;
            foreach (var element in elements)
            {
                var top = element as MapIcon;
                if (top != null && (iconOnTop == null || iconOnTop.ZIndex < top.ZIndex))
                {
                    iconOnTop = top;
                }
            }
            return iconOnTop;
        }
    }
}
