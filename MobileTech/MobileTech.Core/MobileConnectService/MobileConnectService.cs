using System;
using System.Collections.Generic;
using System.Text;

namespace MobileTech.Core
{
    public partial class MobileConnService
    {
		public static bool IsCode(string input)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(input, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
            return !result;
        }
    }
      
}
