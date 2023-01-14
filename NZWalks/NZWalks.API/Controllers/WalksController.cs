using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;
        private readonly IRegionRepository regionRepository;
        private readonly IWalkDifficultyRepository walkDifficultyRepository;

        public WalksController(IWalkRepository walkRepository, IMapper mapper, IRegionRepository regionRepository, IWalkDifficultyRepository walkDifficultyRepository)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
            this.regionRepository = regionRepository;
            this.walkDifficultyRepository = walkDifficultyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalksAsync()
        {
            var walks = await walkRepository.GetAllAsync();
            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walks);

            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            var walkDomain = await walkRepository.GetAsync(id);

            if (walkDomain == null)
                return NotFound();

            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);
            return Ok(walkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync([FromBody] Models.DTO.AddWalkRequest addWalk)
        {
            // Validate the incoming request
            if (!(await ValidateAddWalkAsync(addWalk)))
            {
                return BadRequest(ModelState);
            }

            // Convert DTO  to Domain object
            var walkDomain = mapper.Map<Models.Domain.Walk>(addWalk);

            // Pass domain object to Repository to persist this
            walkDomain = await walkRepository.AddAsync(walkDomain);

            // Convert the Domain object back to DTO
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            // Send DTO response back to client
            return CreatedAtAction(nameof(GetWalkAsync), new { id = walkDomain.Id }, walkDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id, [FromBody] Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            // Validate the incoming request
            if (!(await ValidateUpdateWalkAsync(updateWalkRequest)))
            {
                return BadRequest(ModelState);
            }
        
            var walkDomain = mapper.Map<Models.Domain.Walk>(updateWalkRequest);
            walkDomain = await walkRepository.UpdateAsync(id, walkDomain);

            if (walkDomain == null)
            {
                return NotFound("Walk with this id was not found");
            }

            var walkDto = mapper.Map<Models.DTO.Walk>(walkDomain);

            return Ok(walkDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync([FromRoute] Guid id)
        {
            var walkDomain = await walkRepository.DeleteAsync(id);

            if (walkDomain == null)
            {
                return NotFound("Walk with this id was not found");
            }
            var walkDto = mapper.Map<Models.DTO.Walk>(walkDomain);

            return Ok(walkDto);
        }

        #region Private methods

        private async Task<bool> ValidateAddWalkAsync(Models.DTO.AddWalkRequest addWalk)
        {
            if (addWalk == null)
            {
                ModelState.AddModelError(nameof(addWalk), "Add Region data is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(addWalk.Name))
            {
                ModelState.AddModelError(nameof(addWalk.Name), $"{nameof(addWalk.Name)} is required.");
            }
            if (addWalk.Length <= 0)
            {
                ModelState.AddModelError(nameof(addWalk.Length), $"{nameof(addWalk.Length)} should be greater than zero.");
            }
            var region = await regionRepository.GetAsync(addWalk.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(addWalk.RegionId), $"{nameof(addWalk.RegionId)} is invalid.");
            }

            var difficultyId = await walkDifficultyRepository.GetAsync(addWalk.WalkDifficultyId);
            if (difficultyId == null)
            {
                ModelState.AddModelError(nameof(addWalk.WalkDifficultyId), $"{nameof(addWalk.WalkDifficultyId)} is invalid.");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }
        
        private async Task<bool> ValidateUpdateWalkAsync(UpdateWalkRequest updateWalkRequest)
        {
            if (updateWalkRequest == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest), "Add Region data is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(updateWalkRequest.Name))
            {
                ModelState.AddModelError(nameof(updateWalkRequest.Name), $"{nameof(updateWalkRequest.Name)} is required.");
            }
            if (updateWalkRequest.Length <= 0)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.Length), $"{nameof(updateWalkRequest.Length)} should be greater than zero.");
            }
            var region = await regionRepository.GetAsync(updateWalkRequest.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.RegionId), $"{nameof(updateWalkRequest.RegionId)} is invalid.");
            }

            var difficultyId = await walkDifficultyRepository.GetAsync(updateWalkRequest.WalkDifficultyId);
            if (difficultyId == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.WalkDifficultyId), $"{nameof(updateWalkRequest.WalkDifficultyId)} is invalid.");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }


        #endregion


    }
}
