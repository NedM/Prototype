using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace MagCardLib
{
    /// <summary>
    /// Class for handling magnetic card stripe data for Financial cards
    /// </summary>
    /// <remarks>Implemented according to http://www.gae.ucm.es/~padilla/extrawork/tracks.html</remarks>
    public class MagCard
    {
        private const char DEFAULT_TRACK1_START_SENTINEL = '%';
        private const char FINANCIAL_CARD_FORMAT = 'B';
        private const char DEFAULT_TRACK1_FIELD_SEPARATOR = '^';
        private const char DEFAULT_TRACK_END_SENTINEL = '?';
        private const char DEFAULT_TRACK2_START_SENTINEL = ';';
        private const char DEFAULT_TRACK2_FIELD_SEPARATOR = '=';
        private const char NAME_SEPARATOR_CHAR = '/';
        private const char INITIAL_SEPARATOR_CHAR = ' ';
        private const int EXPIRATION_DATE_LENGTH = 4;
        private const int EXPIRATION_MONTH_LENGTH = 2;
        private const int EXPIRATION_YEAR_LENGTH = 2;
        private const int SERVICE_CODE_LENGTH = 3;
        private const int MASK_LENGTH = 12;
        private const char MASK_CHAR = '*';

        private static char _track1StartSentinel;
        private static char _formatCode;
        private static char _track1FieldSeparator;

        private static char _trackEndSentinel;

        private static char _track2StartSentinel;
        private static char _track2FieldSeparator;

        static MagCard()
        {
            _track1StartSentinel = DEFAULT_TRACK1_START_SENTINEL;
            _formatCode = FINANCIAL_CARD_FORMAT;
            _track1FieldSeparator = DEFAULT_TRACK1_FIELD_SEPARATOR;
            _trackEndSentinel = DEFAULT_TRACK_END_SENTINEL;

            _track2StartSentinel = DEFAULT_TRACK2_START_SENTINEL;
            _track2FieldSeparator = DEFAULT_TRACK2_FIELD_SEPARATOR;
        }

        public static void Configure(char track1StartSentinel = DEFAULT_TRACK1_START_SENTINEL,
                                     char formatCode = FINANCIAL_CARD_FORMAT,
                                     char track1FieldSeparator = DEFAULT_TRACK1_FIELD_SEPARATOR,
                                     char track1EndSentinel = DEFAULT_TRACK_END_SENTINEL,
                                     char track2StartSentinel = DEFAULT_TRACK2_START_SENTINEL,
                                     char track2FieldSeparator = DEFAULT_TRACK2_FIELD_SEPARATOR)
        {
            _track1StartSentinel = track1StartSentinel;
            _formatCode = formatCode;
            _track1FieldSeparator = track1FieldSeparator;
            _trackEndSentinel = track1EndSentinel;

            _track2StartSentinel = track2StartSentinel;
            _track2FieldSeparator = track2FieldSeparator;
        }

        #region Parsing methods

        public static MagCard Parse(string magCardData)
        {
            if (string.IsNullOrEmpty(magCardData))
            {
                throw new ArgumentException("No data to parse!", magCardData);
            }

            MagCard card = new MagCard();

            try
            {
                MagCard.ParseData(ref card, magCardData);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid Magnetic Card Data!", ex);
            }

            return card;
        }

        private static void ParseData(ref MagCard card, string magCardData)
        {
            string[] tracks = magCardData.Split(new char[] {_trackEndSentinel}, StringSplitOptions.None);

            foreach (string track in tracks)
            {
                if (track.Length < 1)
                {
                    continue;
                }

                char firstChar = track[0];

                if (firstChar.Equals(_track1StartSentinel))
                {
                    ParseTrack1Data(ref card, track.TrimStart(_track1StartSentinel).TrimStart(_formatCode));
                }
                else if (firstChar.Equals(_track2StartSentinel))
                {
                    ParseTrack2Data(ref card, track.TrimStart(_track2StartSentinel));
                }
                else
                {
                    ParseTrack3Data(ref card, track);
                }
            }
        }

        private static void ParseTrack3Data(ref MagCard card, string track3Data)
        {
            //throw new NotImplementedException();

            //No op. Drop data on the floor
        }

        private static void ParseTrack2Data(ref MagCard card, string track2Data)
        {
            string[] tokenized = track2Data.Split(_track2FieldSeparator);

            if (tokenized.Length != 2)
            {
                throw new InvalidDataException("Track 2 data has invalid format! Data: " + track2Data);
            }

            string track2PrimaryAccountNumber = tokenized[0];

            if (!string.IsNullOrEmpty(track2PrimaryAccountNumber) &&
                (null == card.PrimaryAccountNumber || !track2PrimaryAccountNumber.Equals(card.PrimaryAccountNumber)))
            {
                card.PrimaryAccountNumber = track2PrimaryAccountNumber;
            }

            string expirationEtc = tokenized[1];

            string track2ExpirationDate = ParseExpirationDate(expirationEtc);

            if (!string.IsNullOrEmpty(track2ExpirationDate) &&
                (null == card.ExpirationDate || !track2ExpirationDate.Equals(card.ExpirationDate)))
            {
                card.ExpirationDate = track2ExpirationDate;
            }

            string track2ServiceCode = ParseServiceCode(expirationEtc);

            if (!string.IsNullOrEmpty(track2ServiceCode) &&
                (null == card.ServiceCode || !track2ServiceCode.Equals(card.ServiceCode)))
            {
                card.ServiceCode = track2ServiceCode;
            }

            string track2DiscretionaryData = ParseDiscretionaryData(expirationEtc);

            if (!string.IsNullOrEmpty(track2DiscretionaryData) &&
                (null == card.DiscretionaryData.Track2 ||
                 !track2DiscretionaryData.Equals(card.DiscretionaryData.Track2)))
            {
                card.DiscretionaryData.Track2 = track2DiscretionaryData;
            }
        }

        private static void ParseTrack1Data(ref MagCard card, string track1Data)
        {
            string[] tokenized = track1Data.Split(_track1FieldSeparator);

            card.PrimaryAccountNumber = tokenized[0];

            ParseNameComponents(ref card, tokenized[1]);

            string expirationEtc = tokenized[2];

            card.ExpirationDate = ParseExpirationDate(expirationEtc);
            card.ServiceCode = ParseServiceCode(expirationEtc);
            card.DiscretionaryData.Track1 = ParseDiscretionaryData(expirationEtc);

            // DEBUG
            System.Diagnostics.Debug.Assert(string.Concat(card.ExpirationDate,
                                                          card.ServiceCode,
                                                          card.DiscretionaryData).Equals(expirationEtc));
            // END DEBUG
        }

        private static void ParseNameComponents(ref MagCard card, string fullName)
        {
            card.Name = fullName;

            string[] nameComponents = card.Name.Split(NAME_SEPARATOR_CHAR);

            if (nameComponents.Length > 0)
            {
                card.LastName = nameComponents[0].Trim();

                if (nameComponents.Length > 1)
                {
                    string[] firstNameMiddleInitial = nameComponents[1].Split(INITIAL_SEPARATOR_CHAR);

                    if (firstNameMiddleInitial.Length > 0)
                    {
                        card.FirstName = firstNameMiddleInitial[0].Trim();
                        
                        if (firstNameMiddleInitial.Length > 1)
                        {
                            card.MiddleInitial = firstNameMiddleInitial[1].Trim();
                        }
                    }
                }
            }
        }

        private static string ParseDiscretionaryData(string expirationDateEtc)
        {
            return expirationDateEtc.Substring(0 + EXPIRATION_DATE_LENGTH + SERVICE_CODE_LENGTH);
        }

        private static string ParseExpirationDate(string expirationDateEtc)
        {
            return expirationDateEtc.Substring(0, EXPIRATION_DATE_LENGTH);
        }

        private static string ParseServiceCode(string expirationDateEtc)
        {
            return expirationDateEtc.Substring(0 + EXPIRATION_DATE_LENGTH, SERVICE_CODE_LENGTH);
        }

        #endregion Parsing methods

        private MagCard()
        {
            DiscretionaryData = new DiscretionaryData();
        }

        /// <summary>
        /// 1, 3, or 4 characters
        /// Pin Verification Key Indicator (PKVI) = 1 char
        /// Pin Verification Value (PVV) = 4 chars
        /// Card Verification Value (CVV) | Card Verification Code (CVC) = 3 chars
        /// </summary>
        public DiscretionaryData DiscretionaryData { get; private set; }

        /// <summary>
        /// YYMM format
        /// </summary>
        public string ExpirationDate { get; private set; }

        public string ExpirationMonth
        {
            get
            {
                return ExpirationDate.Substring(0 + EXPIRATION_YEAR_LENGTH, EXPIRATION_MONTH_LENGTH);
            }
        }

        public string ExpirationYear { get { return ExpirationDate.Substring(0, EXPIRATION_YEAR_LENGTH); } }

        public string FirstName { get; private set; }

        public string Name { get; private set; }

        public string LastName { get; private set; }

        public string MiddleInitial { get; set; }

        public string PAN { get { return PrimaryAccountNumber; } }

        /// <summary>
        /// Max 19 characters
        /// </summary>
        public string PrimaryAccountNumber { get; private set; }

        /// <summary>
        /// 3 characters
        /// </summary>
        public string ServiceCode { get; private set; }

        public override string ToString()
        {
            return this.ToString(true);
        }

        public string ToString(bool formatAsMagCardSwipedData)
        {
            if (formatAsMagCardSwipedData)
            {
                return string.Concat(Track1DataToString(), Track2DataToString());
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("Name: {0} {1} {2} ({3})", FirstName, MiddleInitial, LastName, Name);

                sb.AppendFormat(", PAN: {0}", PAN.Replace(PAN.Substring(0, MASK_LENGTH),
                                                          new string(MASK_CHAR, MASK_LENGTH)));

                sb.AppendFormat(", Service Code: {0}", ServiceCode);

                sb.AppendFormat(", Expires: {0}/{1}", ExpirationMonth, ExpirationYear);

                sb.AppendFormat(", {0}", DiscretionaryData);

                return sb.ToString();
            }
        }

        public string Track1DataToString()
        {
            return string.Format("{0}{1}{2}{3}{4}{3}{5}{6}{7}{8}",
                                 _track1StartSentinel,
                                 _formatCode,
                                 PrimaryAccountNumber,
                                 _track1FieldSeparator,
                                 Name,
                                 ExpirationDate,
                                 ServiceCode,
                                 DiscretionaryData.Track1,
                                 _trackEndSentinel);
        }

        public string Track2DataToString()
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}",
                                 _track2StartSentinel,
                                 PrimaryAccountNumber,
                                 _track2FieldSeparator,
                                 ExpirationDate,
                                 ServiceCode,
                                 DiscretionaryData.Track2,
                                 _trackEndSentinel);
        }
    }
}
