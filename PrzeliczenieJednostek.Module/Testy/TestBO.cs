using Common.Module.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using NUnit.Framework;
using PrzeliczenieJednostek.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzeliczenieJednostek.Module.Testy
{
    [TestFixture]
    public class TestBO
    {
        [Test]
        public void TestXpo()
        {
            Assert.IsNotNull(objectSpace);
        }

        [Test]
        public void TestSprawdzanieCzyJednostkaWagiNieJestNulem()
        {
            var mgram = objectSpace.FindObject<JednostkaWagi>(new BinaryOperator(nameof(JednostkaWagi.JM), "mg"));
            var mol = objectSpace.FindObject<JednostkaLicznosci>(new BinaryOperator(nameof(JednostkaLicznosci.JM), "mol"));
            var gram = objectSpace.FindObject<JednostkaWagi>(new BinaryOperator(nameof(JednostkaWagi.JM), "g"));
            Assert.IsNotNull(gram);
            Assert.IsNotNull(mgram);
            Assert.IsNotNull(mol);
        }

        [Test]
        public void TestCzyMamyRtec()
        {
            var parametr = objectSpace.FindObject<Parametr>(new BinaryOperator(nameof(Parametr.Nazwa), "Rtęć"));
            Assert.IsNotNull(parametr);
        }

        [Test]
        public void TestUtworzenieKalkulacjiPodazy()
        {
            var gram = objectSpace.FindObject<JednostkaWagi>(new BinaryOperator(nameof(JednostkaWagi.JM), "g"));
            var mgram = objectSpace.FindObject<JednostkaWagi>(new BinaryOperator(nameof(JednostkaWagi.JM), "mg"));
            var mol = objectSpace.FindObject<JednostkaLicznosci>(new BinaryOperator(nameof(JednostkaLicznosci.JM), "mol"));
            var kalkulacja = objectSpace.CreateObject<ParametryPodazy>();
            kalkulacja.JednostkaMolowa = mol;
            kalkulacja.JednostkaWagowa = gram;
            kalkulacja.IloscMolowa = 1;
            kalkulacja.IloscWagowa = 1;
            Assert.AreEqual(1, kalkulacja.IloscWagowa);

            kalkulacja.JednostkaWagowa = mgram;
            // Assert.AreEqual(1000, kalkulacja.IloscWagowa);

            var parametr = objectSpace.FindObject<Parametr>(new BinaryOperator(nameof(Parametr.Nazwa), "Rtęć"));
            Assert.IsNotNull(parametr);
            kalkulacja.Parametr = parametr;
            kalkulacja.IloscWagowa = 1;
            Assert.AreEqual(0.005m, kalkulacja.IloscMolowa);
        }

        IObjectSpace objectSpace;
        XPObjectSpaceProvider directProvider;
        string connection;

        [SetUp]
        public void SetUp()
        {
            connection = InMemoryDataStoreProvider.ConnectionString;
            directProvider = new XPObjectSpaceProvider(connection, null);
            objectSpace = directProvider.CreateObjectSpace();
            DataGenerator generator = new DataGenerator(objectSpace);
            generator.DodajJednostki();
            objectSpace.CommitChanges();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            XpoDefault.Session = null;

        }

        [TearDown]
        public void TearDown()
        {
            objectSpace.Dispose();
            directProvider.Dispose();
        }


    }
}
