using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace NTag.Behaviors
{
    public class TracksMenuBehavior : Behavior<ListBox>
    {
        public static DependencyProperty MenuOpeningProperty =
            DependencyProperty.Register(nameof(MenuOpening), typeof(ICommand), typeof(TracksMenuBehavior), null);

        public ICommand MenuOpening
        {
            get { return (ICommand)GetValue(MenuOpeningProperty); }
            set { SetValue(MenuOpeningProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ContextMenuOpening += OnMenuOpening;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.ContextMenuOpening -= OnMenuOpening;
            base.OnDetaching();
        }

        private void OnMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (MenuOpening != null && MenuOpening.CanExecute(null))
            {
                MenuOpening.Execute(null);
            }
        }
    }
}
