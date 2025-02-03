using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desnz.Chmm.Identity.Common.Commands
{
    public class TestFileUploadCommand : IRequest<ActionResult<List<string>>>
    {
        public List<IFormFile>? Files { get; set; }
    }
}
