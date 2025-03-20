using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Chess_game.Extensions
{
    public class EnumBindingSource_Extension : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumBindingSource_Extension(Type enumType)
        {
            if(enumType is null || !enumType.IsEnum)
            {
                throw new ArgumentException("Enum Type should be type of enum and not be null");
            }

            EnumType= enumType;
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
