﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core.Interfaces;
using DAL.Models.Surveys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TRAISI.Helpers;
using TRAISI.ViewModels;
using System.Text.RegularExpressions;
using FluentValidation.Results;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace TRAISI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	public class SurveyExecutionController : Controller
	{

		private readonly IUnitOfWork _unitOfWork;
		private readonly IAuthorizationService _authorizationService;
		private readonly IAccountManager _accountManager;
		private readonly ICodeGeneration _codeGeneration;

		public SurveyExecutionController(IUnitOfWork unitOfWork, IHostingEnvironment hostingEnvironment, IAuthorizationService authorizationService, IAccountManager accountManager, ICodeGeneration codeGenerationService)
		{
			this._unitOfWork = unitOfWork;
			this._authorizationService = authorizationService;
			this._accountManager = accountManager;
			this._codeGeneration = codeGenerationService;
		}

		/// <summary>
		/// Get all shortcodes for survey
		/// </summary>
		/// <param name="id"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		[HttpGet("{id}/{mode}")]
		[Produces(typeof(List<Shortcode>))]
		public async Task<IActionResult> GetSurveyShortcodes(int id, string mode)
		{
			return await this.GetSurveyShortcodes(id,mode,-1,-1);
		}

		/// <summary>
		/// Gets shortcodes for survey (with page passed in)
		/// </summary>
		/// <param name="id"></param>
		/// <param name="mode"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet("{id}/{mode}/{pageIndex}/{pageSize}")]
		[Produces(typeof(List<Shortcode>))]
		public async Task<IActionResult> GetSurveyShortcodes(int id, string mode, int pageIndex, int pageSize)
		{
			var survey = await this._unitOfWork.Surveys.GetAsync(id);
			if(survey.Owner == this.User.Identity.Name || await HasExecuteSurveyPermissions(id))
			{
				var shortcodes = await this._unitOfWork.Shortcodes.GetShortcodesForSurveyAsync(id, mode=="test", pageIndex, pageSize);
				return Ok(Mapper.Map<IEnumerable<ShortcodeViewModel>>(shortcodes));
			}
			else
			{
				return BadRequest("User does not have permissions to execute this survey.");
			}
		}

		[HttpGet("{id}/{mode}/count")]
		[Produces(typeof(int))]
		public async Task<IActionResult> GetNumberSurveyShortCodes(int id, string mode)
		{
			var survey = await this._unitOfWork.Surveys.GetAsync(id);
			if(survey.Owner == this.User.Identity.Name || await HasExecuteSurveyPermissions(id))
			{
				var numCodes = await this._unitOfWork.Shortcodes.GetCountOfShortcodesForSurveyAsync(id, mode=="test");
				return Ok(numCodes);
			}
			else
			{
				return BadRequest("User does not have permissions to execute this survey.");
			}
		}


		/// <summary>
		/// Get all group codes for survey
		/// </summary>
		/// <param name="id"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		[HttpGet("{id}/groupcode/{mode}")]
		[Produces(typeof(List<GroupCode>))]
		public async Task<IActionResult> GetSurveyGroupCodes(int id, string mode)
		{
			return await this.GetSurveyGroupCodes(id, mode, -1, -1);
		}

		[HttpGet("{id}/groupcode/{mode}/{pageIndex}/{pageSize}")]
		[Produces(typeof(List<GroupCode>))]
		public async Task<IActionResult> GetSurveyGroupCodes(int id, string mode, int pageIndex, int pageSize)
		{
			var survey = await this._unitOfWork.Surveys.GetAsync(id);
			if(survey.Owner == this.User.Identity.Name || await HasExecuteSurveyPermissions(id))
			{
				var groupcodes = await this._unitOfWork.GroupCodes.GetGroupCodesForSurveyAsync(id, mode=="test", pageIndex, pageSize);
				return Ok(Mapper.Map<IEnumerable<GroupCodeViewModel>>(groupcodes));
			}
			else
			{
				return BadRequest("User does not have permissions to execute this survey.");
			}
		}


		[HttpGet("{id}/groupcode/{mode}/count")]
		[Produces(typeof(int))]
		public async Task<IActionResult> GetNumberSurveyGroupCodes(int id, string mode)
		{
			var survey = await this._unitOfWork.Surveys.GetAsync(id);
			if(survey.Owner == this.User.Identity.Name || await HasExecuteSurveyPermissions(id))
			{
				var numCodes = await this._unitOfWork.GroupCodes.GetCountOfGroupCodesForSurveyAsync(id, mode=="test");
				return Ok(numCodes);
			}
			else
			{
				return BadRequest("User does not have permissions to execute this survey.");
			}
		}


        /// <summary>
        /// Generate short codes for survey
        /// </summary>
        /// <param name="codeParameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateShortcodes([FromBody] CodeGenerationViewModel codeParameters)
        {
            if (ModelState.IsValid)
            {
                Survey survey = await this._unitOfWork.Surveys.GetAsync(codeParameters.SurveyId);
                //Survey survey = codeParameters.NumberOfCodes > 1? await this._unitOfWork.Surveys.GetSurveyWithAllCodesAsync(codeParameters.SurveyId) : await this._unitOfWork.Surveys.GetAsync(codeParameters.SurveyId);
                CodeGeneration codeGenerationParameters = Mapper.Map<CodeGeneration>(codeParameters);
                if (survey.Owner == this.User.Identity.Name || await HasExecuteSurveyPermissions(codeParameters.SurveyId))
                {
                    if (codeGenerationParameters.NumberOfCodes > 1)
                    {
                        this._codeGeneration.GenerateShortCodesBatch(codeGenerationParameters, survey);
                    }
                    else
                    {
                        this._codeGeneration.GenerateShortCode(codeGenerationParameters, survey);
                    }
                    await this._unitOfWork.SaveChangesAsync();
                    return new OkResult();
                }
                return BadRequest("User does not have permissions to execute this survey.");
            } 
			return BadRequest(ModelState);
		}

        [HttpPost("uploadIndividual"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportIndividualCodes()
        {
            try
            {
                var file = Request.Form.Files[0];
                if (Request.Form.ContainsKey("parameters"))
                {
                    CodeGenerationViewModel codeParameters = JsonConvert.DeserializeObject<CodeGenerationViewModel>(Request.Form["parameters"]);
                    codeParameters.NumberOfCodes = 1;   //hack to prevent error
                    CodeGenerationViewModelValidator parameterValidator = new CodeGenerationViewModelValidator();
                    ValidationResult validParameters = parameterValidator.Validate(codeParameters);
                    if (validParameters.IsValid)
                    {
                        int numberofCodes = 0;
                        string line;
                        using (StreamReader fileStream = new StreamReader(file.OpenReadStream()))
                        {
                            //for now assume valid line, generate a code per line (change later)
                            while ((line = fileStream.ReadLine()) != null)
                            {           
                                numberofCodes++;
                            }
                        }
                        codeParameters.NumberOfCodes = numberofCodes;
                        return await this.CreateShortcodes(codeParameters);  
                    }
                    else
                    {
                        AddErrors(validParameters.Errors.Select(e => e.ErrorMessage));
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    return BadRequest("Missing Code Generation Parameters");
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest("Upload Failed: " + ex.Message);
            }
        }

		/// <summary>
		/// Generate group code for survey
		/// </summary>
		/// <param name="codeParameters"></param>
		/// <returns></returns>
		[HttpPost("groupcode")]
		public async Task<IActionResult> CreateGroupCode([FromBody] CodeGenerationViewModel codeParameters)
		{
			if (ModelState.IsValid)
				{	
					var survey = await this._unitOfWork.Surveys.GetAsync(codeParameters.SurveyId);
					
					if(survey.Owner == this.User.Identity.Name || await HasExecuteSurveyPermissions(codeParameters.SurveyId))
					{
                        CodeGeneration codeGenerationParameters = Mapper.Map<CodeGeneration>(codeParameters);
                        this._codeGeneration.GenerateGroupCode(codeGenerationParameters, survey);
						await this._unitOfWork.SaveChangesAsync();
						return new OkResult();
					}
					return BadRequest("User does not have permissions to execute this survey.");
			}
				return BadRequest(ModelState);
		}

        [HttpPost("uploadGroup"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportGroupCodes()
        {
            try
            {
                var file = Request.Form.Files[0];
                if (Request.Form.ContainsKey("parameters"))
                {
                    CodeGenerationViewModel codeParameters = JsonConvert.DeserializeObject<CodeGenerationViewModel>(Request.Form["parameters"]);
                    codeParameters.NumberOfCodes = 1;   //hack to prevent error
                    CodeGenerationViewModelValidator parameterValidator = new CodeGenerationViewModelValidator();
                    ValidationResult validParameters = parameterValidator.Validate(codeParameters);
                    if (validParameters.IsValid)
                    {
                        var survey = await this._unitOfWork.Surveys.GetAsync(codeParameters.SurveyId);
                        if (survey.Owner == this.User.Identity.Name || await HasExecuteSurveyPermissions(codeParameters.SurveyId))
                        {
                            CodeGeneration codeGenerationParameters = Mapper.Map<CodeGeneration>(codeParameters);
                            string line;
                            int lineIndex = 1;
                            List<string> errorList = new List<string>();
                            List<string> groupNames = new List<string>();
                            using (StreamReader fileStream = new StreamReader(file.OpenReadStream()))
                            {
                                //for now assume valid line, generate a code per line (change later)
                                while ((line = fileStream.ReadLine()) != null)
                                {
                                    var lineSplit = line.Split(',');
                                    if (lineSplit.Count() != 1)
                                    {
                                        errorList.Add($"Error at line {lineIndex}: {line} - Skipped generation");
                                    }
                                    else
                                    {
                                        groupNames.Add(lineSplit[0]);
                                    }
                                }
                            }
                            codeGenerationParameters.NumberOfCodes = groupNames.Count;
                            this._codeGeneration.GenerateGroupCodesBatch(codeGenerationParameters, groupNames, survey);
                            await this._unitOfWork.SaveChangesAsync();
                            if (errorList.Count == 0)
                            {
                                return new OkResult();
                            }
                            else
                            {
                                AddErrors(errorList);
                                return BadRequest(ModelState);
                            }
                        }
                        return BadRequest("User does not have permissions to execute this survey.");
                    }
                    else
                    {
                        foreach (var error in validParameters.Errors)
                        {
                            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    return BadRequest("Missing Code Generation Parameters");
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest("Upload Failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Check if user is group admin of given group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<bool> IsGroupAdmin(string groupName)
		{
			var isGroupAdmin = await this._unitOfWork.GroupMembers.IsGroupAdminAsync(this.User.Identity.Name, groupName);
			return isGroupAdmin;
		}

		/// <summary>
		/// Check if user is a member of given group
		/// </summary>
		/// <param name="groupId"></param>
		/// <returns></returns>
		private async Task<bool> MemberOfGroup(int groupId)
		{
			var groupMembers = await this._unitOfWork.UserGroups.GetGroupMembersInfoAsync(groupId);
			bool memberOfGroup = groupMembers.Where(m => m.Item1.UserName == this.User.Identity.Name).Any();
			return memberOfGroup;
		}

		/// <summary>
		/// Check if user has execute survey permissions
		/// </summary>
		/// <param name="surveyId"></param>
		/// <returns></returns>
		private async Task<bool> HasExecuteSurveyPermissions (int surveyId)
		{
			var surveyPermissions = await this._unitOfWork.SurveyPermissions.GetPermissionsForSurveyAsync(this.User.Identity.Name, surveyId);
			bool hasExecuteSurveyPermissions = surveyPermissions.Permissions.Contains("survey.execute");
			return hasExecuteSurveyPermissions;
		}

        private void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

    }
}