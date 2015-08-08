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
                if (element is MapIcon && (iconOnTop == null || iconOnTop.ZIndex < element.ZIndex))
                {
                    iconOnTop = (MapIcon)element;
                }
            }
            return iconOnTop;
        }
    }
}
