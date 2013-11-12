using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RefreshableListBoxSample
{
    internal static class RefreshableListBoxMethods
    {
        internal static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int counter = 0; counter < childCount; counter++)
            {
                yield return VisualTreeHelper.GetChild(parent, counter);
            }
        }

        internal static T GetFirstLogicalChildByType<T>(this FrameworkElement parent, bool applyTemplates)
            where T : FrameworkElement
        {

            Queue<FrameworkElement> queue = new Queue<FrameworkElement>();
            queue.Enqueue(parent);

            while (queue.Count > 0)
            {
                FrameworkElement element = queue.Dequeue();
                var elementAsControl = element as Control;
                if (applyTemplates && elementAsControl != null)
                {
                    elementAsControl.ApplyTemplate();
                }

                if (element is T && element != parent)
                {
                    return (T)element;
                }

                foreach (FrameworkElement visualChild in element.GetVisualChildren().OfType<FrameworkElement>())
                {
                    queue.Enqueue(visualChild);
                }
            }

            return null;
        }
    }
}
