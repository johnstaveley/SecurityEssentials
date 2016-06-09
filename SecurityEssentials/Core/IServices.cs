using System.Collections.Generic;

namespace SecurityEssentials.Core
{
    public interface IServices
    {
        bool SendEmail(string from, ICollection<string> toAddresses, ICollection<string> cc, ICollection<string> bcc, string subject, string body, bool htmlEmail);
    }
}