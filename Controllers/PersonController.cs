using Microsoft.AspNetCore.Mvc;
using ServiceContract;
using ServiceContract.DTO;

namespace CRUDExample.Controllers
{
    public class PersonController : Controller
    {
        //private fields
        private readonly IPersonsService _personService;

        //constructor
        //is it the only way to inject service through constructor?
        //i remember in spring there are 3 ways so im just curious if it is the same
        public PersonController(IPersonsService personsService) {
            _personService = personsService;
        }

        [Route("persons/index")]
        [Route("/")]
        public IActionResult Index(string searchBy, string? searchString)
        {
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name"},
                { nameof(PersonResponse.Email), "Email"},
                { nameof(PersonResponse.DateOfBirth), "Date Of Birth"},
                { nameof(PersonResponse.Gender), "Gender"},
                { nameof(PersonResponse.CountryID), "Country"},
                { nameof(PersonResponse.Address), "Address"}
            };
            //List<PersonResponse> persons = _personService.GetAllPersons();
            List<PersonResponse> persons = 
                _personService.GetFilteredPersons(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            return View(persons);
        }
    }
}
