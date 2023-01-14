using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalkDificultiesController : Controller
    {
        private readonly IWalkDifficultyRepository walkDifficultyRepository;
        private readonly IMapper mapper;

        public WalkDificultiesController(IWalkDifficultyRepository walkDifficultyRepository, IMapper mapper)
        {
            this.walkDifficultyRepository = walkDifficultyRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalkDifficultiesAsync()
        {
            var difficultiesDomain = await walkDifficultyRepository.GetAllAsync();

            var difficultiesDTO = mapper.Map<List<Models.DTO.WalkDifficulty>>(difficultiesDomain);

            return Ok(difficultiesDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkDifficultyByIdAsync")]
        public async Task<IActionResult> GetWalkDifficultyByIdAsync([FromRoute] Guid id)
        {
            var walkDifficultyDomain = await walkDifficultyRepository.GetAsync(id);

            if (walkDifficultyDomain == null)
            {
                return NotFound();
            }

            var difficultyDTO = mapper.Map<Models.DTO.WalkDifficulty>(walkDifficultyDomain);

            return Ok(difficultyDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkDifficultyAsync(Models.DTO.AddWalkDifficultyRequest addWalkDifficultyRequest)
        {
            // Validate incoming request
            //if (!(await ValidateAddWalkDifficultyAsync(addWalkDifficultyRequest)))
            //{
            //    return BadRequest(ModelState);
            //}

            var walkDifficultyDomain = mapper.Map<Models.Domain.WalkDifficulty>(addWalkDifficultyRequest);

            walkDifficultyDomain = await walkDifficultyRepository.AddAsync(walkDifficultyDomain);

            var walkDifficultyDTO = mapper.Map<Models.DTO.WalkDifficulty>(walkDifficultyDomain);

            return CreatedAtAction(nameof(GetWalkDifficultyByIdAsync), new { id = walkDifficultyDomain.Id }, walkDifficultyDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkDifficultyAsync([FromRoute] Guid id, [FromBody] Models.DTO.UpdateWalkDifficultyRequest updateWalkDifficultyRequest)
        {
            // Validate incoming request
            //if (ValidateUpdateWalkDifficultyAsync(updateWalkDifficultyRequest))
            //{
            //    return BadRequest(ModelState);
            //}

            var walkDifficultyDomain = mapper.Map<Models.Domain.WalkDifficulty>(updateWalkDifficultyRequest);
            walkDifficultyDomain = await walkDifficultyRepository.UpdateAsync(id, walkDifficultyDomain);

            if (walkDifficultyDomain == null)
            {
                return NotFound();
            }

            var walkDifficultyDTO = mapper.Map<Models.DTO.WalkDifficulty>(walkDifficultyDomain);

            return Ok(walkDifficultyDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteDifficultyAsync([FromRoute] Guid id)
        {
            var walkDifficultyDomain = await walkDifficultyRepository.DeleteAsync(id);

            if (walkDifficultyDomain == null)
            {
                return NotFound();
            }

            var walkDifficultyDTO = mapper.Map<Models.DTO.WalkDifficulty>(walkDifficultyDomain);

            return Ok(walkDifficultyDTO);
        }

        #region Private methods 
        private async Task<bool> ValidateAddWalkDifficultyAsync(Models.DTO.AddWalkDifficultyRequest addWalkDifficultyRequest)
        {
            if (addWalkDifficultyRequest == null)
            {
                ModelState.AddModelError(nameof(addWalkDifficultyRequest), $"{nameof(addWalkDifficultyRequest)} data is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(addWalkDifficultyRequest.Code))
            {
                ModelState.AddModelError(nameof(addWalkDifficultyRequest.Code), $"{nameof(addWalkDifficultyRequest.Code)} is required.");
            }

            var difficulty = await walkDifficultyRepository.GetByCodeAsync(addWalkDifficultyRequest?.Code);
            
            if (difficulty != null)
            {
                ModelState.AddModelError(nameof(addWalkDifficultyRequest.Code), $"{nameof(addWalkDifficultyRequest.Code)} already exists.");

            }
            if (ModelState.Count > 0)
            {
                return false;
            }

            return true;
        }

        private bool ValidateUpdateWalkDifficultyAsync(UpdateWalkDifficultyRequest updateWalkDifficultyRequest)
        {
            if (updateWalkDifficultyRequest == null)
            {
                ModelState.AddModelError(nameof(updateWalkDifficultyRequest), "Add Walk Difficulty data is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(updateWalkDifficultyRequest.Code))
            {
                ModelState.AddModelError(nameof(updateWalkDifficultyRequest.Code), $"{nameof(updateWalkDifficultyRequest.Code)} is required.");
            }

            if (ModelState.Count > 0)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
