﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Models
{

    public class CreateCountryDTO
    {
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Country Name is too long")]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 2, ErrorMessage = "Short Country Name is too long")]
        public string ShortName { get; set; }
    }

    public class CountryDTO : CreateCountryDTO
    {
        public int CountryId { get; set; }
        public IList<HotelDTO> Hotels { get; set; }
    }

    public class UpdateCountryDTO : CreateCountryDTO 
    {
        public IList<CreateHotelDTO> Hotels { get; set; }
    }
}
