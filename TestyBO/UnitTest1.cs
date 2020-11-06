using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using NUnit.Framework;

namespace TestyBO
{
    public class Tests
    {
        #region setup

        private IObjectSpace objectSpace;
        private XPObjectSpaceProvider directProvider;
        private string connectionString;
        /// private DataGenerator generator;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {​​​​​
          XpoDefault.Session = null;
        }​​​​​
      [SetUp]
        public void SetUp()
        {​​​​​
connectionString = InMemoryDataStoreProvider.ConnectionString;
            directProvider = new XPObjectSpaceProvider(connectionString, null);
            objectSpace = directProvider.CreateObjectSpace();
            // generator = new DataGenerator(objectSpace);
            //mol = generator.DodajJednostkeLicznosci("mol");
            //mmol = generator.DodajJednostkeLicznosci("mmol", 1000, "mol");
            //μmol = generator.DodajJednostkeLicznosci("μmol", 1000, "mmol");
            //nmol = generator.DodajJednostkeLicznosci("nmol", 1000000, "mmol");
            //pmol = generator.DodajJednostkeLicznosci("pmol", 1000000000, "mmol");
            //objectSpace.CommitChanges();
        }​​​​​

         [TearDown]
        public void TearDown()
        {​​​​​
            objectSpace.Dispose();
            directProvider.Dispose();
        }​​​​​
#endregion
    }
}