namespace Orc.Skia
{
    using System;
    using System.Windows.Media.Animation;
    using Catel.MVVM.Converters;

    [System.Windows.Data.ValueConversion(typeof(bool), typeof(RepeatBehavior))]
    public class BooleanToRepeatBehaviorConverter : ValueConverterBase<bool, RepeatBehavior>
    {
        protected override object Convert(bool value, Type targetType, object parameter)
        {
            return value ? RepeatBehavior.Forever : new RepeatBehavior(1);
        }
    }
}
