
namespace MagCardLib.Test
{
    public static class MagCardTestData
    {
        public const string CARD_DATA = "%B1234123412341234^CardUser/John Q^030510100000019301000000877000000?" +
                                        ";1234123412341234=0305101193010877?";
        
        public static class Valid
        {
            public static class Track1Only
            {
                public const string OTHER = @"%B1234123412341234^CardUser/John Q^030510100000019301000000877000000?";
            }

            public static class Track2Only
            {
                public const string MASTER_CARD = @";5301250070000191=08051010912345678901?3";
                public const string OTHER = @";1234123412341234=0305101193010877?";
            }
        }
    }
}
