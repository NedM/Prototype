using System;
using System.Globalization;

namespace MagCardLib
{
    public class MagCard
    {
        private const char DEFAULT_TRACK1_START_SENTINEL = '%';
        private const char FINANCIAL_CARD_FORMAT = 'B';
        private const char DEFAULT_TRACK1_FIELD_SEPARATOR = '^';
        private const char DEFAULT_TRACK_END_SENTINEL = '?';
        private const char DEFAULT_TRACK2_START_SENTINEL = ';';
        private const char DEFAULT_TRACK2_FIELD_SEPARATOR = '=';

        private static char _track1StartSentinel;
        private static char _formatCode;
        private static char _track1FieldSeparator;
        private static char _track1EndSentinel;

        private static char _track2StartSentinel;
        private static char _track2FieldSeparator;
        private static char _track2EndSentinel;

        static MagCard()
        {
            _track1StartSentinel = DEFAULT_TRACK1_START_SENTINEL;
            _formatCode = FINANCIAL_CARD_FORMAT;
            _track1FieldSeparator = DEFAULT_TRACK1_FIELD_SEPARATOR;
            _track1EndSentinel = DEFAULT_TRACK_END_SENTINEL;

            _track2StartSentinel = DEFAULT_TRACK2_START_SENTINEL;
            _track2FieldSeparator = DEFAULT_TRACK2_FIELD_SEPARATOR;
            _track2EndSentinel = DEFAULT_TRACK_END_SENTINEL;
        }

        public static void Configure(char track1StartSentinel = DEFAULT_TRACK1_START_SENTINEL,
                                     char formatCode = FINANCIAL_CARD_FORMAT,
                                     char track1FieldSeparator = DEFAULT_TRACK1_FIELD_SEPARATOR,
                                     char track1EndSentinel = DEFAULT_TRACK_END_SENTINEL,
                                     char track2StartSentinel = DEFAULT_TRACK2_START_SENTINEL,
                                     char track2FieldSeparator = DEFAULT_TRACK2_FIELD_SEPARATOR,
                                     char track2EndSentinel = DEFAULT_TRACK_END_SENTINEL)
        {
            _track1StartSentinel = track1StartSentinel;
            _formatCode = formatCode;
            _track1FieldSeparator = track1FieldSeparator;
            _track1EndSentinel = track1EndSentinel;

            _track2StartSentinel = track2StartSentinel;
            _track2FieldSeparator = track2FieldSeparator;
            _track2EndSentinel = track2EndSentinel;
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
                ParseData(ref card, magCardData);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid Magnetic Card Data!", ex);
            }

            return card;
        }

        private static void ParseData(ref MagCard card, string magCardData)
        {
            if (magCardData.StartsWith(_track1StartSentinel.ToString(CultureInfo.InvariantCulture)))
            {
                string[] tracks = magCardData.Split(new char[] {_track1EndSentinel}, StringSplitOptions.None);

                if (!string.IsNullOrEmpty(tracks[0]))
                {
                    ParseTrack1Data(ref card, tracks[0]);
                }

                if (tracks.Length > 1 && !string.IsNullOrEmpty(tracks[1]))
                {
                    ParseTrack2Data(ref card, tracks[1]);
                }

                if (tracks.Length > 2 && !string.IsNullOrEmpty(tracks[2]))
                {
                    ParseTrack3Data(ref card, tracks[2]);
                }
            }
            else if (magCardData.StartsWith(_track2StartSentinel.ToString()))
            {
                ParseTrack2Data(ref card, magCardData);
            }
            else
            {
                throw new ArgumentException(string.Format("Unexpected start sentinel: \'{0}\'!", magCardData[0]),
                                            magCardData);
            }
        }

        private static void ParseTrack3Data(ref MagCard card, string track3Data)
        {
            throw new NotImplementedException();
        }

        private static void ParseTrack2Data(ref MagCard card, string track2Data)
        {
            string[] tokenized = track2Data.Split(_track2FieldSeparator);

            string track2PrimaryAccountNumber = tokenized[0].TrimStart(_track2StartSentinel);

            if (!string.IsNullOrEmpty(track2PrimaryAccountNumber) &&
                (null == card.PrimaryAccountNumber || !track2PrimaryAccountNumber.Equals(card.PrimaryAccountNumber)))
            {
                card.PrimaryAccountNumber = track2PrimaryAccountNumber;
            }

            string track2ExpirationDate = ParseExpirationDate(tokenized[1]);

            if (!string.IsNullOrEmpty(track2ExpirationDate) &&
                (null == card.ExpirationDate || !track2ExpirationDate.Equals(card.ExpirationDate)))
            {
                card.ExpirationDate = track2ExpirationDate;
            }

            string track2ServiceCode = ParseServiceCode(tokenized[1]);

            if (!string.IsNullOrEmpty(track2ServiceCode) &&
                (null == card.ServiceCode || !track2ServiceCode.Equals(card.ServiceCode)))
            {
                card.ServiceCode = track2ServiceCode;
            }

            string track2DiscretionaryData = ParseDiscretionaryData(tokenized[1]);

            if (!string.IsNullOrEmpty(track2DiscretionaryData) &&
                (null == card.DiscretionaryData || !track2DiscretionaryData.Equals(card.DiscretionaryData)))
            {
                card.DiscretionaryData = track2DiscretionaryData;
            }
        }

        private static void ParseTrack1Data(ref MagCard card, string track1Data)
        {
            string[] tokenized = track1Data.Split(_track1FieldSeparator);

            card.PrimaryAccountNumber = tokenized[0].Substring(2);

            card.Name = tokenized[1];

            card.ExpirationDate = ParseExpirationDate(tokenized[2]);
            card.ServiceCode = ParseServiceCode(tokenized[2]);
            card.DiscretionaryData = ParseDiscretionaryData(tokenized[2]);

            // DEBUG
            System.Diagnostics.Debug.Assert(string.Concat(card.ExpirationDate,
                                                          card.ServiceCode,
                                                          card.DiscretionaryData).Equals(tokenized[2]));
            // END DEBUG
        }

        private static string ParseDiscretionaryData(string expirationDateEtc)
        {
            return expirationDateEtc.Substring(7);
        }

        private static string ParseServiceCode(string expirationDateEtc)
        {
            const int serviceCodeLength = 3;
            return expirationDateEtc.Substring(4, serviceCodeLength);
        }

        private static string ParseExpirationDate(string expirationDateEtc)
        {
            return expirationDateEtc.Substring(0, 4);
        }

        #endregion Parsing methods

        private MagCard() { }

        /// <summary>
        /// 1, 3, or 4 characters
        /// Pin Verification Key Indicator (PKVI) = 1 char
        /// Pin Verification Value (PVV) = 4 chars
        /// Card Verification Value (CVV) | Card Verification Code (CVC) = 3 chars
        /// </summary>
        public string DiscretionaryData { get; private set; }

        /// <summary>
        /// YYMM format
        /// </summary>
        public string ExpirationDate { get; private set; }

        public string ExpirationMonth { get { return ExpirationDate.Substring(1, 2); } }

        public string ExpirationYear { get { return ExpirationDate.Substring(0, 2); } }

        public string Name { get; private set; }

        /// <summary>
        /// Max 19 characters
        /// </summary>
        public string PrimaryAccountNumber { get; private set; }

        public string PAN { get { return PrimaryAccountNumber; } }

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
            return string.Concat(Track1DataToString(), Track2DataToString());
        }

        private string Track1DataToString()
        {
            return string.Format("{0}{1}{2}{3}{4}{3}{5}{6}{7}{8}",
                                 _track1StartSentinel,
                                 _formatCode,
                                 PrimaryAccountNumber,
                                 _track1FieldSeparator,
                                 Name,
                                 ExpirationDate,
                                 ServiceCode,
                                 DiscretionaryData,
                                 _track1EndSentinel);
        }

        private string Track2DataToString()
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}",
                                 _track2StartSentinel,
                                 PrimaryAccountNumber,
                                 _track2FieldSeparator,
                                 ExpirationDate,
                                 ServiceCode,
                                 DiscretionaryData,
                                 _track2EndSentinel);
        }
    }
}
