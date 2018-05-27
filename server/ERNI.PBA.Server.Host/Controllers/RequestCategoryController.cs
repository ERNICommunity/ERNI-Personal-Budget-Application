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

        [HttpPost]
        public async Task<IActionResult> AddCategory(PostCategoryModel payload, CancellationToken cancellationToken)
        {
            var requestCategory = new RequestCategory
            {
                Title = payload.Title
            };

            _requestCategoryRepository.AddRequestCategory(requestCategory);

            await _unitOfWork.SaveChanges(cancellationToken);

            return Ok();
        }
    }
}