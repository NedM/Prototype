using LevelUp.Integrations.Configuration;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace eThor.Agilysys
{
    [DataContract(Name = "EThorAgilysysConfig", Namespace = "http://www.TheLevelUp.com")]
    [JsonObject]
    public class EThorAgilysysConfig : IIntegrationConfigSection
    {
        /// <summary>
        /// App Id #353 
        /// </summary>
        public string ClientId
        {
            get { return "304e3a8a9f39a0e631eff75a45fbcf537f175c110eba4f18aaf38b0ac1549bb8"; }
        }

        public string DisplayName
        {
            get { return "eThor Agilysys"; }
        }

        public bool IsValid(out string[] errors)
        {
            errors = new string[]{};

            //TODO: If you add properties to this class, do validation of the user input in this class
            //For example, if you add an integer property for TenderId:
            //
            //string tenderIdError;
            //if (!TenderId.HasValue)
            //{
            //    errorsList.Add("Tender ID is not set!");
            //}
            //else if (!Utilities.ValidationUtils.IsPositiveInteger(TenderId.Value,
            //                                                      "Tender ID",
            //                                                      out tenderIdError))
            //{
            //    errorsList.Add(tenderIdError);
            //}
            //
            //errors = errorsList.ToArray();
            //
            //return errorsList.Length > 0

            return true;
        }
    }
}
