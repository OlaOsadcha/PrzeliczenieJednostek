using Common.Module.Module.BusinessObjects;
using DevExpress.ExpressApp;
using PrzeliczenieJednostek.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzeliczenieJednostek.Module.Testy
{
   public class DataGenerator
    {
        private IObjectSpace objectSpace;
        public DataGenerator(IObjectSpace objectSpace)
        {
            this.objectSpace = objectSpace;
            XafTypesInfo.Instance.RegisterEntity(typeof(JednostkaMiary));
            XafTypesInfo.Instance.RegisterEntity(typeof(JednostkaWagi));
            XafTypesInfo.Instance.RegisterEntity(typeof(JednostkaLicznosci));
            XafTypesInfo.Instance.RegisterEntity(typeof(TabelaKonwersji));
            XafTypesInfo.Instance.RegisterEntity(typeof(Parametr));
            XafTypesInfo.Instance.RegisterEntity(typeof(ParametryPodazy));
        }

        public void DodajJednostki()
        {
           
            var gram = objectSpace.CreateObject<JednostkaWagi>();
            gram.JM = "g";
            gram.JednostkaBazowa = gram;
            gram.Przelicznik = 1;          

            var mg = objectSpace.CreateObject<JednostkaWagi>();
            mg.JM = "mg";
            mg.JednostkaBazowa = gram;
            mg.Przelicznik = 1000m;

            var mikrog = objectSpace.CreateObject<JednostkaWagi>();
            mikrog.JM = "μg";
            mikrog.JednostkaBazowa = gram;
            mikrog.Przelicznik = 1000000m;

            var mol = objectSpace.CreateObject<JednostkaLicznosci>();
            mol.JM = "mol";
            mol.JednostkaBazowa = mol;
            mol.Przelicznik = 1;

            var mmol = objectSpace.CreateObject<JednostkaLicznosci>();
            mmol.JM = "mmol";
            mmol.JednostkaBazowa = mol;
            mmol.Przelicznik = 1000m;

            var micromol = objectSpace.CreateObject<JednostkaLicznosci>();
            micromol.JM = "μmol";
            micromol.JednostkaBazowa = mol;
            micromol.Przelicznik = 1000000m;

            var molgram = objectSpace.CreateObject<TabelaKonwersji>();
            molgram.JednostkaLicznosci = mol;
            molgram.JednostkaWagi = gram;

            var mmolgram = objectSpace.CreateObject<TabelaKonwersji>();
            mmolgram.JednostkaLicznosci = mmol;
            mmolgram.JednostkaWagi = mg;

            var micromolgram = objectSpace.CreateObject<TabelaKonwersji>();
            micromolgram.JednostkaLicznosci = micromol;
            micromolgram.JednostkaWagi = mikrog;

            var rtec = objectSpace.CreateObject<Parametr>();
            rtec.Nazwa = "Rtęć";
            rtec.LiczbaMolowa = 200;
            rtec.JednostkaBazowa = mmol;

            var wapn = objectSpace.CreateObject<Parametr>();
            wapn.Nazwa = "Wapń";
            wapn.LiczbaMolowa = 40;
            wapn.JednostkaBazowa = mmol;

            var sod = objectSpace.CreateObject<Parametr>();
            sod.Nazwa = "Sód";
            sod.LiczbaMolowa = 23;
            sod.JednostkaBazowa = mmol;
        }
    }
}
