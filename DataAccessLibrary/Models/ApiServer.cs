using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
        using System.ComponentModel;
        using System.ComponentModel.DataAnnotations;

        public class ApiServer
        {
                public int Id { get; set; }
                [DisplayName("External API Server URL")]
                [Required(ErrorMessage = "A URL is required")]
                [MaxLength(1024)]
                public string URL { get; set; }
        }
}