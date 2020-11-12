using Common.Module.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using NUnit.Framework;
using PrzeliczenieJednostek.Module.BusinessObjects;
using PrzeliczenieJednostek.Module.Helpers;
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
            var kalkulacja = objectSpace.CreateObject<ParametryPodazy>();
            var tabelaKonwersji = this.objectSpace.CreateCollection(typeof(TabelaKonwersji));
            Assert.IsNotNull(gram);
            Assert.IsNotNull(mgram);
            Assert.IsNotNull(mol);
            Assert.IsNotNull(kalkulacja);
            Assert.IsNotNull(tabelaKonwersji);
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
            kalkulacja.IloscWagowa = 5;
            Assert.AreEqual(25m, kalkulacja.IloscMolowa);
        }

        [Test]
        public void TestCzyJednostkiSaPoprawnieObliczoneZMolNaMmol()
        {
            var kalkulacja = objectSpace.CreateObject<ParametryPodazy>();
            var mmol = objectSpace.FindObject<JednostkaLicznosci>(new BinaryOperator(nameof(JednostkaLicznosci.JM), "mmol"));
            var mol = objectSpace.FindObject<JednostkaLicznosci>(new BinaryOperator(nameof(JednostkaLicznosci.JM), "mol"));
            var przekonwertowaneWartosci = PrzeliczenieJednostekHelper.GetPrzeliczonaJednostka(0.005m, mol);

            var tuple = new Tuple<JednostkaMiary, decimal>(mmol, 5);
            Assert.AreEqual(tuple, przekonwertowaneWartosci);
        }

        [Test]
        public void TestCzyPoprawnieSieUstawiajaDaneZTabeliKonwersji()
        {
            var gram = objectSpace.FindObject<JednostkaWagi>(new BinaryOperator(nameof(JednostkaWagi.JM), "g"));
            var molFromTabelaKonwersji = this.objectSpace.FindObject<TabelaKonwersji>(new BinaryOperator(nameof(JednostkaWagi), gram)).JednostkaLicznosci;
            var oczekiwanyMol = objectSpace.FindObject<JednostkaLicznosci>(new BinaryOperator(nameof(JednostkaLicznosci.JM), "mol"));

            Assert.AreEqual(oczekiwanyMol, molFromTabelaKonwersji);
        }

        [Test]
        public void TestZamieniamyGramyNaInneJednostki()
        {
            var gram = objectSpace.FindObject<JednostkaWagi>(new BinaryOperator(nameof(JednostkaWagi.JM), "g"));
            decimal iloscGramy = 2m;
            var mgram = objectSpace.FindObject<JednostkaWagi>(new BinaryOperator(nameof(JednostkaWagi.JM), "mg"));
            var parametr = objectSpace.FindObject<Parametr>(new BinaryOperator(nameof(Parametr.Nazwa), "Rtęć"));

            var kalkulacja = objectSpace.CreateObject<ParametryPodazy>();
            kalkulacja.Parametr = parametr;
            kalkulacja.JednostkaWagowa = gram;
            kalkulacja.IloscWagowa = iloscGramy;
            kalkulacja.JednostkaWagowa = mgram;
            
            decimal wartoscOczekiwanaWMg = 2000m;
            Assert.AreEqual(wartoscOczekiwanaWMg, kalkulacja.IloscWagowa);
        }

        [Test]
        public void TestIloscWMolachNiePowinnaSieZmienic()
        {
            var gram = objectSpace.FindObject<JednostkaWagi>(new BinaryOperator(nameof(JednostkaWagi.JM), "g"));
            decimal iloscGramy = 2m;
            var mol = objectSpace.FindObject<JednostkaLicznosci>(new BinaryOperator(nameof(JednostkaLicznosci.JM), "mol"));
            var mgram = objectSpace.FindObject<JednostkaWagi>(new BinaryOperator(nameof(JednostkaWagi.JM), "mg"));

            var kalkulacja = objectSpace.CreateObject<ParametryPodazy>();
            var parametr = objectSpace.FindObject<Parametr>(new BinaryOperator(nameof(Parametr.Nazwa), "Rtęć"));
            kalkulacja.Parametr = parametr;
            kalkulacja.JednostkaWagowa = gram;
            kalkulacja.IloscWagowa = iloscGramy;

            kalkulacja.JednostkaWagowa = mgram;
            kalkulacja.IloscMolowa = 1m;
            kalkulacja.JednostkaMolowa = mol;

            Assert.AreEqual(1, kalkulacja.IloscMolowa);
            Assert.AreEqual(200000m, kalkulacja.IloscWagowa);
        }

        [Test]
        public void TestZamieniamyMoleNaInneJednostki()
        {
            decimal iloscMole = 2m;
            var mol = objectSpace.FindObject<JednostkaLicznosci>(new BinaryOperator(nameof(JednostkaLicznosci.JM), "mol"));
            var mmol = objectSpace.FindObject<JednostkaLicznosci>(new BinaryOperator(nameof(JednostkaLicznosci.JM), "mmol"));

            var kalkulacja = objectSpace.CreateObject<ParametryPodazy>();
            kalkulacja.JednostkaMolowa = mol;
            kalkulacja.IloscMolowa = iloscMole;
            kalkulacja.JednostkaMolowa = mmol;

            decimal wartoscOczekiwanaWMg = 0.002m;
            Assert.AreEqual(wartoscOczekiwanaWMg, kalkulacja.IloscMolowa);
        }

        /// <summary>
        /// 2gr zmieniamy na miligramy, powinno wyjsc 2000mg
        /// Ilość w molach nie powinna ulec zmianie
        /// To samo w molach: jak zmienimy 2ml na milimole to powinno wyjsc 2000mmol
        /// Ilosc wagowa nie powinna ulec zmianie 
        /// </summary>

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
