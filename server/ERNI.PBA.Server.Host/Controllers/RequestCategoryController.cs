using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Model;
using Microsoft.AspNetCore.Mvc;

namespace ERNI.PBA.Server.Host.Controllers
{
    [Route("api/[controller]")]
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
            var result = await _requestCategoryRepository.GetRequestCategories(cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id, CancellationToken cancellationToken)
        {
            var requestCategory = await _requestCategoryRepository.GetRequestCategory(id, cancellationToken);

            return Ok(requestCategory);
        }

        [HttpPost]
        public async Task<RequestCategory> AddCategory([FromBody] PostCategoryModel payload, CancellationToken cancellationToken)
        {
            var requestCategory = new RequestCategory
            {
                Title = payload.Title
            }; 

            _requestCategoryRepository.AddRequestCategory(requestCategory);

            await _unitOfWork.SaveChanges(cancellationToken);

            return requestCategory;
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

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }

        [HttpDelete]
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