namespace Orc.Skia
{
    using System;
    using System.Windows.Media.Animation;
    using Catel.MVVM.Converters;

    [System.Windows.Data.ValueConversion(typeof(bool), typeof(RepeatBehavior))]
    public class BooleanToRepeatBehaviorConverter : ValueConverterBase<bool, RepeatBehavior>
    {
        private readonly RepeatBehavior _oneTimeRepeat = new RepeatBehavior(1);

        protected override object Convert(bool value, Type targetType, object parameter)
        {
            return value ? RepeatBehavior.Forever : _oneTimeRepeat;
        }
    }
}
