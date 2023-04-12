using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Common;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Person;
using UserManagement;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.IO;
using System.Net.Http.Headers;
using Models.ViewModels.DSAuth.Setup;
using Authentication.AuthSchemes;

namespace APIGateway.Controllers.Person
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JWT_BEARER_TOKEN_STATELESS)]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService personService;
        private readonly IUserManagement userManagementService;

        public VwDSUser User
        {
            get
            {
                return (VwDSUser)this.Request.HttpContext.Items["User"];
            }
        }
        public PersonController(IPersonService personService, IUserManagement userManagementService)
        {
            this.personService = personService;
            this.userManagementService = userManagementService;
        }
        #region GET-APIs
        [HttpGet]
        public async Task<ApiResponse> GetPersonInfoByCNIC(string cnic)
        {
            try
            {
                personService.VwDSUser = User;
                DataSet resultData = await personService.GetPersonInfoByCNIC(cnic);
                var lstPersonInfo = resultData.Tables[0].ToList<VwDSPerson>();

                var apiResponseType = lstPersonInfo.Count > 0 ? ApiResponseType.SUCCESS : ApiResponseType.NOT_FOUND;
                var msg = lstPersonInfo.Count > 0 ? Constants.RECORD_FOUND_MESSAGE : Constants.NOT_FOUND_MESSAGE;

                return ApiResponse.GetApiResponse(apiResponseType, lstPersonInfo, msg);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        //#region POST-APIs
        //[HttpPost]
        //public async Task<ApiResponse> SavePersonDocument(VwPersonDocument personDocument)
        //{
        //    try
        //    {
        //        personService.VwDSUser = User;

        //        DataSet resultData = await this.personService.SavePersonDocument(personDocument);
        //        object data;
        //        var apiResponseType = ApiResponseType.SUCCESS;
        //        var msg = Constants.DATA_SAVED_MESSAGE;

        //        long docId;
        //        long.TryParse(resultData.Tables[0].Rows[0][0].ToString(), out docId);

        //        if (resultData.Tables.Count > 0 && docId > 0)
        //        {
        //            apiResponseType = ApiResponseType.SUCCESS;
        //            msg = Constants.DATA_SAVED_MESSAGE;
        //            data = new
        //            {
        //                DocumentId = resultData.Tables[0].Rows[0][0].ToString()
        //            };

        //        }
        //        else
        //        {
        //            apiResponseType = ApiResponseType.FAILED;
        //            msg = Constants.DATA_NOT_SAVED_MESSAGE;
        //            data = null;
        //        }

        //        return ApiResponse.GetApiResponse(apiResponseType, data, msg);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //[HttpPost, DisableRequestSizeLimit]
        //public async Task<IActionResult> Upload()
        //{
        //    try
        //    {
        //        var currentYear = DateTime.Now.Year;
        //        var currentMonth = DateTime.Now.Month;
        //        var currentDay = DateTime.Now.Day;
        //        var formCollection = await Request.ReadFormAsync();
        //        var file = formCollection.Files.First();
        //        var folderName = Path.Combine("Attachments", "ConsumerPhotos");
        //        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //        if (!Directory.Exists(folderName))
        //        {
        //            Directory.CreateDirectory(folderName);
        //        }
        //        if (file.Length > 0)
        //        {
        //            Guid newName = Guid.NewGuid();
        //            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //            fileName = newName + "_" + fileName;
        //            var fullPath = Path.Combine(pathToSave, fileName);
        //            var dbPath = Path.Combine(folderName, fileName);
        //            using (var stream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //            }
        //            var applicationIdIndex = fileName.LastIndexOf("_");
        //            var applicationId = "";
        //            if (applicationIdIndex != -1)
        //            {
        //                string[] parts = fileName.Split('_');
        //                applicationId = parts[1];
        //                var personDocs = new VwPersonDocument();

        //                personDocs.DocumentId = 0;
        //                personDocs.DocumentName = fileName;
        //                personDocs.DocumentDescription = "Person Photo";
        //                personDocs.DocumentPath = fullPath;
        //                personDocs.DocumentType = "Photo";
        //                personDocs.ApplicationId = Int64.Parse(applicationId);
        //                personDocs.PersonId = 0;
        //                await SavePersonDocument(personDocs);

        //            }
        //            return Ok(new { dbPath});
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex}");
        //    }
        //}

        ////[HttpPost]
        ////public async Task<ApiResponse> GetPersonImage(long applicationId)
        ////{
        ////    try
        ////    {
        ////        personService.VwDSUser = User;
        ////        DataSet resultData = await personService.GetPersonImage(applicationId);
        ////        var personImage = resultData.Tables[0].Rows.Count > 0 ? resultData.Tables[0].Rows[0][0].ToString() : null;

        ////        var apiResponseType = string.IsNullOrWhiteSpace(personImage) ? ApiResponseType.NOT_FOUND : ApiResponseType.SUCCESS;
        ////        var msg = string.IsNullOrWhiteSpace(personImage) ? Constants.NOT_FOUND_MESSAGE : Constants.RECORD_FOUND_MESSAGE;

        ////        return ApiResponse.GetApiResponse(apiResponseType, personImage, msg);
        ////    }
        ////    catch (Exception)
        ////    {
        ////        throw;
        ////    }
        ////}
        ////#endregion
    }
}
