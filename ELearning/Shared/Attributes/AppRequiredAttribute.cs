using Share.Consts;
using System.ComponentModel.DataAnnotations;

namespace Shared.Attributes
{
    public class AppRequiredAttribute : RequiredAttribute
    {
        public AppRequiredAttribute() : base()
        {
            this.ErrorMessage = AttributeErrMesg.REQUIRED;
        }
    }
}
