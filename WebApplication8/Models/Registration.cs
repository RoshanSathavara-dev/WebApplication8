using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication8.Models
{
    public class Registration
    {
        public int RegId { get; set; }

        
        public string FullName { get; set; }

       
        public string BookingType { get; set; }

        [Required]
        public string Country { get; set; }
        [Required]
        public string State { get; set; }
        public string SelectedCountry { get; set; }

        public string SelectedState { get; set; }
        public List<Countries> Countries { get; set; }
        public List<States> States { get; set; }

        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BookingDate { get; set; } = DateTime.Now;



        public bool Status { get; set; }

       
        public string Description { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public string Pdf { get; set; }


        [DataType(DataType.Date)]

        [DisplayFormat(DataFormatString = "{0;MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;



        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0;yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedDate { get; set; }

        public string Operations { get; set; }

    }
    public class Countries
    {
        public int Cid { get; set; }
        public string CName { get; set; }
    }
    public class States
    {
        public int Sid { get; set; }
        public string Sname { get; set; }
        public int Counid { get; set; }
    }

 
}
