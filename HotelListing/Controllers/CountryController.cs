using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "120SecondsDuration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
        {
            
            var countries = await _unitOfWork.Countries.GetPagedList(requestParams);
            var results = _mapper.Map<IList<CountryDTO>>(countries);
            return Ok(results);
            //GlobalHandlerException added instead of Try Catch

        }


        [HttpGet("{id:int}", Name = "GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {
            var country = await _unitOfWork.Countries.Get(q => q.CountryId == id/*, new List<string> { "Hotels" }*/);
            var result = _mapper.Map<CountryDTO>(country);
            return Ok(result);
           
        }


        [Authorize(Roles = "Administrator")] //I commented it out to use the end point and text my consume api
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateCountry)} ");
                return BadRequest(ModelState);
            }

            var country = _mapper.Map<Country>(countryDTO);
            await _unitOfWork.Countries.Insert(country);
            await _unitOfWork.Save();

            return CreatedAtRoute("GetCountry", new { id = country.CountryId }, country);
           
        }


        //[Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCountry([FromBody] UpdateCountryDTO countryDTO, int id)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid UPDATE attempt in the name of {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }

            
            var country = await _unitOfWork.Countries.Get(q => q.CountryId == id);
            if (country == null)
            {
                _logger.LogError($"Invalid UPDATE attempt in the name of {nameof(UpdateCountry)}");
                return BadRequest("Submitted data is invalid");
            }
            _mapper.Map(countryDTO, country);
            _unitOfWork.Countries.Update(country);
            await _unitOfWork.Save();

            return NoContent();
            
        }


        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid DELETE attempt in the {nameof(DeleteCountry)}");
                return BadRequest();
            }

            var country = await _unitOfWork.Countries.Get(q => q.CountryId == id);

            if (country == null)
            {
                _logger.LogError($"Invalid DELETE attempt in the {nameof(DeleteCountry)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Countries.Delete(id);
            await _unitOfWork.Save();
            //GlobalHandlerException added instead of Try Catch

            return NoContent();
            
        }
    }
}
