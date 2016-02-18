using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Prototype.Test
{
    [TestClass]
    public class ConstructCheckTests
    {
        #region Raw Array Data

        private readonly string[] _inputArrayData = new string[]
                {
                    "this", "is", "some", "garbage", "data",
                    "5", "114001", "40105", "40305", "40306", "111001",
                    "5",      "1",     "1",     "1",     "1",      "1",
                    "5",   "0.63",  "0.64",  "0.65",  "0.66",   "0.67",
                    "5",      "a",     "b",     "c",     "d",      "e",
                    "5",     "AA",    "BB",    "CC",    "DD",     "EE",
                    "5",     "1a",    "2b",    "3c",    "4d",     "5e",
                };

        private readonly string[][] _outputArrayDataTarget = new string[][]
                {
                    new string[]{"114001", "1", "0.63", "a", "AA", "1a"},
                    new string[]{"40105", "1", "0.64", "b", "BB", "2b"},
                    new string[]{"40305", "1", "0.65", "c", "CC", "3c"},
                    new string[]{"40306", "1", "0.66", "d", "DD", "4d"},
                    new string[]{"111001", "1", "0.67", "e", "EE", "5e"},
                };

        private readonly string[] _microsInputArray = new string[]
            {
                //Line below is junk data representing a prefix
                "some", "irrelevant", "data", "prefixes", "real", "data",
                //Lines below represent names or ids of the items
                "19", "invalid item 1", "Burger", "Fries", "Extra Salt", "invalid item", "invalid sub item", 
                "Soup", "invalid sub item 2",
                "Chowdah", "Clam", "Extra oyster crackers", "Cold", "Really Cold", "Large", "Bowl", "Bread", "Wheat",
                "Pepsi Cola", "Large",
                //Line below represents menu item level. level 0 is top/parent level. 
                //higher numbers are sub items of the lower numbered items the preceed it
                "19", "0", "0", "0", "1", "0", "1", "0", "1", "1", "2", "2", "1", "2", "1", "1", "2", "3", "0", "1",
                //Line below indicates whether the item "isValid". 0 = invalid, 1 = valid
                "19", "0", "1", "1", "1", "0", "0", "1", "0", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1",
            };

        #endregion Raw Array Data

        #region MenuItem Data

        private readonly List<MenuItem> _inputItems = new List<MenuItem>()
            {
                new MenuItem("invalid item 1", 0, false),
                new MenuItem("Burger", 0),
                new MenuItem("Fries", 0),
                new MenuItem("Extra Salt", 1),
                new MenuItem("invalid item", 0, false),
                new MenuItem("invalid sub item", 1, false),
                new MenuItem("Soup", 0),
                new MenuItem("invalid sub item 2", 1, false),
                new MenuItem("Chowdah", 1),
                new MenuItem("Clam", 2),
                new MenuItem("Extra oyster crackers", 2),
                new MenuItem("Cold", 1),
                new MenuItem("Really Cold", 2),
                new MenuItem("Large", 1),
                new MenuItem("Bowl", 1),
                new MenuItem("Bread", 2),
                new MenuItem("Wheat", 3),
                new MenuItem("Pepsi Cola", 0),
                new MenuItem("Large", 1),
            };

        private readonly List<ConvertedItem> _microsOutputItems = new List<ConvertedItem>()
            {
                new ConvertedItem("Burger", 0),
                new ConvertedItem("Fries", 0, new List<ConvertedItem>()
                    {
                        new ConvertedItem("Extra Salt", 1),
                    }),
                new ConvertedItem("Soup", 0, new List<ConvertedItem>()
                    {
                        new ConvertedItem("Chowdah", 1),
                        new ConvertedItem("Clam", 1),
                        new ConvertedItem("Extra oyster crackers", 1),
                        new ConvertedItem("Cold", 1),
                        new ConvertedItem("Really Cold", 1),
                        new ConvertedItem("Large", 1),
                        new ConvertedItem("Bowl", 1),
                        new ConvertedItem("Bread", 1),
                        new ConvertedItem("Wheat", 1),
                    }),
                new ConvertedItem("Pepsi Cola", 0, new List<ConvertedItem>()
                    {
                        new ConvertedItem("Large", 1),
                    })
            }; 

        private readonly List<ConvertedItem> _alohaOutputItems = new List<ConvertedItem>()
            {
                new ConvertedItem("Burger", 0),
                new ConvertedItem("Fries", 0, new List<ConvertedItem>()
                    {
                        new ConvertedItem("Extra Salt", 1),
                    }),
                new ConvertedItem("Soup", 0, new List<ConvertedItem>()
                    {
                        new ConvertedItem("Chowdah", 1, new List<ConvertedItem>()
                            {
                                new ConvertedItem("Clam", 2),
                                new ConvertedItem("Extra oyster crackers", 2),
                            }),
                        new ConvertedItem("Cold", 1, new List<ConvertedItem>()
                            {
                                new ConvertedItem("Really Cold", 2),
                            }),
                        new ConvertedItem("Large", 1),
                        new ConvertedItem("Bowl", 1, new List<ConvertedItem>()
                            {
                                new ConvertedItem("Bread", 2, new List<ConvertedItem>()
                                    {
                                        new ConvertedItem("Wheat", 3),
                                    }),
                            }),
                    }),
                new ConvertedItem("Pepsi Cola", 0, new List<ConvertedItem>()
                    {
                        new ConvertedItem("Large", 1),
                    })
            };

        private readonly List<ConvertedItem> _itemsWithPrices = new List<ConvertedItem>()
            {
                new ConvertedItem("Burger", 550, 0),
                new ConvertedItem("Fries", 25, 0, new List<ConvertedItem>()
                    {
                        new ConvertedItem("Extra Salt", 1),
                    }),
                new ConvertedItem("Soup", 375, 0, new List<ConvertedItem>()
                    {
                        new ConvertedItem("Chowdah", 0, 1, new List<ConvertedItem>()
                            {
                                new ConvertedItem("Clam", 2),
                                new ConvertedItem("Extra oyster crackers", 10, 2),
                            }),
                        new ConvertedItem("Cold", 0,  1, new List<ConvertedItem>()
                            {
                                new ConvertedItem("Really Cold", 2),
                            }),
                        new ConvertedItem("Large", 100, 1),
                        new ConvertedItem("Bowl", 0, 1, new List<ConvertedItem>()
                            {
                                new ConvertedItem("Bread", 150, 2, new List<ConvertedItem>()
                                    {
                                        new ConvertedItem("Wheat", 3),
                                    }),
                            }),
                    }),
                new ConvertedItem("Pepsi Cola", 200, 0, new List<ConvertedItem>()
                    {
                        new ConvertedItem("Large", 50, 1),
                    })
            };

        #endregion MenuItem Data

        [TestMethod]
        public void ItemDataWithPrices()
        {
            string json = JsonConvert.SerializeObject(_itemsWithPrices);

            Trace.WriteLine(json);
        }

        [TestMethod]
        public void AlohaCheckData()
        {
            List<ConvertedItem> converted = AlohaCheck.BuildItemList(new Queue<MenuItem>(_inputItems));

            foreach (var convertedItem in converted)
            {
                Trace.WriteLine(convertedItem.ToString());
            }

            converted.ShouldAllBeEquivalentTo(_alohaOutputItems);
        }

        [TestMethod]
        public void MicrosCheckData()
        {
            List<ConvertedItem> converted = MicrosCheck.BuildItemList(_inputItems);

            foreach (var convertedItem in converted)
            {
                Trace.WriteLine(convertedItem.ToString());
            }

            converted.ShouldAllBeEquivalentTo(_microsOutputItems);
        }

        [TestMethod]
        public void MicrosCheckData_FromArray()
        {
            //In reference to pos-common PR https://github.com/TheLevelUp/pos-common/pull/220
            List<ConvertedItem> converted = MicrosCheck.BuildItemList(MicrosCheck.ConvertToItems(_microsInputArray, 6));

            foreach (var convertedItem in converted)
            {
                Trace.WriteLine(convertedItem.ToString());
            }

            converted.ShouldAllBeEquivalentTo(_microsOutputItems);
        }

        [TestMethod]
        public void MicrosCheckData_LastItemUnmodified()
        {
            List<MenuItem> inputList = _inputItems;
            List<ConvertedItem> outputList = _microsOutputItems;
            MenuItem additionalItem = new MenuItem("Snickers", 0);
            inputList.Add(additionalItem);
            outputList.Add(ConvertedItem.FromMenuItem(additionalItem));

            List<ConvertedItem> converted = MicrosCheck.BuildItemList(inputList);

            foreach (var convertedItem in converted)
            {
                Trace.WriteLine(convertedItem.ToString());
            }

            converted.ShouldAllBeEquivalentTo(outputList);
        }

        [TestMethod]
        public void ConvertMicrosData()
        {
            //In reference to pos-common PR https://github.com/TheLevelUp/pos-common/pull/220
            string[][] result = MicrosCheck.ConvertToItems(_inputArrayData, 5);

            result.Should().NotBeNull();
            result.Length.Should().NotBe(0);
            result.Length.Should().Be(_outputArrayDataTarget.Length);

            for (int i = 0; i < result.Length; i++)
            {
                Trace.WriteLine(string.Format("Target: {0}\n" +
                                              "Actual: {1}",
                                              string.Join(" ", _outputArrayDataTarget[i]),
                                              string.Join(" ", result[i])));
                result[i].ShouldAllBeEquivalentTo(_outputArrayDataTarget[i]);
            }
        }
    }
}
