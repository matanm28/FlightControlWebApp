using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Converters {
    using System.ComponentModel;
    using System.Globalization;

    public class UtcDateTimeConverter : DateTimeConverter {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            return ((DateTime)base.ConvertFrom(context, culture, value)).ToUniversalTime();
        }
    }
}
