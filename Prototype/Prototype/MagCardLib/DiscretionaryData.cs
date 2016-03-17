
namespace MagCardLib
{
    public class DiscretionaryData
    {
        public DiscretionaryData(string track1DiscretionaryData = null, string track2DiscretionaryData = null)
        {
            Track1 = track1DiscretionaryData;
            Track2 = track2DiscretionaryData;
        }

        public string Track1 { get; set; }

        public string Track2 { get; set; }

        public override string ToString()
        {
            return string.Format("Discretionary Data [1: {0}, 2: {1}]", Track1, Track2);
        }
    }
}
