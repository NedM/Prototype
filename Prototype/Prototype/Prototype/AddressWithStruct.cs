﻿using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Prototype
{
    namespace Prototype
    {
        /// <summary>
        /// Class representing an address object for a business or individual
        /// </summary>
        [DataContract(Name = "AddressWithStruct", Namespace = "DesignPatterns")]
        [JsonObject]
        public class AddressWithStruct
        {
            public struct GeoLocation
            {
                public readonly double Latitude;
                public readonly double Longitude;

                public GeoLocation(double latitude, double longitude)
                {
                    Latitude = latitude;
                    Longitude = longitude;
                }
            }

            /// <summary>
            /// Constructor for the Address object
            /// </summary>
            /// <param name="name">Name of the addressee</param>
            /// <param name="streetAddress">Line 1 of street address of addressee. e.g. 123 Main St.</param>
            /// <param name="extendedAddress">Line 2 of street address of addressee. e.g. Apt. #1A</param>
            /// <param name="city">City or town of addressee</param>
            /// <param name="state">State of addressee</param>
            /// <param name="region">Region, county, or province of addressee</param>
            /// <param name="postalCode">Zip code or postal code for addressee</param>
            /// <param name="country">Nation of addressee</param>
            /// <param name="website">Web site for addressee</param>
            /// <param name="coordinates">Latitude and longitudinal coordinates of addressee for mapping</param>
            public AddressWithStruct(string name,
                                     string streetAddress = "",
                                     string extendedAddress = "",
                                     string city = "",
                                     string state = "",
                                     string region = "",
                                     string postalCode = "",
                                     string country = "U.S.A.",
                                     double latitude = 0,
                                     double longitude = 0,
                                     Uri website = null)
            {
                this.Name = name;
                this.StreetAddress = streetAddress;
                this.ExtendedAddress = extendedAddress;
                this.City = city;
                this.State = state;
                this.Region = region;
                this.PostalCode = postalCode;
                this.Country = country;
                this.Website = website;
                this.GeographicCoordinates = new GeoLocation(latitude, longitude);
            }

            /// <summary>
            /// Name of the addressee
            /// </summary>
            [DataMember]
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Basic street address. e.g. 123 Main st.
            /// </summary>
            [DataMember]
            [JsonProperty(PropertyName = "street_address")]
            public string StreetAddress { get; set; }

            /// <summary>
            /// Address line 2. Apartment number etc.
            /// </summary>
            [DataMember]
            [JsonProperty(PropertyName = "extended_address")]
            public string ExtendedAddress { get; set; }

            /// <summary>
            /// The city of the addressee
            /// </summary>
            [DataMember]
            [JsonProperty(PropertyName = "city")]
            public string City { get; set; }

            /// <summary>
            /// The state of the addressee. This may be the same as region.
            /// </summary>
            [DataMember]
            [JsonProperty(PropertyName = "state")]
            public string State { get; set; }

            /// <summary>
            /// If state is not appropriate, use region to define the general area where the addressee lives
            /// </summary>
            [DataMember]
            [JsonProperty(PropertyName = "region")]
            public string Region { get; set; }

            /// <summary>
            /// Zip code for addressee
            /// </summary>
            [DataMember]
            [JsonProperty(PropertyName = "postal_code")]
            public string PostalCode { get; set; }

            /// <summary>
            /// Nation where addressee lives
            /// </summary>
            [DataMember]
            [JsonProperty(PropertyName = "country")]
            public string Country { get; set; }

            /// <summary>
            /// Website for addressee. May be set to null where not applicable
            /// </summary>
            [DataMember]
            [JsonProperty(PropertyName = "website")]
            public Uri Website { get; set; }

            /// <summary>
            /// Geographic coordinates of the addressee if appropriate. May be set to null.
            /// </summary>
            [DataMember]
            [JsonProperty(PropertyName = "geographic_coordinates")]
            public GeoLocation GeographicCoordinates { get; private set; }

            public override string ToString()
            {
                return ToString(false);
            }

            public string ToString(bool includeValueDescriptors = false)
            {
                StringBuilder sb = new StringBuilder();

                AppendLineIfNotNullOrEmpty(ref sb, Name, "Name", includeValueDescriptors);
                AppendLineIfNotNullOrEmpty(ref sb, StreetAddress, "Address 1", includeValueDescriptors);
                AppendLineIfNotNullOrEmpty(ref sb, ExtendedAddress, "Address 2", includeValueDescriptors);

                string regionString = string.Empty;
                if (!string.IsNullOrEmpty(Region) && Region != State)
                {
                    regionString = " " + Region;
                }

                AppendLineIfNotNullOrEmpty(ref sb,
                                           string.Format("{0}, {1}{2} {3}",
                                                         City,
                                                         State,
                                                         regionString,
                                                         PostalCode),
                                           "Locale",
                                           includeValueDescriptors);

                AppendLineIfNotNullOrEmpty(ref sb, Country, "Country", includeValueDescriptors);

                if (null != Website)
                {
                    AppendLineIfNotNullOrEmpty(ref sb, Website.ToString(), "Website", includeValueDescriptors);
                }

                return sb.ToString();
            }

            private void AppendLineIfNotNullOrEmpty(ref StringBuilder builder,
                                                    string value,
                                                    string valueDescriptor,
                                                    bool includeValueDescriptor)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    builder.AppendLine(includeValueDescriptor
                                           ? string.Format("{0}: {1}", valueDescriptor, value)
                                           : value);
                }
            }
        }
    }

}
