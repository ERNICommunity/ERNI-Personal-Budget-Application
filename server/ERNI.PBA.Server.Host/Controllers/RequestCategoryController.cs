using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class RequestCategoryController : Controller
    {
        private readonly IRequestCategoryRepository _requestCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RequestCategoryController(IRequestCategoryRepository requestCategoryRepository, IUnitOfWork unitOfWork)
        {
            _requestCategoryRepository = requestCategoryRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var categories = await _requestCategoryRepository.GetRequestCategories(cancellationToken);

            var result = categories.Select(_ => new
            {
                Id = _.Id,
                Title = _.Title,
                IsActive = _.IsActive,
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id, CancellationToken cancellationToken)
        {
            var requestCategory = await _requestCategoryRepository.GetRequestCategory(id, cancellationToken);

            var result = new
            {
                Title = requestCategory.Title,
                IsActive = requestCategory.IsActive
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] PostCategoryModel payload, CancellationToken cancellationToken)
        {
            var requestCategory = new RequestCategory
            {
                Title = payload.Title,
                IsActive = true,
                IsUrlNeeded = false
            }; 

            _requestCategoryRepository.AddRequestCategory(requestCategory);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryModel payload,CancellationToken cancellationToken)
        {
            var requestCategory = await _requestCategoryRepository.GetRequestCategory(payload.Id, cancellationToken);

            if (requestCategory == null)
            {
                return BadRequest("Not a valid id");
            }

            requestCategory.Title = payload.Title;
            requestCategory.IsActive = payload.IsActive;
            requestCategory.IsUrlNeeded = payload.IsUrlNeeded;
            requestCategory.SpendLimit = payload.SpendLimit;

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            var requestCategory = await _requestCategoryRepository.GetRequestCategory(id, cancellationToken);

            if (requestCategory == null)
            {
                return BadRequest("Not a valid id");
            }

            _requestCategoryRepository.DeleteRequestCategory(requestCategory);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }
    }
}