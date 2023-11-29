using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;

namespace CRUDExample.Controllers
{
    //[Route("persons")]
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;

        //constructor
        //is it the only way to inject service through constructor?
        //i remember in spring there are 3 ways so im just curious if it is the same
        public PersonsController(IPersonsService personsService,
            ICountriesService countriesService) {
            _personService = personsService;
            _countriesService = countriesService;
        }

        //[Route("persons/index")]
        //[Route("index")]
        [Route("[action]")]
        [Route("/")]
        public IActionResult Index(string searchBy, string? searchString, string sortBy = 
            nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = 
            SortOrderOptions.ASC)
        {
            //Search
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

            //Sort
            List<PersonResponse> sortedPersons = _personService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(sortedPersons);
        }
        //[Route("persons/create")]
        //[Route("create")]
        [Route("[action]")]
        [HttpGet]
        public IActionResult Create() {

            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp => 
            new SelectListItem() { Text=temp.CountryName, 
                Value=temp.CountryID.ToString()});
            return View();
        }
        [HttpPost]
        //[Route("create")]
        [Route("[action]")]
        public IActionResult Create(PersonAddRequest personAddRequest) {
            if (!ModelState.IsValid) {
                List<CountryResponse> countries = _countriesService.GetAllCountries();

                ViewBag.Countries = countries.Select(temp =>
                new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryID.ToString()
                });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();

                return View();
            }
            PersonResponse personResponse = _personService.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public IActionResult Edit(Guid personID)
        {
            PersonResponse? personResponse = 
                _personService.GetPersonByPersonID(personID);

            if (personResponse == null) {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = 
                personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            });


            return View(personUpdateRequest);
        }
    }
}
