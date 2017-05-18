using System.Collections.Generic;
using System.Linq;

namespace SecurityEssentials.Model
{
    public class SEIdentityResult
    {
        public SEIdentityResult(IEnumerable<string> errors)
        {
            var err = errors as IList<string> ?? errors.ToList();
            if (err.Any())
            {
                Errors = err;
                Succeeded = false;
            }
            else
            {
                Errors = new List<string>();
                Succeeded = true;
            }
        }

        //
        // Summary:
        //     Failure constructor that takes error messages
        public SEIdentityResult(params string[] errors) : this(errors.ToList())
        {
        }

        // Summary:
        //     List of errors
        public IEnumerable<string> Errors { get; }

        public bool Succeeded { get; }
    }
}