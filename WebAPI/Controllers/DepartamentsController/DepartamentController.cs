using AutoMapper;
using Contracts.AllRepository.DepartamentsRepository;
using Entities.DTO.DepartamentDTOS;
using Entities.Model.AnyClasses;
using Entities.Model.DepartamentsModel;
using Entities.Model.FileModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TSTUWebAPI.Controllers.FileControllers;

namespace TSTUWebAPI.Controllers.DepartamentsController
{
    [Route("api/departament")]
    [ApiController]
    public class DepartamentController : ControllerBase
    {
        private readonly IDepartamentRepository _repository;
        private readonly IMapper _mapper;
        public DepartamentController(IDepartamentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        // Departament CRUD

        [Authorize(Roles = "Admin")]
        [HttpPost("createdepartament")]
        public IActionResult CreateDepartament(DepartamentCreatedDTO departament1)
        {
            var departament = _mapper.Map<Departament>(departament1);
            departament.status_id = 1;
            departament.crated_at = DateTime.UtcNow;

            FileUploadRepository fileUpload = new FileUploadRepository();

            var Url = fileUpload.SaveFileAsync(departament1.img_up);
            if (Url == "File not found or empty!" || Url == "Invalid file extension!" || Url == "Error!")
            {
                return BadRequest("File created error!");
            }
            if (Url != null && Url.Length > 0)
            {
                departament.img_ = new Files
                {
                    title = Guid.NewGuid().ToString(),
                    url = Url
                };
            }


            int check = _repository.CreateDepartament(departament);

            if (check == 0)
            {
                fileUpload.DeleteFileAsync(Url);
                return StatusCode(400);
            }
            CreatedItemId createdItemId = new CreatedItemId()
            {
                id = check,
                StatusCode = 200
            };

            return Ok(createdItemId);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getalldepartament")]
        public IActionResult GetAllDepartament(int queryNum, int pageNum)
        {
            queryNum = Math.Abs(queryNum);
            pageNum = Math.Abs(pageNum);
            IEnumerable<Departament> departaments1 = _repository.AllDepartament(queryNum, pageNum);
            var departaments = _mapper.Map<IEnumerable<DepartamentReadedDTO>>(departaments1);
            if (departaments == null) { }
            return Ok(departaments);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getbyiddepartament/{id}")]
        public IActionResult GetByIdDepartament(int id)
        {

            Departament departament1 = _repository.GetDepartamentById(id);
            if (departament1 == null)
            {

            }
            var departament = _mapper.Map<DepartamentReadedDTO>(departament1);
            if (departament == null) { }

            return Ok(departament);
        }

        [HttpGet("sitegetalldepartament")]
        public IActionResult GetAllDepartamentsite(int queryNum, int pageNum)
        {
            queryNum = Math.Abs(queryNum);
            pageNum = Math.Abs(pageNum);
            IEnumerable<Departament> departaments1 = _repository.AllDepartamentSite(queryNum, pageNum);
            var departaments = _mapper.Map<IEnumerable<DepartamentReadedSiteDTO>>(departaments1);
            if (departaments == null) { }
            return Ok(departaments);
        }

        [HttpGet("sitegetalldepartamentchild/{parent_id}")]
        public IActionResult GetAllDepartamentChild(int parent_id)
        {

            IEnumerable<Departament> departaments1 = _repository.AllDepartamentChild(parent_id);
            var departaments = _mapper.Map<IEnumerable<DepartamentChildReadedSiteDTO>>(departaments1);
            if (departaments == null) { }
            return Ok(departaments);
        }


        [HttpGet("sitegetbyiddepartament/{id}")]
        public IActionResult GetByIdDepartamentsite(int id)
        {

            Departament departament1 = _repository.GetDepartamentByIdSite(id);
            if (departament1 == null)
            {

            }
            var departament = _mapper.Map<DepartamentReadedSiteDTO>(departament1);
            if (departament == null) { }

            return Ok(departament);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("deletedepartament/{id}")]
        public IActionResult DeleteDepartament(int id)
        {
            bool check = _repository.DeleteDepartament(id);
            if (!check)
            {
                return StatusCode(400);
            }
            bool check1 = _repository.SaveChanges();
            if (!check1)
            {
                return StatusCode(400);
            }
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("updatedepartament/{id}")]
        public IActionResult UpdateDepartament(DepartamentUpdatedDTO departament1, int id)
        {

            try
            {
                if (departament1 == null)
                {
                    return BadRequest();
                }

                var dbupdated = _mapper.Map<Departament>(departament1);
                dbupdated.updated_at = DateTime.UtcNow;

                FileUploadRepository fileUpload = new FileUploadRepository();

                var Url = fileUpload.SaveFileAsync(departament1.img_up);
                if (Url == "File not found or empty!" || Url == "Invalid file extension!" || Url == "Error!")
                {
                    return BadRequest("File created error!");
                }
                if (Url != null && Url.Length > 0)
                {
                    dbupdated.img_ = new Files
                    {
                        title = Guid.NewGuid().ToString(),
                        url = Url
                    };
                }

                bool updatedcheck = _repository.UpdateDepartament(id, dbupdated);
                if (!updatedcheck)
                {
                    return BadRequest();
                }
                bool check = _repository.SaveChanges();
                if (!check)
                {
                    fileUpload.DeleteFileAsync(Url);
                    return BadRequest();
                }
                return Ok("Updated");
            }
            catch
            {
                return BadRequest();
            }
        }


    }
}

