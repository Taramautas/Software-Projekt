using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Uebungsprojekt.Models
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1 * 1024 * 1024)]
        public IEnumerable<IFormFile> file { get; set; }
        // [AllowedExtensions(new string[] { ".json"})]

    }
}
