using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispetcher.Common.Events
{
    public delegate void AttachmentSavedEventHandler(DateTime? messageDate, string attachmentFilename);
}
