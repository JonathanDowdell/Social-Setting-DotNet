using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Setting.Setting.Model;
using Social_Setting.Setting.Service;
using Social_Setting.User.Service;
using Social_Setting.Utils.Model;

namespace Social_Setting.Setting.Controller;

[ApiController]
[Route("setting"), Authorize]
public class SettingController : Microsoft.AspNetCore.Mvc.Controller
{

    private readonly ISettingService _settingService;

    private readonly IUserService _userService;

    public SettingController(ISettingService settingService, IUserService userService)
    {
        _settingService = settingService;
        _userService = userService;
    }

    

    /// <summary> The CreatePost function creates a new setting.</summary>
    ///
    /// <param name="createSettingRequest"> Object containing the Title and Description of the setting.</param>
    ///
    /// <returns> A 201 created response with the location of the new setting.</returns>
    [HttpPost, Authorize]
    public async Task<ActionResult<SettingResponse>> CreatePost(CreateSettingRequest createSettingRequest)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        var settingEntity = await _settingService.CreateSettingAsync(currentUser, createSettingRequest);
        return Created("/setting", new SettingResponse(settingEntity));
    }

    /// <summary> The GetSettingByName function returns a setting by name.</summary>
    ///
    /// <param name="name"> The name of the setting to be retrieved.</param>
    ///
    /// <returns> A settingresponse object.</returns>
    [HttpGet("{name}"), AllowAnonymous]
    public async Task<ActionResult<SettingResponse>> GetSettingByName(string name)
    {
        var settingEntity = await _settingService.GetSettingByNameAsync(name);
        return Ok(new SettingResponse(settingEntity));
    }
    

    /// <summary> The GetSettingsByName function returns a list of settings that match the name provided.</summary>
    ///
    /// <param name="name"> The name of the setting to be retrieved</param>
    ///
    /// <returns> All the settings that match the name passed in.</returns>
    [HttpGet("search/{name}"), AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SettingResponse>>> GetSettingsByName(string name)
    {
        var settingEntities = await _settingService.GetSettingsByNameAsync(name);
        return Ok(settingEntities.Select(setting => new SettingResponse(setting)));
    }

    /// <summary> The GetAllSubSettings function returns all sub settings from the database.</summary>
    ///
    /// <param name="skip"> The name of the setting to be retrieved</param>
    /// <param name="take"> The name of the setting to be retrieved</param>
    /// <returns> All the settings.</returns>
    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SettingResponse>>> GetAllSubSettings([FromQuery(Name = "Skip")] int skip, [FromQuery(Name = "Take")] int take)
    {
        var pagination = new Pagination
        {
            Skip = skip,
            Take = take
        };
        var allSubSettingEntities = await _settingService.GetAllSubSettings(pagination);
        return Ok(allSubSettingEntities.Select(setting => new SettingResponse(setting)));
    }
    
}